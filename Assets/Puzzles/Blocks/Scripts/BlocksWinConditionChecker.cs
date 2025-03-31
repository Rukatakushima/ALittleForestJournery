namespace Blocks
{
    public class WinConditionChecker : BaseWinConditionChecker
    {
        private BackgroundCell[,] bgCellGrid;
        private int rows, columns;

        public override void Initialize(/*BackgroundCell[,] grid, int rows, int columns*/)
        {
            bgCellGrid = GameManager.Instance.BGCellGrid;

            LevelData level = GameManager.Instance.level;
            rows = level.GridRows;
            columns = level.GridColumns;
        }

        public override void CheckWinCondition()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (!bgCellGrid[i, j].isFilled)
                    {
                        return;
                    }
                }
            }

            NotifyWin();
        }
    }
}