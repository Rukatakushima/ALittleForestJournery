namespace Escape
{
    public class WinConditionChecker : BaseWinConditionChecker
    {

        private GamePiece winPiece;
        private int columnsCount;

        public void Initialize(GamePiece winPiece, int columnsCount)
        {
            this.winPiece = winPiece;
            this.columnsCount = columnsCount;
        }

        public override void CheckWinCondition()
        {
            if (winPiece.CurrentGridPos.y + winPiece.Size < columnsCount)
                return;

            NotifyWin();
        }
    }
}