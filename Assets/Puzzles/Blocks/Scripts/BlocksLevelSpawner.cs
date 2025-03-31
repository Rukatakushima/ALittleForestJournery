using System.Collections.Generic;
// using ObjectsPool;
using UnityEngine;

namespace Blocks
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        [SerializeField] private BackgroundCell backgroundCellPrefab;
        [SerializeField] private Blocks blockPrefab;
        [SerializeField] private SpriteRenderer blockSpritePrefab;
        [SerializeField] private float blocksSpawnSize, bgCellPositionRate;
        [SerializeField] private List<Color> blocksColors;
        private BackgroundCell[,] BackgroundCellGrid;

        // private BackgroundCell PreloadBGCell() => Instantiate(backgroundCellPrefab);
        // private void GetAction(BackgroundCell bGCellPrefab) => bGCellPrefab.gameObject.SetActive(true);
        // private void ReturnAction(BackgroundCell bGCellPrefab) => bGCellPrefab.gameObject.SetActive(false);
        // private PoolBase<BackgroundCell> backgroundCellPrefabObjectPool;

        // private Blocks PreloadBlock() => Instantiate(blockPrefab);
        // private void GetAction(Blocks blockPrefab) => blockPrefab.gameObject.SetActive(true);
        // private void ReturnAction(Blocks blockPrefab) => blockPrefab.gameObject.SetActive(false);
        // private PoolBase<Blocks> blockPrefabObjectPool;

        // private SpriteRenderer PreloadSpriteRenderer() => Instantiate(blockSpritePrefab);
        // private void GetAction(SpriteRenderer blockSpritePrefab) => blockSpritePrefab.gameObject.SetActive(true);
        // private void ReturnAction(SpriteRenderer blockSpritePrefab) => blockSpritePrefab.gameObject.SetActive(false);
        // private PoolBase<SpriteRenderer> blockSpritePrefabObjectPool;

        public override void SpawnLevel()
        {
            // CreateObjectPools();
            SpawnGrid();
            SpawnBlocks();
            OnLevelSpawned?.Invoke();
            // GameManager.Instance.cameraController.SetupCamera(level, blocksSpawnSize, level.BlockRows, level.BlockColumns);
            // GameManager.Instance.winConditionChecker.Initialize(BGCellGrid, level.Rows, level.Columns);
        }

        // private void CreateObjectPools()
        // {
        //     backgroundCellPrefabObjectPool = new PoolBase<BackgroundCell>(PreloadBGCell, GetAction, ReturnAction, level.Rows * level.Columns);
        //     blockPrefabObjectPool = new PoolBase<Blocks>(PreloadBlock, GetAction, ReturnAction, level.Blocks.Count);
        //     blockSpritePrefabObjectPool = new PoolBase<SpriteRenderer>(PreloadSpriteRenderer, GetAction, ReturnAction, level.CalculateTotalBlockPositions());
        // }

        private void SpawnGrid()
        {
            BackgroundCellGrid = new BackgroundCell[level.GridRows, level.GridColumns];
            for (int i = 0; i < level.GridRows; i++)
            {
                for (int j = 0; j < level.GridColumns; j++)
                {
                    BackgroundCell backgroundCell = Instantiate(backgroundCellPrefab);//backgroundCellPrefabObjectPool.GetFromPool();
                    backgroundCell.name = i.ToString() + " " + j.ToString();
                    backgroundCell.transform.position = new Vector2(j + bgCellPositionRate, i + bgCellPositionRate);
                    backgroundCell.Init(level.Data[i * level.GridColumns + j]); // i = стока, размер строки = кол-во столбцов
                    BackgroundCellGrid[i, j] = backgroundCell;
                }
            }

            GameManager.Instance.Initialize(level, BackgroundCellGrid, blocksSpawnSize);
            // GameManager.Instance.OnGameAwaked?.Invoke();
        }

        private void SpawnBlocks()
        {

            Vector2 startPos = new Vector2(
                (level.GridColumns - level.BlockColumns * blocksSpawnSize) * bgCellPositionRate,
                -level.BlockRows * blocksSpawnSize - blocksSpawnSize);

            for (int i = 0; i < level.Blocks.Count; i++)
            {
                Blocks mainBlock = Instantiate(blockPrefab);//blockPrefabObjectPool.GetFromPool();
                mainBlock.name = i.ToString() + ") Block";

                Vector2Int blockPos = level.Blocks[i].StartPosition;
                Vector3 blockSpawnPosition = startPos + new Vector2(blockPos.y, blockPos.x) * blocksSpawnSize;
                mainBlock.transform.position = blockSpawnPosition;
                mainBlock.SetStartParametrs(blockSpawnPosition, level.Blocks[i].BlockPositions, blocksSpawnSize);

                CreateChildrenBlocks(mainBlock, level.Blocks[i].Id);
                mainBlock.ElevateSprites(true);
            }
        }

        private void CreateChildrenBlocks(Blocks mainBlock, int blockNumber)
        {
            foreach (var position in mainBlock.blockPositions)
            {
                SpriteRenderer spawnedBlock = Instantiate(blockSpritePrefab);//blockSpritePrefabObjectPool.GetFromPool();
                spawnedBlock.name = "Child Block: " + blockNumber.ToString();
                spawnedBlock.transform.SetParent(mainBlock.transform);
                spawnedBlock.color = blocksColors[blockNumber];
                spawnedBlock.transform.localPosition = new Vector2(position.y, position.x);
                spawnedBlock.transform.localScale = Vector2.one * blocksSpawnSize;
                mainBlock.blockSpriteRenderers.Add(spawnedBlock);
            }
        }
    }
}