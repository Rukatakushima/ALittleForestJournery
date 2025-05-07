using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        [SerializeField] private BackgroundCell backgroundCellPrefab;
        [SerializeField] private Color emptyColor, blockedColor, correctColor, incorrectColor;
        [SerializeField] private Blocks blocksPrefab;
        [SerializeField] private SpriteRenderer blockSprite;
        [SerializeField] private float blocksSpawnSize, bgCellPositionRate;
        [SerializeField] private List<Color> blocksColors;
        
        private BackgroundCell[,] _backgroundCellGrid;

        public override void SpawnLevel()
        {
            SpawnGrid();
            SpawnBlocks();

            GameManager.Instance.Initialize(level, _backgroundCellGrid, blocksSpawnSize);
            OnLevelSpawned?.Invoke();
        }

        private void SpawnGrid()
        {
            _backgroundCellGrid = new BackgroundCell[level.gridRows, level.gridColumns];
            for (int i = 0; i < level.gridRows; i++)
            {
                for (int j = 0; j < level.gridColumns; j++)
                {
                    BackgroundCell backgroundCell = Instantiate(backgroundCellPrefab);
                    backgroundCell.name = i + " " + j;
                    backgroundCell.transform.position = new Vector2(j + bgCellPositionRate, i + bgCellPositionRate);
                    backgroundCell.Init(level.data[i * level.gridColumns + j], emptyColor, blockedColor, correctColor, incorrectColor);
                    _backgroundCellGrid[i, j] = backgroundCell;
                }
            }

        }

        private void SpawnBlocks()
        {
            Vector2 startPos = new Vector2(
                (level.gridColumns - level.blockColumns * blocksSpawnSize) * bgCellPositionRate,
                -level.blockRows * blocksSpawnSize - blocksSpawnSize);

            for (int i = 0; i < level.blocks.Count; i++)
            {
                Blocks mainBlock = Instantiate(blocksPrefab);
                mainBlock.name = i + ") Block";

                Vector2Int blockPos = level.blocks[i].startPosition;
                Vector3 blockSpawnPosition = startPos + new Vector2(blockPos.y, blockPos.x) * blocksSpawnSize;
                mainBlock.transform.position = blockSpawnPosition;
                mainBlock.SetStartParameters(blockSpawnPosition, level.blocks[i].blockPositions, blocksSpawnSize);

                CreateChildrenBlocks(mainBlock, i);
                mainBlock.ElevateSprites(true);
            }
        }

        private void CreateChildrenBlocks(Blocks mainBlock, int blockNumber)
        {
            foreach (var position in mainBlock.blockPositions)
            {
                SpriteRenderer spawnedBlock = Instantiate(blockSprite, mainBlock.transform, true);
                spawnedBlock.name = "Child Block: " + blockNumber.ToString();
                spawnedBlock.color = blocksColors[blockNumber + 1];
                spawnedBlock.transform.localPosition = new Vector2(position.y, position.x);
                spawnedBlock.transform.localScale = Vector2.one * blocksSpawnSize;
                mainBlock.blockSpriteRenderers.Add(spawnedBlock);
            }
        }
    }
}