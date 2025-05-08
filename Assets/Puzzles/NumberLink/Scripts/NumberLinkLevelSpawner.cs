using UnityEngine;

namespace NumberLink
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        [SerializeField] private SpriteRenderer backgroundSprite;
        [SerializeField] private Cell cellPrefab;
        [SerializeField] private float cellGap, cellSize;

        public override void SpawnLevel()
        {
            CreateLevelGrid();
            CreateBackground();
            OnLevelSpawned?.Invoke();
        }

        private void CreateLevelGrid()
        {
            int[,] levelGrid = new int[level.rows, level.columns];
            for (int i = 0; i < level.rows; i++)
            {
                for (int j = 0; j < level.columns; j++)
                {
                    levelGrid[i, j] = level.data[i * level.rows + j];
                }
            }
            CreateCells(levelGrid);
        }

        private void CreateBackground()
        {
            float width = (cellSize + cellGap) * level.columns - cellGap;
            float height = (cellSize + cellGap) * level.rows - cellGap;
            backgroundSprite.size = new Vector2(width, height);

            Vector2 backgroundPosition = new Vector2( width - cellSize, height - cellSize);
            backgroundSprite.transform.position = backgroundPosition;
        }

        private void CreateCells(int[,] levelGrid)
        {
            Cell[,] cellGrid = new Cell[level.rows, level.columns];

            Vector2 startPosition = Vector2.zero;
            Vector2 rightOffset = Vector2.right * (cellSize + cellGap);
            Vector2 topOffset = Vector2.up * (cellSize + cellGap);

            for (int i = 0; i < level.rows; i++)
            {
                for (int j = 0; j < level.columns; j++)
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
        }
    }
}