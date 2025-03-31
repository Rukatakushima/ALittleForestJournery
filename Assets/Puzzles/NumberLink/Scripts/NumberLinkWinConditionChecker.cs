namespace NumberLink
{
    public class WinConditionChecker : BaseWinConditionChecker
    {
        private LevelData level;
        private Cell[,] cellGrid;

        public override void Initialize(/*LevelData levelData, Cell[,] cellGrid*/)
        {
            level = GameManager.Instance.level;
            cellGrid = GameManager.Instance.cellGrid;
        }

        public override void CheckWinCondition()
        {
            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    if (cellGrid[i, j] != null && cellGrid[i, j].Number != 0) return;
                }
            }

            NotifyWin();
        }
    }
}