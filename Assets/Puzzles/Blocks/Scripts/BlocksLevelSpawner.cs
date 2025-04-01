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
        [SerializeField] private List<Color> blocksColors;
        [SerializeField] private float blocksSpawnSize, bgCellPositionRate;
        [SerializeField] private SpriteRenderer blockSpritePrefab;

        public override void SpawnLevel()
        {
            SpawnGrid();
            SpawnBlocks();

            GameManager.Instance.Initialize(level, BackgroundCellGrid, blocksSpawnSize);
            OnLevelSpawned?.Invoke();
        }

        private void SpawnGrid()
        {
            BackgroundCellGrid = new BackgroundCell[level.GridRows, level.GridColumns];
            for (int i = 0; i < level.GridRows; i++)
            {
                for (int j = 0; j < level.GridColumns; j++)
                {
                    BackgroundCell backgroundCell = Instantiate(backgroundCellPrefab);
                    backgroundCell.name = i.ToString() + " " + j.ToString();
                    backgroundCell.transform.position = new Vector2(j + bgCellPositionRate, i + bgCellPositionRate);
                    backgroundCell.Init(level.Data[i * level.GridColumns + j]);
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
                Blocks mainBlock = Instantiate(blockPrefab);
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
                SpriteRenderer spawnedBlock = Instantiate(blockSpritePrefab);
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