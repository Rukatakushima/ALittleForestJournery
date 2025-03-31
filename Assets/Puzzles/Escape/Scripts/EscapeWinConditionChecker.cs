namespace Escape
{
    public class WinConditionChecker : BaseWinConditionChecker
    {
        private GamePiece winPiece;
        private int columnsCount;

        public override void Initialize(/*GamePiece winPiece, int columnsCount*/)
        {
            winPiece = GameManager.Instance.winPiece;
            columnsCount = GameManager.Instance.level.Columns;
        }

        public override void CheckWinCondition()
        {
            if (winPiece.CurrentGridPos.y + winPiece.Size < columnsCount)
                return;

            NotifyWin();
        }
    }
}