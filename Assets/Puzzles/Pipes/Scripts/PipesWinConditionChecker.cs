using UnityEngine;

namespace Pipes
{
    public class WinConditionChecker : BaseWinConditionChecker
    {
        private LevelData level;
        private Pipe[,] pipes;

        public void Initialize(LevelData level, Pipe[,] pipes)
        {
            this.level = level;
            this.pipes = pipes;
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

            NotifyWin();
        }
    }
}