using System.Collections.Generic;

namespace Connect
{
    public class WinConditionChecker : BaseWinConditionChecker
    {
        private List<Node> nodes;

        public void Initialize(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        public override void CheckWinCondition()
        {
            bool IsWinning = true;

            foreach (var item in nodes)
            {
                IsWinning &= item.IsWin;
                if (!IsWinning)
                {
                    return;
                }
            }

            NotifyWin();
        }
    }
}