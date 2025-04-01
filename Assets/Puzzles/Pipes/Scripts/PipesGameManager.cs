using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pipes
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;
        public LevelData level { get; private set; }

        public Pipe[,] pipes { get; private set; }
        private FillChecker fillChecker;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
            StartCoroutine(ShowHint());
        }

        public void Initialize(LevelData level, List<Pipe> startPipes, Pipe[,] pipes)
        {
            this.level = level;
            fillChecker = new FillChecker(pipes, startPipes);
            this.pipes = pipes;
        }

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            int row = Mathf.FloorToInt(mousePosition.y);
            int col = Mathf.FloorToInt(mousePosition.x);
            if (row < 0 || col < 0) return;
            if (row >= level.Row) return;
            if (col >= level.Col) return;

            pipes[row, col].RotatePipe();
            StartCoroutine(ShowHint());
        }

        protected override void HandleInputUpdate(Vector2 mousePosition) { }

        protected override void HandleInputEnd() { }

        private IEnumerator ShowHint()
        {
            yield return new WaitForSeconds(0.1f);
            fillChecker.CheckFill();
            CheckWinCondition();
        }

        public override void CheckWinCondition()
        {
            for (int i = 0; i < level.Row; i++)
            {
                for (int j = 0; j < level.Col; j++)
                {
                    if (!pipes[i, j].IsFilled)
                        return;
                }
            }

            OnWin?.Invoke();
        }
    }
}