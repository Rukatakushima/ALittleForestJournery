using System.Collections.Generic;
using UnityEngine;

namespace NumberLink
{
    public class GameManager : BaseGameManager<LevelSpawner, DefaultCameraController, WinConditionChecker, LevelData>
    {
        public static GameManager Instance;

        [SerializeField] private SpriteRenderer highlightSprite;
        [SerializeField] private Vector2 highlightSize;
        public float EdgeSize;

        private Cell[,] cellGrid;
        private Cell startCell;
        private Vector2 startPosition;

        private List<Vector2Int> Directions = new List<Vector2Int>()
        { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

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
                        cellGrid[i, j].Init();
                    }
                }
            }
        }

        public void SetEdgeSize(float EdgeSize) => this.EdgeSize = EdgeSize;

        protected override void HandleMouseDown(Vector2 mousePosition)
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
                    startCell.RemoveEdge(0);

                hit = Physics2D.Raycast(mousePosition, Vector2.down);
                if (hit && hit.collider.TryGetComponent(out startCell))
                    startCell.RemoveEdge(1);

                hit = Physics2D.Raycast(mousePosition, Vector2.right);
                if (hit && hit.collider.TryGetComponent(out startCell))
                    startCell.RemoveEdge(2);

                hit = Physics2D.Raycast(mousePosition, Vector2.up);
                if (hit && hit.collider.TryGetComponent(out startCell))
                    startCell.RemoveEdge(3);

                startCell = null;
                CheckWinCondition();
            }
        }

        protected override void HandleMouseDrag(Vector2 mousePosition)
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

        protected override void HandleMouseUp()
        {
            if (startCell == null) return;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit && hit.collider.TryGetComponent(out Cell endCell))
            {
                if (endCell == startCell)
                {
                    startCell.RemoveAllEdges();
                    for (int i = 0; i < 4; i++)
                    {
                        Cell adjacentCell = GetAdjacentCell(startCell.row, startCell.column, i);
                        if (adjacentCell != null)
                        {
                            int adjacentDirection = (i + 2) % 4;
                            adjacentCell.RemoveEdge(adjacentDirection);
                            adjacentCell.RemoveEdge(adjacentDirection);
                        }
                    }
                }
                else
                {
                    Vector2 offset = mousePosition - startPosition;
                    Vector2Int offsetDirection = GetDirection(offset);
                    int directionIndex = GetDirectionIndex(offsetDirection);

                    if (startCell.IsValidCell(endCell, directionIndex))
                    {
                        startCell.AddEdge(directionIndex);
                        endCell.AddEdge((directionIndex + 2) % 4);
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
                result = 0;

            if (offsetDirection == Vector2Int.left)
                result = 2;

            if (offsetDirection == Vector2Int.up)
                result = 1;

            if (offsetDirection == Vector2Int.down)
                result = 3;

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
            Vector2Int result;

            if (Mathf.Abs(offset.y) > Mathf.Abs(offset.x))
            {
                if (offset.y > 0)
                    result = Vector2Int.up;
                else
                    result = Vector2Int.down;
            }
            else
            {
                if (offset.y > 0)
                    result = Vector2Int.right;
                else
                    result = Vector2Int.left;
            }

            return result;
        }

        public Cell GetAdjacentCell(int row, int column, int direction)
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