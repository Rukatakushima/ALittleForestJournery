namespace Fifteen
{
    public class WinConditionChecker : BaseWinConditionChecker
    {
        private Box[,] boxes;

        public override void Initialize(/*Box[,] boxes*/) => boxes = GameManager.Instance.boxes;

        public override void CheckWinCondition()
        {
            {
                for (int i = 0; i < boxes.GetLength(0); i++)
                {
                    for (int j = 0; j < boxes.GetLength(1); j++)
                    {
                        if (!boxes[i, j].isInCorrectPosition)
                        {
                            return;
                        }
                    }
                }

                NotifyWin();
            }
        }
    }
}