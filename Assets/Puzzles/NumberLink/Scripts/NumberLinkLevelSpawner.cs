using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace NumberLink
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        // private LevelData level;

        [SerializeField] private Cell cellPrefab;
        [SerializeField] private SpriteRenderer backgroundSprite;

        [SerializeField] private float cellGap;
        [SerializeField] private float cellSize;
        [SerializeField] private float levelGap;
        [SerializeField] private float backgroundPositionController = 2f;

        public void Initialize(LevelData level) => this.level = level;

        public override void SpawnLevel()
        {
            // GameManager.Instance.SetEdgeSize(cellGap + cellSize);

            int[,] levelGrid = new int[level.Rows, level.Columns];
            FullfillLevelGridData(levelGrid);
            CreateBackcround();
            CreateCells(levelGrid);
        }

        private void FullfillLevelGridData(int[,] levelGrid)
        {
            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    levelGrid[i, j] = level.Data[i * level.Rows + j];
                }
            }
        }

        private void CreateBackcround()
        {
            float width = (cellSize + cellGap) * level.Columns - cellGap + levelGap;
            float height = (cellSize + cellGap) * level.Rows - cellGap + levelGap;
            backgroundSprite.size = new Vector2(width, height);

            Vector2 backgroundPosition = new Vector2(
                width / backgroundPositionController - cellSize / backgroundPositionController - levelGap / backgroundPositionController,
                height / backgroundPositionController - cellSize / backgroundPositionController - levelGap / backgroundPositionController);
            backgroundSprite.transform.position = backgroundPosition;
        }

        private void CreateCells(int[,] levelGrid)
        {
            Cell[,] cellGrid = new Cell[level.Rows, level.Columns];

            Vector2 startPosition = Vector2.zero;
            Vector2 rightOffset = Vector2.right * (cellSize + cellGap);
            Vector2 topOffset = Vector2.up * (cellSize + cellGap);

            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    Vector2 spawnPosition = startPosition + j * rightOffset + i * topOffset;

                    Cell tempCell = Instantiate(cellPrefab, spawnPosition, Quaternion.identity);
                    tempCell.InitializeCellData(i, j, levelGrid[i, j]);
                    tempCell.gameObject.name = "Cell) " + i.ToString() + " - " + j.ToString();

                    cellGrid[i, j] = tempCell;

                    if (tempCell.Number == 0)
                    {
                        Destroy(tempCell.gameObject);
                        cellGrid[i, j] = null;
                    }
                }
            }

            GameManager.Instance.Initialize(level, cellGrid, cellGap + cellSize);
            // OnLevelSpawned?.Invoke();
        }


        private const int RIGHT = 0;
        private const int TOP = 1;
        private const int LEFT = 2;
        private const int BOTTOM = 3;

        public void InitializeCellData(Cell cell, int row, int column, int number)
        {
            cell.Number = number;
            cell.rowCoordinate = row;
            cell.columnCoordinate = column;

            Dictionary<int, int> edgeCounts = new()
            {
                { RIGHT, 0 },
                { LEFT, 0 },
                { TOP, 0 },
                { BOTTOM, 0 }
            };

            Dictionary<int, Cell> connectedCell = new()
            {
                { LEFT, null },
                { TOP, null },
                { BOTTOM, null },
                { RIGHT, null }
            };
        }
    }
}