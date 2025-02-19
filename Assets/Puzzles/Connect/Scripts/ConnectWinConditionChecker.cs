using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class WinConditionChecker : BaseWinConditionChecker
    {
        public List<Node> nodes;

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