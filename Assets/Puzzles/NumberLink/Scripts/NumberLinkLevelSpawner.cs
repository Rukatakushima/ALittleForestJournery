using UnityEngine;

namespace NumberLink
{
    public class LevelSpawner : BaseLevelSpawner
    {
        [SerializeField] private Cell cellPrefab;
        [SerializeField] private SpriteRenderer bgSprite;

        [SerializeField] private LevelData levelData;
        [SerializeField] private float cellGap;
        [SerializeField] private float cellSize;
        [SerializeField] private float levelGap;
        // private int[,] levelGrid;
        // private Cell[,] cellGrid;

        public void Initialize()
        {

        }

        public override void SpawnLevel()
        {

            int[,] levelGrid = new int[levelData.Rows, levelData.Columns];
            Cell[,] cellGrid = new Cell[levelData.Rows, levelData.Columns];
            
            for (int i = 0; i < levelData.Rows; i++)
            {
                for (int j = 0; j < levelData.Columns; j++)
                {
                    levelGrid[i, j] = levelData.Data[i * levelData.Rows + j];
                }
            }

            float width = (cellSize + cellGap) * levelData.Columns - cellGap + levelGap;
            float height = (cellSize + cellGap) * levelData.Rows - cellGap + levelGap;
            bgSprite.size = new Vector2(width, height);
            Vector3 bgPos = new Vector3(
                width / 2f - cellSize / 2f - levelGap / 2f,
                height / 2f - cellSize / 2f - levelGap / 2f,
                0
                );
            bgSprite.transform.position = bgPos;

            Vector3 startPos = Vector3.zero;
            Vector3 rightOffset = Vector3.right * (cellSize + cellGap);
            Vector3 topOffset = Vector3.up * (cellSize + cellGap);

            for (int i = 0; i < levelData.Rows; i++)
            {
                for (int j = 0; j < levelData.Columns; j++)
                {
                    Vector3 spawnPos = startPos + j * rightOffset + i * topOffset;
                    Cell tempCell = Instantiate(cellPrefab, spawnPos, Quaternion.identity);
                    tempCell.Init(i, j, levelGrid[i, j]);
                    cellGrid[i, j] = tempCell;
                    if (tempCell.Number == 0)
                    {
                        Destroy(tempCell.gameObject);
                        cellGrid[i, j] = null;
                    }
                }
            }

            for (int i = 0; i < levelData.Rows; i++)
            {
                for (int j = 0; j < levelData.Columns; j++)
                {
                    if (cellGrid[i, j] != null)
                    {
                        cellGrid[i, j].Init();
                    }
                }
            }
            GameManager.Instance.SetGameManagerParameters(cellGrid);
        }
    }
}