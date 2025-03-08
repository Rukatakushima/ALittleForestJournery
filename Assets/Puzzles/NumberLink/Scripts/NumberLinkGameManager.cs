using System.Collections.Generic;
using UnityEngine;

namespace NumberLink
{
    public class GameManager : BaseGameManager<LevelSpawner, DefaultCameraController, WinConditionChecker, LevelData>
    {
        public static GameManager Instance;

        [SerializeField] private SpriteRenderer highlightSprite;
        [SerializeField] private Vector2 highlightSize;
        [HideInInspector] public float EdgeSize;

        private Cell[,] cellGrid;
        private Cell startCell;
        private Vector2 startPosition;

        private List<Vector2Int> Directions = new List<Vector2Int>()
        { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        public const int LINK_DIRECTIONS = 4;
        private const int RIGHT_DIRECTION_ID = 0;
        private const int TOP_DIRECTION_ID = 1;
        private const int LEFT_DIRECTION_ID = 2;
        private const int BOTTOM_DIRECTION_ID = 3;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();

            highlightSprite.gameObject.SetActive(false);
        }

        protected override void SetupManagers()
        {
            cameraController.SetupCamera(level.Columns * level.Rows);

            levelSpawner.Initialize(level);
            levelSpawner.SpawnLevel();

            winConditionChecker.Initialize(level, cellGrid);
        }

        public void SetCellGrid(Cell[,] cellGrid)
        {
            this.cellGrid = cellGrid;

            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    if (cellGrid[i, j] != null)
                    {
                        cellGrid[i, j].InitializeCell();
                    }
                }
            }
        }

        public void SetEdgeSize(float EdgeSize) => this.EdgeSize = EdgeSize;

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            startCell = null;
            startPosition = mousePosition;

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit && hit.collider.TryGetComponent(out startCell))
            {
                highlightSprite.gameObject.SetActive(true);
                highlightSprite.size = highlightSize;
                highlightSprite.transform.position = startCell.transform.position;
            }
            else
            {
                hit = Physics2D.Raycast(mousePosition, Vector2.left);
                if (hit && hit.collider.TryGetComponent(out startCell))
                    startCell.RemoveLink(RIGHT_DIRECTION_ID);

                hit = Physics2D.Raycast(mousePosition, Vector2.down);
                if (hit && hit.collider.TryGetComponent(out startCell))
                    startCell.RemoveLink(TOP_DIRECTION_ID);

                hit = Physics2D.Raycast(mousePosition, Vector2.right);
                if (hit && hit.collider.TryGetComponent(out startCell))
                    startCell.RemoveLink(LEFT_DIRECTION_ID);

                hit = Physics2D.Raycast(mousePosition, Vector2.up);
                if (hit && hit.collider.TryGetComponent(out startCell))
                    startCell.RemoveLink(BOTTOM_DIRECTION_ID);

                startCell = null;
                CheckWinCondition();
            }
        }

        protected override void HandleInputUpdate(Vector2 mousePosition)
        {
            if (startCell == null) return;

            Vector2 offset = mousePosition - startPosition;
            Vector2Int offsetDirection = GetDirection(offset);

            int directionIndex = GetDirectionIndex(offsetDirection);
            float offsetValue = GetOffset(offset, offsetDirection);
            Vector3 angle = new Vector3(0, 0, 90f * (directionIndex - 1));

            highlightSprite.transform.eulerAngles = angle;
            highlightSprite.size = new Vector2(highlightSize.x, offsetValue);
        }

        protected override void HandleInputEnd()
        {
            if (startCell == null) return;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit && hit.collider.TryGetComponent(out Cell endCell))
            {
                if (endCell == startCell)
                    startCell.RemoveAllLinks();

                else
                {
                    Vector2Int offsetDirection = GetDirection(mousePosition - startPosition);
                    int directionIndex = GetDirectionIndex(offsetDirection);

                    if (startCell.IsValidCell(endCell, directionIndex))
                    {
                        startCell.AddLink(directionIndex);

                        int oppositeDirectionID = startCell.OppositeDirections[directionIndex];
                        endCell.AddLink(oppositeDirectionID);
                    }
                }
            }

            startCell = null;
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

            if (angle >= -45 && angle < 45)
                return Vector2Int.right;
            else if (angle >= 45 && angle < 135)
                return Vector2Int.up;
            else if (angle >= -135 && angle < -45)
                return Vector2Int.down;
            else
                return Vector2Int.left;
        }

        public Cell GetLinkedCell(int row, int column, int direction)
        {
            Vector2Int currentDirection = Directions[direction];
            Vector2Int startPosition = new Vector2Int(row, column);
            Vector2Int checkPosition = startPosition + currentDirection;

            while (IsValid(checkPosition) && cellGrid[checkPosition.x, checkPosition.y] == null)
            {
                checkPosition += currentDirection;
            }

            return IsValid(checkPosition) ? cellGrid[checkPosition.x, checkPosition.y] : null;
        }

        public bool IsValid(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < level.Rows && pos.y < level.Columns;
    }
}