using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    public class GameManager : BaseGameManager<LevelSpawner, CameraController, WinConditionChecker, LevelData>
    {
        public static GameManager Instance;

        [SerializeField] private float blockSpawnSize, blockHighLightSize, blockPutSize;
        private BGCell[,] BGCellGrid;
        private Block currentBlock;
        private Vector2 curPos, prevPos;
        private List<Block> gridBlocks;

        protected override void Awake()
        {
            Instance = this;

            gridBlocks = new List<Block>();
            base.Awake();
        }

        protected override void SetupManagers()
        {
            cameraController.SetupCamera(level, blockSpawnSize, level.BlockRows, level.BlockColumns);

            levelSpawner.Initialize(blockSpawnSize, level, cameraController.bgCellPositionRate);
            levelSpawner.SpawnLevel();

            winConditionChecker.Initialize(BGCellGrid, level.Rows, level.Columns);
        }

        public void SetBGCellGrid(BGCell[,] BGCellGrid)
        {
            this.BGCellGrid = BGCellGrid;
        }

        private void ResetHighLight()
        {
            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    if (!BGCellGrid[i, j].isBlocked)
                    {
                        BGCellGrid[i, j].ResetHighLight();
                    }
                }
            }
        }

        private void UpdateFilled()
        {
            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    if (!BGCellGrid[i, j].isBlocked)
                    {
                        BGCellGrid[i, j].isFilled = false;
                    }
                }
            }

            foreach (var block in gridBlocks)
            {
                foreach (var pos in block.BlockPositions())
                {
                    if (IsValidPos(pos))
                    {
                        BGCellGrid[pos.x, pos.y].isFilled = true;
                    }
                }
            }
        }

        private void UpdateHighLight()
        {
            bool isCorrect = IsCorrectMove();
            foreach (var pos in currentBlock.BlockPositions())
            {
                if (IsValidPos(pos))
                {
                    BGCellGrid[pos.x, pos.y].UpdateHighlight(isCorrect);
                }
            }
        }

        private bool IsCorrectMove()
        {
            foreach (var pos in currentBlock.BlockPositions())
            {
                if (!IsValidPos(pos) || BGCellGrid[pos.x, pos.y].isFilled)
                {
                    return false;
                }
            }
            return true;
        }


        private bool IsValidPos(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < level.Rows && pos.y < level.Columns;
        }

        protected override void HandleMouseDown(Vector2 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (!hit) return;

            currentBlock = hit.collider.transform.parent.GetComponent<Block>();
            if (currentBlock == null) return;

            curPos = mousePosition;
            prevPos = mousePosition;

            currentBlock.ElevateSprites();
            currentBlock.transform.localScale = Vector3.one * blockHighLightSize;

            if (gridBlocks.Contains(currentBlock))
            {
                gridBlocks.Remove(currentBlock);
            }

            UpdateFilled();
            ResetHighLight();
            UpdateHighLight();
        }

        protected override void HandleMouseDrag(Vector2 mousePosition)
        {
            if (currentBlock == null) return;

            curPos = mousePosition;
            currentBlock.UpdatePos(curPos - prevPos);
            prevPos = curPos;

            ResetHighLight();
            UpdateHighLight();
        }

        protected override void HandleMouseUp()
        {
            if (currentBlock == null) return;

            currentBlock.ElevateSprites(true);

            if (IsCorrectMove())
            {
                currentBlock.UpdateCorrectMove();
                currentBlock.transform.localScale = Vector3.one * blockPutSize;
                gridBlocks.Add(currentBlock);
            }
            else if (Input.mousePosition.y < 0)
            {
                currentBlock.UpdateStartMove();
                currentBlock.transform.localScale = Vector3.one * blockSpawnSize;
            }
            else
            {
                currentBlock.UpdateIncorrectMove();
                if (currentBlock.curPos.y > 0)
                {
                    gridBlocks.Add(currentBlock);
                    currentBlock.transform.localScale = Vector3.one * blockPutSize;
                }
                else
                {
                    currentBlock.transform.localScale = Vector3.one * blockSpawnSize;
                }
            }

            currentBlock = null;
            ResetHighLight();
            UpdateFilled();

            CheckWinCondition();
        }
    }
}