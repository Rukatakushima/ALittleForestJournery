namespace Blocks
{
    public class WinConditionChecker : BaseWinConditionChecker
    {
        private BackgroundCell[,] bgCellGrid;
        private int rows, columns;

        public void Initialize(BackgroundCell[,] grid, int rows, int columns)
        {
            bgCellGrid = grid;
            this.rows = rows;
            this.columns = columns;
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