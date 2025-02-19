using System.Collections.Generic;
using UnityEngine;
using ObjectsPool;

namespace Blocks
{
    public class GameManager : BaseGameManager<CameraController, WinConditionChecker, LevelData>
    {
        public static GameManager Instance;

        [SerializeField] private BGCell bGCellPrefab;
        [SerializeField] private Block blockPrefab;
        [SerializeField] private float blockSpawnSize, blockHighLightSize, blockPutSize;

        private BGCell[,] BGCellGrid;
        private Block currentBlock;
        private Vector2 curPos, prevPos;
        private List<Block> gridBlocks;

        public BGCell PreloadBGCell() => Instantiate(bGCellPrefab);
        public void GetAction(BGCell bGCellPrefab) => bGCellPrefab.gameObject.SetActive(true);
        public void ReturnAction(BGCell bGCellPrefab) => bGCellPrefab.gameObject.SetActive(false);
        public PoolBase<BGCell> bGCellPrefabObjectPool;

        public Block PreloadBlock() => Instantiate(blockPrefab);
        public void GetAction(Block blockPrefab) => blockPrefab.gameObject.SetActive(true);
        public void ReturnAction(Block blockPrefab) => blockPrefab.gameObject.SetActive(false);
        public PoolBase<Block> blockPrefabObjectPool;

        protected override void Awake()
        {
            Instance = this;

            gridBlocks = new List<Block>();
            bGCellPrefabObjectPool = new PoolBase<BGCell>(PreloadBGCell, GetAction, ReturnAction, level.Rows * level.Columns);
            blockPrefabObjectPool = new PoolBase<Block>(PreloadBlock, GetAction, ReturnAction, level.Blocks.Count);//level.Blocks.Count

            base.Awake();
        }

        protected override void SpawnLevel()
        {
            SpawnGrid();
            SpawnBlocks();
        }

        protected override void SetupManagers() //override
        {
            cameraController.SetupCamera(level, blockSpawnSize, level.BlockRows, level.BlockColumns);

            winConditionChecker.Initialize(BGCellGrid, level.Rows, level.Columns);
            winConditionChecker.OnWin.AddListener(sceneLoader.ChangeScene);
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
                    bgCell.transform.position = new Vector2(j + cameraController.bgCellPositionRate, i + cameraController.bgCellPositionRate);
                    bgCell.Init(level.Data[i * level.Columns + j]); // i = стока, размер строки = кол-во столбцов
                    BGCellGrid[i, j] = bgCell;
                }
            }
        }

        private void SpawnBlocks()
        {
            Vector2 startPos = Vector2.zero;
            startPos.x = 0.25f + (level.Columns - level.BlockColumns * blockSpawnSize) * cameraController.bgCellPositionRate;
            startPos.y = -level.BlockRows * blockSpawnSize + 0.25f - 1f;

            for (int i = 0; i < level.Blocks.Count; i++)
            {
                Block block = blockPrefabObjectPool.GetFromPool();
                block.name = i.ToString() + ") Block";

                Vector2Int blockPos = level.Blocks[i].StartPos;
                Vector3 blockSpawnPos = startPos + new Vector2(blockPos.y, blockPos.x) * blockSpawnSize;
                block.transform.position = blockSpawnPos;
                block.Init(level.Blocks[i].BlockPositions, blockSpawnPos, level.Blocks[i].Id);
            }
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
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (!hit) return;
            currentBlock = hit.collider.transform.parent.GetComponent<Block>();
            if (currentBlock == null) return;
            curPos = mousePos2D;
            prevPos = mousePos2D;
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
            if (currentBlock != null)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                curPos = mousePos;
                currentBlock.UpdatePos(curPos - prevPos);
                prevPos = curPos;
                ResetHighLight();
                UpdateHighLight();
            }
        }

        protected override void HandleMouseUp()
        {
            if (currentBlock != null)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                currentBlock.ElevateSprites(true);

                if (IsCorrectMove())
                {
                    currentBlock.UpdateCorrectMove();
                    currentBlock.transform.localScale = Vector3.one * blockPutSize;
                    gridBlocks.Add(currentBlock);
                }
                else if (mousePos2D.y < 0)
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

                winConditionChecker.CheckWinCondition();
            }
        }
    }
}