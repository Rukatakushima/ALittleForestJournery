using System.Collections.Generic;
using UnityEngine;

namespace OneStroke
{
    public class WinConditionChecker : BaseWinConditionChecker
    {
        private Dictionary<Vector2Int, Edge> edges;

        public void Initialize(Dictionary<Vector2Int, Edge> edges)
        {
            this.edges = edges; // Передаем ссылку на edges
        }

        public override bool isWinConditionFulfilled()
        {
            if (edges == null) return false;

            foreach (var item in edges)
            {
                if (!item.Value.IsFilled)
                {
                    return false;
                }
            }
            NotifyWin();
            return true;
        }
    }
}