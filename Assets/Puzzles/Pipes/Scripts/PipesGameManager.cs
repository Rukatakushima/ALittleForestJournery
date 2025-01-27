using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pipes
{
    public class PipesGameManager : MonoBehaviour
    {
        public static PipesGameManager Instance;

        [SerializeField] private PipesLevelData _level;
        [SerializeField] private PipesPipe _cellPrefab;

        private bool hasGameFinished;
        private PipesPipe[,] pipes;
        private List<PipesPipe> startPipes;

        public float cameraSizeController = 2f;

        private void Awake()
        {
            Instance = this;
            hasGameFinished = false;
            SpawnLevel();
        }

        private void SpawnLevel()
        {
            pipes = new PipesPipe[_level.Row, _level.Col];
            startPipes = new List<PipesPipe>();

            for (int i = 0; i < _level.Row; i++)
            {
                for (int j = 0; j < _level.Col; j++)
                {
                    Vector2 spawnPos = new Vector2(j + 0.5f, i + 0.5f);
                    PipesPipe tempPipe = Instantiate(_cellPrefab);
                    tempPipe.transform.position = spawnPos;
                    tempPipe.Init(_level.Data[i * _level.Col + j]);
                    pipes[i, j] = tempPipe;
                    if (tempPipe.PipeType == 1)
                    {
                        startPipes.Add(tempPipe);
                    }
                }
            }

            Camera.main.orthographicSize = Mathf.Max(_level.Row, _level.Col) / cameraSizeController;
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.x = _level.Col * 0.5f;
            cameraPos.y = _level.Row * 0.5f;
            Camera.main.transform.position = cameraPos;

            StartCoroutine(ShowHint());
        }

        private void Update()
        {
            if (hasGameFinished) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int row = Mathf.FloorToInt(mousePos.y);
            int col = Mathf.FloorToInt(mousePos.x);
            if (row < 0 || col < 0) return;
            if (row >= _level.Row) return;
            if (col >= _level.Col) return;

            if (Input.GetMouseButtonDown(0))
            {
                pipes[row, col].UpdateInput();
                StartCoroutine(ShowHint());
            }
        }

        private IEnumerator ShowHint()
        {
            yield return new WaitForSeconds(0.1f);
            CheckFill();
            CheckWin();
        }

        private void CheckFill()
        {
            for (int i = 0; i < _level.Row; i++)
            {
                for (int j = 0; j < _level.Col; j++)
                {
                    PipesPipe tempPipe = pipes[i, j];
                    if (tempPipe.PipeType != 0)
                    {
                        tempPipe.IsFilled = false;
                    }
                }
            }

            Queue<PipesPipe> check = new Queue<PipesPipe>();
            HashSet<PipesPipe> finished = new HashSet<PipesPipe>();
            foreach (var pipe in startPipes)
            {
                check.Enqueue(pipe);
            }

            while (check.Count > 0)
            {
                PipesPipe pipe = check.Dequeue();
                finished.Add(pipe);
                List<PipesPipe> connected = pipe.ConnectedPipes();
                foreach (var connectedPipe in connected)
                {
                    if (!finished.Contains(connectedPipe))
                    {
                        check.Enqueue(connectedPipe);
                    }
                }
            }

            foreach (var filled in finished)
            {
                filled.IsFilled = true;
            }

            for (int i = 0; i < _level.Row; i++)
            {
                for (int j = 0; j < _level.Col; j++)
                {
                    PipesPipe tempPipe = pipes[i, j];
                    tempPipe.UpdateFilled();
                }
            }

        }

        private void CheckWin()
        {
            for (int i = 0; i < _level.Row; i++)
            {
                for (int j = 0; j < _level.Col; j++)
                {
                    if (!pipes[i, j].IsFilled)
                        return;
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