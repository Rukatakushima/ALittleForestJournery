using System.Collections.Generic;
using UnityEngine;

namespace OneStroke
{
    public class WinConditionChecker : BaseWinConditionChecker
    {
        private Dictionary<Vector2Int, Edge> edges;

        public override void Initialize(/*Dictionary<Vector2Int, Edge> edges*/) => edges = GameManager.Instance.edges;

        public override void CheckWinCondition()
        {
            if (edges == null) return;

            foreach (var item in edges)
            {
                if (!item.Value.IsFilled)
                {
                    return;
                }
            }

            NotifyWin();
        }
    }
}