using System.Collections.Generic;
using UnityEngine;

namespace Pipes
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        [SerializeField] private Pipe cellPrefab;
        private Pipe[,] _pipes;

        public override void SpawnLevel()
        {
            _pipes = new Pipe[level.row, level.col];
            List<Pipe> startPipes = new List<Pipe>();

            for (int i = 0; i < level.row; i++)
            {
                for (int j = 0; j < level.col; j++)
                {
                    Vector2 spawnPos = new Vector2(j + 0.5f, i + 0.5f);
                    Pipe tempPipe = Instantiate(cellPrefab);
                    tempPipe.transform.position = spawnPos;
                    tempPipe.Init(level.data[i * level.col + j]);
                    _pipes[i, j] = tempPipe;
                    if (tempPipe.pipeType == 1)
                    {
                        startPipes.Add(tempPipe);
                    }
                }
            }

            GameManager.Instance.Initialize(level, startPipes, _pipes);
            OnLevelSpawned?.Invoke();
        }
    }
}