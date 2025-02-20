using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pipes
{
    public class GameManager : BaseGameManager<LevelSpawner, DefaultCameraController, WinConditionChecker, LevelData>
    {
        public static GameManager Instance;

        private Pipe[,] pipes;
        private List<Pipe> startPipes;
        private FillChecker fillChecker;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        protected override void SetupManagers()
        {
            cameraController.SetupCamera(Mathf.Max(level.Row, level.Col));

            startPipes = new List<Pipe>();
            levelSpawner.Initialize(level, pipes);
            levelSpawner.SpawnLevel();

            winConditionChecker.Initialize(level, pipes);

            fillChecker = new FillChecker(pipes, startPipes);

            StartCoroutine(ShowHint());
        }

        public void SetPipes(List<Pipe> startPipes, Pipe[,] pipes)
        {
            this.startPipes = startPipes;
            this.pipes = pipes;
        }


        protected override void HandleMouseDown(Vector2 mousePosition)
        {
            int row = Mathf.FloorToInt(mousePosition.y);
            int col = Mathf.FloorToInt(mousePosition.x);
            if (row < 0 || col < 0) return;
            if (row >= level.Row) return;
            if (col >= level.Col) return;

            pipes[row, col].RotatePipe();
            StartCoroutine(ShowHint());
        }

        protected override void HandleMouseDrag(Vector2 mousePosition) { }

        protected override void HandleMouseUp() { }

        private IEnumerator ShowHint()
        {
            yield return new WaitForSeconds(0.1f);
            fillChecker.CheckFill();
            CheckWinCondition();
        }
    }
}