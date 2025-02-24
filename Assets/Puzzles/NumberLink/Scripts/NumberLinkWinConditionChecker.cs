namespace NumberLink
{
    public class WinConditionChecker : BaseWinConditionChecker
    {
        private LevelData levelData;
        private Cell[,] cellGrid;

        public void Initialize(LevelData levelData, Cell[,] cellGrid)
        {
            this.levelData = levelData;
            this.cellGrid = cellGrid;
        }

        public override void CheckWinCondition()
        {
            for (int i = 0; i < levelData.Rows; i++)
            {
                for (int j = 0; j < levelData.Columns; j++)
                {
                    if (cellGrid[i, j] != null && cellGrid[i, j].Number != 0) return;
                }
            }

            NotifyWin();
        }
    }
}