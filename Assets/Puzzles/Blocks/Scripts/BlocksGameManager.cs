using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;

        public LevelData level { get; private set; }
        public float defaultBlockSize { get; private set; }
        [SerializeField] public float backgroundCellPositionRate { get; private set; } = 0.5f;
        public BackgroundCell[,] BGCellGrid { get; private set; }
        private Blocks currentBlock;
        private Vector2 curPos, prevPos;
        private List<Blocks> gridBlocks;
        [SerializeField] private float blockPutSize = 1f;
        [SerializeField] private float blockHighLightSize = 1f;

        protected override void Awake()
        {
            Instance = this;
            gridBlocks = new List<Blocks>();
            base.Awake();
        }

        public void Initialize(LevelData level, BackgroundCell[,] BGCellGrid, float blockSpawnSize)
        {
            this.level = level;
            this.BGCellGrid = BGCellGrid;
            defaultBlockSize = blockSpawnSize;
        }

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (!hit) return;

            currentBlock = hit.collider.transform.parent.GetComponent<Blocks>();
            if (currentBlock == null) return;

            curPos = mousePosition;
            prevPos = mousePosition;

            currentBlock.ElevateSprites();
            ChangeCurrentBlockScale(blockHighLightSize);

            if (gridBlocks.Contains(currentBlock))
                gridBlocks.Remove(currentBlock);

            UpdateFilled();
            ResetHighLight();
            UpdateHighLight();
        }

        private void UpdateFilled()
        {
            for (int i = 0; i < level.GridRows; i++)
            {
                for (int j = 0; j < level.GridColumns; j++)
                {
                    if (!BGCellGrid[i, j].isBlocked)
                        BGCellGrid[i, j].isFilled = false;
                }
            }

            foreach (var block in gridBlocks)
            {
                foreach (var pos in block.BlockPositions)
                {
                    if (IsValidPos(pos))
                        BGCellGrid[pos.x, pos.y].isFilled = true;
                }
            }
        }

        private bool IsValidPos(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < level.GridRows && pos.y < level.GridColumns;

        protected override void HandleInputUpdate(Vector2 mousePosition)
        {
            if (currentBlock == null) return;

            curPos = mousePosition;
            currentBlock.UpdatePos(curPos - prevPos);
            prevPos = curPos;

            ResetHighLight();
            UpdateHighLight();
        }

        private void ResetHighLight()
        {
            for (int i = 0; i < level.GridRows; i++)
            {
                for (int j = 0; j < level.GridColumns; j++)
                {
                    if (!BGCellGrid[i, j].isBlocked)
                        BGCellGrid[i, j].ResetHighLight();
                }
            }
        }

        private void UpdateHighLight()
        {
            bool isCorrect = IsCorrectMove();
            foreach (var pos in currentBlock.BlockPositions)
            {
                if (IsValidPos(pos))
                    BGCellGrid[pos.x, pos.y].UpdateHighlight(isCorrect);
            }
        }

        protected override void HandleInputEnd()
        {
            if (currentBlock == null) return;

            currentBlock.ElevateSprites(true);

            if (IsCorrectMove())
            {
                currentBlock.UpdateCorrectMove();
                ChangeCurrentBlockScale(blockPutSize);
                gridBlocks.Add(currentBlock);
            }
            else if (Input.mousePosition.y < 0)
            {
                currentBlock.UpdateStartMove();
                ChangeCurrentBlockScale(defaultBlockSize);
            }
            else
            {
                currentBlock.UpdateIncorrectMove();
                if (currentBlock.curPos.y > 0)
                {
                    gridBlocks.Add(currentBlock);
                    ChangeCurrentBlockScale(blockPutSize);
                }
                else
                    ChangeCurrentBlockScale(defaultBlockSize);
            }

            currentBlock = null;
            ResetHighLight();
            UpdateFilled();

            CheckWinCondition();
        }

        private bool IsCorrectMove()
        {
            foreach (var pos in currentBlock.BlockPositions)
            {
                if (!IsValidPos(pos) || BGCellGrid[pos.x, pos.y].isFilled)
                {
                    return false;
                }
            }
            return true;
        }

        private void ChangeCurrentBlockScale(float size) => currentBlock.transform.localScale = Vector3.one * size;

        public override void CheckWinCondition()
        {
            for (int i = 0; i < level.GridRows; i++)
            {
                for (int j = 0; j < level.GridColumns; j++)
                {
                    if (!BGCellGrid[i, j].isFilled)
                    {
                        return;
                    }
                }
            }
            OnWin?.Invoke();
        }
    }
}