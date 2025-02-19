using ObjectsPool;
using UnityEngine;

namespace Blocks
{
    public class LevelSpawner : BaseLevelSpawner
    {
        private LevelData level;

        public BGCell[,] BGCellGrid;
        [SerializeField] private BGCell bGCellPrefab;
        [SerializeField] private Block blockPrefab;
        private float blockSpawnSize;
        private float bgCellPositionRate;

        public BGCell PreloadBGCell() => Instantiate(bGCellPrefab);
        public void GetAction(BGCell bGCellPrefab) => bGCellPrefab.gameObject.SetActive(true);
        public void ReturnAction(BGCell bGCellPrefab) => bGCellPrefab.gameObject.SetActive(false);
        public PoolBase<BGCell> bGCellPrefabObjectPool;

        public Block PreloadBlock() => Instantiate(blockPrefab);
        public void GetAction(Block blockPrefab) => blockPrefab.gameObject.SetActive(true);
        public void ReturnAction(Block blockPrefab) => blockPrefab.gameObject.SetActive(false);
        public PoolBase<Block> blockPrefabObjectPool;

        public void Initialize(float blockSpawnSize, LevelData level, float bgCellPositionRate)
        {
            this.blockSpawnSize = blockSpawnSize;
            this.level = level;
            this.bgCellPositionRate = bgCellPositionRate;
        }

        public override void SpawnLevel()
        {
            CreateObjectPools();
            SpawnGrid();
            SpawnBlocks();
        }

        private void CreateObjectPools()
        {
            bGCellPrefabObjectPool = new PoolBase<BGCell>(PreloadBGCell, GetAction, ReturnAction, level.Rows * level.Columns);
            blockPrefabObjectPool = new PoolBase<Block>(PreloadBlock, GetAction, ReturnAction, level.Blocks.Count);
        }

        private void SpawnGrid()
        {
            BGCellGrid = new BGCell[level.Rows, level.Columns];
            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    BGCell bgCell = bGCellPrefabObjectPool.GetFromPool();
                    bgCell.name = i.ToString() + " " + j.ToString();
                    bgCell.transform.position = new Vector2(j + bgCellPositionRate, i + bgCellPositionRate);
                    bgCell.Init(level.Data[i * level.Columns + j]); // i = стока, размер строки = кол-во столбцов
                    BGCellGrid[i, j] = bgCell;
                }
            }

            GameManager.Instance.SetBGCellGrid(BGCellGrid);
        }

        private void SpawnBlocks()
        {
            Vector2 startPos = new Vector2(
                (level.Columns - level.BlockColumns * blockSpawnSize) * bgCellPositionRate,
                -level.BlockRows * blockSpawnSize - blockSpawnSize);

            for (int i = 0; i < level.Blocks.Count; i++)
            {
                Block block = blockPrefabObjectPool.GetFromPool();
                block.name = i.ToString() + ") Block";

                Vector2Int blockPos = level.Blocks[i].StartPos;
                Vector3 blockSpawnPos = startPos + new Vector2(blockPos.y, blockPos.x) * blockSpawnSize;
                block.transform.position = blockSpawnPos;
                block.Initialize(level.Blocks[i].BlockPositions, blockSpawnPos, level.Blocks[i].Id);
            }
        }
    }
}