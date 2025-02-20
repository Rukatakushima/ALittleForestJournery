namespace Fill
{
    public class WinConditionChecker : BaseWinConditionChecker
    {
        private LevelData level;
        private Cell[,] cells;

        public void Initialize(LevelData level, Cell[,] cells)
        {
            this.level = level;
            this.cells = cells;
        }

        public override void CheckWinCondition()
        {
            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    if (!cells[i, j].Filled)
                        return;
                }
            }

            NotifyWin();
        }
    }
}