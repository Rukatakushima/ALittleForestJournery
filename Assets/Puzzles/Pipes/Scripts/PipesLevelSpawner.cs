using System.Collections.Generic;
using UnityEngine;

namespace Pipes
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        [SerializeField] private Pipe cellPrefab;
        // private LevelData level;
        private Pipe[,] pipes;

        // public void Initialize(LevelData level, Pipe[,] pipes)
        // {
        //     this.level = level;
        //     this.pipes = pipes;
        // }

        public override void SpawnLevel()
        {
            pipes = new Pipe[level.Row, level.Col];
            List<Pipe> startPipes = new List<Pipe>();

            for (int i = 0; i < level.Row; i++)
            {
                for (int j = 0; j < level.Col; j++)
                {
                    Vector2 spawnPos = new Vector2(j + 0.5f, i + 0.5f);
                    Pipe tempPipe = Instantiate(cellPrefab);
                    tempPipe.transform.position = spawnPos;
                    tempPipe.Init(level.Data[i * level.Col + j]);
                    pipes[i, j] = tempPipe;
                    if (tempPipe.PipeType == 1)
                    {
                        startPipes.Add(tempPipe);
                    }
                }
            }

            GameManager.Instance.Initialize(level, startPipes, pipes);
        }
    }
}