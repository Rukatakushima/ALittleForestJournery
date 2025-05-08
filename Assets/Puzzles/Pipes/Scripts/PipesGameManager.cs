using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pipes
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;
        
        private LevelData _level;
        private Pipe[,] _pipes;
        private FillChecker _fillChecker;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
            StartCoroutine(ShowHint());
        }

        public void Initialize(LevelData levelData, List<Pipe> startPipes, Pipe[,] pipes)
        {
            _level = levelData;
            _fillChecker = new FillChecker(pipes, startPipes);
            _pipes = pipes;
        }

        private Coroutine _hintCoroutine;

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            int row = Mathf.FloorToInt(mousePosition.y);
            int col = Mathf.FloorToInt(mousePosition.x);
            if (row < 0 || col < 0 || row >= _level.row || col >= _level.col) return;

            _pipes[row, col].RotatePipe();

            if (_hintCoroutine != null) StopCoroutine(_hintCoroutine);
            _hintCoroutine = StartCoroutine(ShowHint());
        }

        protected override void HandleInputUpdate(Vector2 mousePosition) { }

        protected override void HandleInputEnd() { }

        private IEnumerator ShowHint()
        {
            yield return new WaitForSeconds(0.1f);
            _fillChecker.CheckFill();
            CheckWinCondition();
        }

        public override void CheckWinCondition()
        {
            for (int i = 0; i < _level.row; i++)
            {
                for (int j = 0; j < _level.col; j++)
                {
                    if (!_pipes[i, j].isFilled)
                        return;
                }
            }

            OnWin?.Invoke();
        }
    }
}