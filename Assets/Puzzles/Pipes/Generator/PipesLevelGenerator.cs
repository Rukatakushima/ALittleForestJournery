using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pipes
{
    public class PipesGenerator : MonoBehaviour
    {
        public static PipesGenerator Instance;

        [SerializeField] private LevelData _level;
        [SerializeField] private SpawnCell _cellPrefab;

        [SerializeField] private int _row, _col;

        private bool hasGameFinished;
        private SpawnCell[,] pipes;
        private List<SpawnCell> startPipes;

        private void Awake()
        {
            Instance = this;
            hasGameFinished = false;
            CreateLevelData();
            SpawnLevel();
        }

        private void CreateLevelData()
        {
            if (_level.col == _col && _level.row == _row) return;

            _level.row = _row;
            _level.col = _col;
            _level.data = new List<int>();

            for (int i = 0; i < _row; i++)
            {
                for (int j = 0; j < _col; j++)
                {
                    _level.data.Add(0);
                }
            }
        }

        private void SpawnLevel()
        {
            pipes = new SpawnCell[_level.row, _level.col];
            startPipes = new List<SpawnCell>();

            for (int i = 0; i < _level.row; i++)
            {
                for (int j = 0; j < _level.col; j++)
                {
                    Vector2 spawnPos = new Vector2(j + 0.5f, i + 0.5f);
                    SpawnCell tempPipe = Instantiate(_cellPrefab);
                    tempPipe.transform.position = spawnPos;
                    tempPipe.Init(_level.data[i * _level.col + j]);
                    pipes[i, j] = tempPipe;
                    if (tempPipe.PipeType == 1)
                    {
                        startPipes.Add(tempPipe);
                    }
                }
            }

            Camera.main.orthographicSize = Mathf.Max(_level.row, _level.col) + 2f;
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.x = _level.col * 0.5f;
            cameraPos.y = _level.row * 0.5f;
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
            if (row >= _level.row) return;
            if (col >= _level.col) return;

            if (Input.GetMouseButtonDown(0))
            {
                pipes[row, col].UpdateInput();
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                pipes[row, col].Init(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                pipes[row, col].Init(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                pipes[row, col].Init(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                pipes[row, col].Init(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                pipes[row, col].Init(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                pipes[row, col].Init(5);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                pipes[row, col].Init(6);
            }

            StartCoroutine(ShowHint());
        }

        private IEnumerator ShowHint()
        {
            yield return new WaitForSeconds(0.1f);
            ResetStartPipe();
            CheckFill();
            CheckWin();
            SaveData();
        }

        private void CheckFill()
        {
            for (int i = 0; i < _level.row; i++)
            {
                for (int j = 0; j < _level.col; j++)
                {
                    SpawnCell tempPipe = pipes[i, j];
                    if (tempPipe.PipeType != 0)
                    {
                        tempPipe.IsFilled = false;
                    }
                }
            }

            Queue<SpawnCell> check = new Queue<SpawnCell>();
            HashSet<SpawnCell> finished = new HashSet<SpawnCell>();
            foreach (var pipe in startPipes)
            {
                check.Enqueue(pipe);
            }

            while (check.Count > 0)
            {
                SpawnCell pipe = check.Dequeue();
                finished.Add(pipe);
                List<SpawnCell> connected = pipe.ConnectedPipes();
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

            for (int i = 0; i < _level.row; i++)
            {
                for (int j = 0; j < _level.col; j++)
                {
                    SpawnCell tempPipe = pipes[i, j];
                    tempPipe.UpdateFilled();
                }
            }

        }

        private void ResetStartPipe()
        {
            startPipes = new List<SpawnCell>();

            for (int i = 0; i < _level.row; i++)
            {
                for (int j = 0; j < _level.col; j++)
                {
                    if (pipes[i, j].PipeType == 1)
                    {
                        startPipes.Add(pipes[i, j]);
                    }
                }
            }
        }

        private void SaveData()
        {
            for (int i = 0; i < _level.row; i++)
            {
                for (int j = 0; j < _level.col; j++)
                {
                    _level.data[i * _level.col + j] = pipes[i, j].PipeData;
                }
            }

            EditorUtility.SetDirty(_level);
        }

        private void CheckWin()
        {
            for (int i = 0; i < _level.row; i++)
            {
                for (int j = 0; j < _level.col; j++)
                {
                    if (!pipes[i, j].IsFilled) return;
                }
            }
        }
    }
}