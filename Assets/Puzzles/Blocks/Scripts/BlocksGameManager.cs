using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectsPool;

namespace Blocks
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private Level level;
        [SerializeField] private BGCell bGCellPrefab;
        [SerializeField] private Block blockPrefab;
        [SerializeField] private float blockSpawnSize, blockHighLightSize, blockPutSize;

        private BGCell[,] BGCellGrid;
        [SerializeField] private bool hasGameFinished;
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

        public float bgCellPositionRate = 0.5f;
        public float cameraSizeController = 0.65f;
        public bool spawnInEditor;
        private bool isAwaken = false;

        private void Awake()
        {
            Instance = this;
            hasGameFinished = false;
            cameraSizeController = blockPrefab.transform.localScale.x / 2;
            bgCellPositionRate = blockPrefab.transform.localScale.x / 2;
            gridBlocks = new List<Block>();
            bGCellPrefabObjectPool = new PoolBase<BGCell>(PreloadBGCell, GetAction, ReturnAction, level.Rows * level.Columns);
            blockPrefabObjectPool = new PoolBase<Block>(PreloadBlock, GetAction, ReturnAction, level.Blocks.Count);//level.Blocks.Count
            SpawnGrid();
            SpawnBlocks();
        }

        private void OnValidate()
        {
            if (spawnInEditor && isAwaken == false)
            {
                Awake();
                isAwaken = true;
            }
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
        }

        private void SpawnBlocks()
        {
            Vector2 startPos = Vector2.zero;
            startPos.x = 0.25f + (level.Columns - level.BlockColumns * blockSpawnSize) * bgCellPositionRate;
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

            SetCamera(startPos);
        }

        private void SetCamera(Vector2 startPos)
        {
            float maxColumns = Mathf.Max(level.Columns, level.BlockColumns * blockSpawnSize);
            float maxRows = level.Rows + 2f + level.BlockRows * blockSpawnSize;

            Camera.main.orthographicSize = Mathf.Max(maxColumns, maxRows) * cameraSizeController;
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = level.Columns * bgCellPositionRate;
            camPos.y = (level.Rows + bgCellPositionRate + startPos.y) * bgCellPositionRate;
            Camera.main.transform.position = camPos;
        }

        private void Update()
        {
            if (hasGameFinished) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            if (Input.GetMouseButtonDown(0))
            {
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
            else if (Input.GetMouseButton(0) && currentBlock != null)
            {
                curPos = mousePos;
                currentBlock.UpdatePos(curPos - prevPos);
                prevPos = curPos;
                ResetHighLight();
                UpdateHighLight();
            }
            else if (Input.GetMouseButtonUp(0) && currentBlock != null)
            {
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
                CheckWin();
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

        private void CheckWin()
        {
            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    if (!BGCellGrid[i, j].isFilled)
                    {
                        return;
                    }
                }
            }

            hasGameFinished = true;
            StartCoroutine(GameFinished());
        }

        private IEnumerator GameFinished()
        {
            yield return new WaitForSeconds(1f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}