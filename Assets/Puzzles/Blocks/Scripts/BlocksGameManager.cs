using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blocks
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;

        public LevelData Level { get; private set; }
        public float DefaultBlockSize { get; private set; }
        [SerializeField] public float backgroundCellPositionRate = 0.5f;
        [SerializeField] private float blockPutSize = 1f;
        [SerializeField] private float blockHighLightSize = 1f;
        
        private BackgroundCell[,] _bgCellGrid;
        private Blocks _currentBlock;
        private Vector2 _curPos, _prevPos;
        private List<Blocks> _gridBlocks;

        protected override void Awake()
        {
            Instance = this;
            _gridBlocks = new List<Blocks>();
            base.Awake();
        }

        public void Initialize(LevelData level, BackgroundCell[,] bgCellGrid, float blockSpawnSize)
        {
            Level = level;
            _bgCellGrid = bgCellGrid;
            DefaultBlockSize = blockSpawnSize;
        }

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (!hit) return;

            _currentBlock = hit.collider.transform.parent.GetComponent<Blocks>();
            if (_currentBlock == null) return;

            _curPos = mousePosition;
            _prevPos = mousePosition;

            _currentBlock.ElevateSprites();
            ChangeCurrentBlockScale(blockHighLightSize);

            if (_gridBlocks.Contains(_currentBlock))
                _gridBlocks.Remove(_currentBlock);

            UpdateFilled();
            ResetHighLight();
            UpdateHighLight();
        }

        private void UpdateFilled()
        {
            for (int i = 0; i < Level.gridRows; i++)
            {
                for (int j = 0; j < Level.gridColumns; j++)
                {
                    if (!_bgCellGrid[i, j].IsBlocked)
                        _bgCellGrid[i, j].SetFilled(false);
                }
            }

            foreach (var pos in from block in _gridBlocks from pos in block.BlockPositions where IsValidPos(pos) select pos)
            {
                _bgCellGrid[pos.x, pos.y].SetFilled(true);
            }
        }
        
        private bool IsValidPos(Vector2Int pos) => pos is { x: >= 0, y: >= 0 } && pos.x < Level.gridRows && pos.y < Level.gridColumns;

        protected override void HandleInputUpdate(Vector2 mousePosition)
        {
            if (_currentBlock == null) return;

            _curPos = mousePosition;
            _currentBlock.UpdatePos(_curPos - _prevPos);
            _prevPos = _curPos;

            ResetHighLight();
            UpdateHighLight();
        }

        private void ResetHighLight()
        {
            for (int i = 0; i < Level.gridRows; i++)
            {
                for (int j = 0; j < Level.gridColumns; j++)
                {
                    if (!_bgCellGrid[i, j].IsBlocked)
                        _bgCellGrid[i, j].ResetHighLight();
                }
            }
        }

        private void UpdateHighLight()
        {
            bool isCorrect = IsCorrectMove();
            foreach (var pos in _currentBlock.BlockPositions.Where(IsValidPos))
            {
                _bgCellGrid[pos.x, pos.y].UpdateHighlight(isCorrect);
            }
        }

        protected override void HandleInputEnd()
        {
            if (_currentBlock == null) return;

            _currentBlock.ElevateSprites(true);

            if (IsCorrectMove())
            {
                _currentBlock.UpdateCorrectMove();
                ChangeCurrentBlockScale(blockPutSize);
                _gridBlocks.Add(_currentBlock);
            }
            else if (Input.mousePosition.y < 0)
            {
                _currentBlock.UpdateStartMove();
                ChangeCurrentBlockScale(DefaultBlockSize);
            }
            else
            {
                _currentBlock.UpdateIncorrectMove();
                if (_currentBlock.curPos.y > 0)
                {
                    _gridBlocks.Add(_currentBlock);
                    ChangeCurrentBlockScale(blockPutSize);
                }
                else
                    ChangeCurrentBlockScale(DefaultBlockSize);
            }

            _currentBlock = null;
            ResetHighLight();
            UpdateFilled();

            CheckWinCondition();
        }

        private bool IsCorrectMove() => _currentBlock.BlockPositions.All(pos => IsValidPos(pos) && !_bgCellGrid[pos.x, pos.y].IsFilled);

        private void ChangeCurrentBlockScale(float size) => _currentBlock.transform.localScale = Vector3.one * size;

        public override void CheckWinCondition()
        {
            for (int i = 0; i < Level.gridRows; i++)
            {
                for (int j = 0; j < Level.gridColumns; j++)
                {
                    if (!_bgCellGrid[i, j].IsFilled)
                    {
                        return;
                    }
                }
            }
            OnWin?.Invoke();
        }
    }
}