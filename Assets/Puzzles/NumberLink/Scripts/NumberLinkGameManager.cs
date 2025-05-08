using System.Collections.Generic;
using UnityEngine;

namespace NumberLink
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;
        
        [SerializeField] private SpriteRenderer highlightSprite;
        [SerializeField] private Vector2 highlightSize;
        public float EdgeSize { get; private set; }

        private Camera _camera;
        private LevelData _level;
        private Cell[,] _cellGrid;
        private Cell _startCell;
        private Vector2 _startPosition;
        private readonly List<Vector2Int> _directions = new () { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        private const int RIGHT_DIRECTION_ID = 0;
        private const int TOP_DIRECTION_ID = 1;
        private const int LEFT_DIRECTION_ID = 2;
        private const int BOTTOM_DIRECTION_ID = 3;

        private bool IsValid(Vector2Int pos) => pos is { x: >= 0, y: >= 0 } && pos.x < _level.rows && pos.y < _level.columns;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();

            highlightSprite.gameObject.SetActive(false);
        }

        private void Start()
        {
            if (Camera.main != null)
                _camera = Camera.main;
        }

        public void Initialize(LevelData levelData, Cell[,] cellsGrid, float size)
        {
            _level = levelData;
            EdgeSize = size;
            _cellGrid = cellsGrid;

            for (int i = 0; i < levelData.rows; i++)
            {
                for (int j = 0; j < levelData.columns; j++)
                {
                    if (cellsGrid[i, j] != null)
                        cellsGrid[i, j].InitializeCell();
                }
            }
        }

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            _startCell = null;
            _startPosition = mousePosition;

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit && hit.collider.TryGetComponent(out _startCell))
            {
                highlightSprite.gameObject.SetActive(true);
                highlightSprite.size = highlightSize;
                highlightSprite.transform.position = _startCell.transform.position;
            }
            else
            {
                hit = Physics2D.Raycast(mousePosition, Vector2.left);
                if (hit && hit.collider.TryGetComponent(out _startCell))
                    _startCell.RemoveLink(RIGHT_DIRECTION_ID);

                hit = Physics2D.Raycast(mousePosition, Vector2.down);
                if (hit && hit.collider.TryGetComponent(out _startCell))
                    _startCell.RemoveLink(TOP_DIRECTION_ID);

                hit = Physics2D.Raycast(mousePosition, Vector2.right);
                if (hit && hit.collider.TryGetComponent(out _startCell))
                    _startCell.RemoveLink(LEFT_DIRECTION_ID);

                hit = Physics2D.Raycast(mousePosition, Vector2.up);
                if (hit && hit.collider.TryGetComponent(out _startCell))
                    _startCell.RemoveLink(BOTTOM_DIRECTION_ID);

                _startCell = null;
                CheckWinCondition();
            }
        }

        protected override void HandleInputUpdate(Vector2 mousePosition)
        {
            if (_startCell == null) return;

            Vector2 offset = mousePosition - _startPosition;
            Vector2Int offsetDirection = GetDirection(offset);

            int directionIndex = GetDirectionIndex(offsetDirection);
            float offsetValue = GetOffset(offset, offsetDirection);
            Vector3 angle = new Vector3(0, 0, 90f * (directionIndex - 1));

            highlightSprite.transform.eulerAngles = angle;
            highlightSprite.size = new Vector2(highlightSize.x, offsetValue);
        }

        protected override void HandleInputEnd()
        {
            if (_startCell == null) return;

            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit && hit.collider.TryGetComponent(out Cell endCell))
            {
                if (endCell == _startCell)
                    _startCell.RemoveAllLinks();

                else
                {
                    Vector2Int offsetDirection = GetDirection(mousePosition - _startPosition);
                    int directionIndex = GetDirectionIndex(offsetDirection);

                    if (_startCell.IsValidCell(endCell, directionIndex))
                    {
                        _startCell.AddLink(directionIndex);

                        int oppositeDirectionID = _startCell.OppositeDirections[directionIndex];
                        endCell.AddLink(oppositeDirectionID);
                    }
                }
            }

            _startCell = null;
            highlightSprite.gameObject.SetActive(false);
            CheckWinCondition();
        }

        private int GetDirectionIndex(Vector2Int offsetDirection)
        {
            int result = 0;

            if (offsetDirection == Vector2Int.right)
                result = RIGHT_DIRECTION_ID;

            if (offsetDirection == Vector2Int.left)
                result = LEFT_DIRECTION_ID;

            if (offsetDirection == Vector2Int.up)
                result = TOP_DIRECTION_ID;

            if (offsetDirection == Vector2Int.down)
                result = BOTTOM_DIRECTION_ID;

            return result;
        }

        private float GetOffset(Vector2 offset, Vector2Int offsetDirection)
        {
            float result = 0;

            if (offsetDirection == Vector2Int.left || offsetDirection == Vector2Int.right)
            {
                result = Mathf.Abs(offset.x);
            }

            if (offsetDirection == Vector2Int.up || offsetDirection == Vector2Int.down)
            {
                result = Mathf.Abs(offset.y);
            }

            return result;
        }

        private Vector2Int GetDirection(Vector2 offset)
        {
            float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            return angle switch
            {
                >= -45 and < 45 => Vector2Int.right,
                >= 45 and < 135 => Vector2Int.up,
                >= -135 and < -45 => Vector2Int.down,
                _ => Vector2Int.left
            };
        }

        public Cell GetLinkedCell(int row, int column, int direction)
        {
            Vector2Int currentDirection = _directions[direction];
            Vector2Int startPos = new Vector2Int(row, column);
            Vector2Int checkPosition = startPos + currentDirection;

            while (IsValid(checkPosition) && _cellGrid[checkPosition.x, checkPosition.y] == null)
            {
                checkPosition += currentDirection;
            }

            return IsValid(checkPosition) ? _cellGrid[checkPosition.x, checkPosition.y] : null;
        }

        public override void CheckWinCondition()
        {
            for (int i = 0; i < _level.rows; i++)
            {
                for (int j = 0; j < _level.columns; j++)
                {
                    if (_cellGrid[i, j] != null && _cellGrid[i, j].Number != 0) return;
                }
            }

            OnWin?.Invoke();
        }
    }
}