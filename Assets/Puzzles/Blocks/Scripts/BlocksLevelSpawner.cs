using System.Collections.Generic;
using ObjectsPool;
using UnityEngine;

namespace Blocks
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        public BackgroundCell[,] BackgroundCellGrid;
        [SerializeField] private BackgroundCell backgroundCellPrefab;
        [SerializeField] private Blocks blockPrefab;
        // [SerializeField] private float blocksSpawnSize;
        [SerializeField] private List<Color> blocksColors;
        [SerializeField] private float blocksSpawnSize;
        [SerializeField] private float bgCellPositionRate;

        public BackgroundCell PreloadBGCell() => Instantiate(backgroundCellPrefab);
        public void GetAction(BackgroundCell bGCellPrefab) => bGCellPrefab.gameObject.SetActive(true);
        public void ReturnAction(BackgroundCell bGCellPrefab) => bGCellPrefab.gameObject.SetActive(false);
        public PoolBase<BackgroundCell> backgroundCellPrefabObjectPool;

        public Blocks PreloadBlock() => Instantiate(blockPrefab);
        public void GetAction(Blocks blockPrefab) => blockPrefab.gameObject.SetActive(true);
        public void ReturnAction(Blocks blockPrefab) => blockPrefab.gameObject.SetActive(false);
        public PoolBase<Blocks> blockPrefabObjectPool;

        [SerializeField] private SpriteRenderer blockSpritePrefab;
        public SpriteRenderer PreloadSpriteRenderer() => Instantiate(blockSpritePrefab);
        public void GetAction(SpriteRenderer blockSpritePrefab) => blockSpritePrefab.gameObject.SetActive(true);
        public void ReturnAction(SpriteRenderer blockSpritePrefab) => blockSpritePrefab.gameObject.SetActive(false);
        public PoolBase<SpriteRenderer> blockSpritePrefabObjectPool;

        public override void SpawnLevel()
        {
            CreateObjectPools();
            SpawnGrid();
            SpawnBlocks();
            OnLevelSpawned?.Invoke();
        }

        private void CreateObjectPools()
        {
            backgroundCellPrefabObjectPool = new PoolBase<BackgroundCell>(PreloadBGCell, GetAction, ReturnAction, level.GridRows * level.GridColumns);
            blockPrefabObjectPool = new PoolBase<Blocks>(PreloadBlock, GetAction, ReturnAction, level.Blocks.Count);
            blockSpritePrefabObjectPool = new PoolBase<SpriteRenderer>(PreloadSpriteRenderer, GetAction, ReturnAction, level.CalculateTotalBlockPositions);
        }

        private void SpawnGrid()
        {
            BackgroundCellGrid = new BackgroundCell[level.GridRows, level.GridColumns];
            for (int i = 0; i < level.GridRows; i++)
            {
                for (int j = 0; j < level.GridColumns; j++)
                {
                    BackgroundCell backgroundCell = backgroundCellPrefabObjectPool.GetFromPool();
                    backgroundCell.name = i.ToString() + " " + j.ToString();
                    backgroundCell.transform.position = new Vector2(j + bgCellPositionRate, i + bgCellPositionRate);
                    backgroundCell.Init(level.Data[i * level.GridColumns + j]); // i = стока, размер строки = кол-во столбцов
                    BackgroundCellGrid[i, j] = backgroundCell;
                }
            }

        }

        private void SpawnBlocks()
        {
            Vector2 startPos = new Vector2(
                (level.GridColumns - level.BlockColumns * blocksSpawnSize) * bgCellPositionRate,
                -level.BlockRows * blocksSpawnSize - blocksSpawnSize);

            for (int i = 0; i < level.Blocks.Count; i++)
            {
                Blocks mainBlock = blockPrefabObjectPool.GetFromPool();
                mainBlock.name = i.ToString() + ") Block";

                Vector2Int blockPos = level.Blocks[i].StartPosition;
                Vector3 blockSpawnPosition = startPos + new Vector2(blockPos.y, blockPos.x) * blocksSpawnSize;
                mainBlock.transform.position = blockSpawnPosition;
                mainBlock.SetStartParametrs(blockSpawnPosition, level.Blocks[i].BlockPositions, blocksSpawnSize);

                CreateChildrenBlocks(mainBlock, level.Blocks[i].Id);
                mainBlock.ElevateSprites(true);
            }

            GameManager.Instance.Initialize(level, BackgroundCellGrid, blocksSpawnSize);
        }

        private void CreateChildrenBlocks(Blocks mainBlock, int blockNumber)
        {
            foreach (var position in mainBlock.blockPositions)
            {
                SpriteRenderer spawnedBlock = blockSpritePrefabObjectPool.GetFromPool();
                spawnedBlock.name = "Child Block: " + blockNumber.ToString();
                spawnedBlock.transform.SetParent(mainBlock.transform);
                spawnedBlock.color = blocksColors[blockNumber + 1];
                spawnedBlock.transform.localPosition = new Vector2(position.y, position.x);
                spawnedBlock.transform.localScale = Vector2.one * blocksSpawnSize;
                mainBlock.blockSpriteRenderers.Add(spawnedBlock);
            }
        }
    }
}