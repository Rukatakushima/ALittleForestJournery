using System.Collections.Generic;
using UnityEngine;

namespace Pipes
{
    [CreateAssetMenu(fileName = "PipesLevel", menuName = "Levels/PipesLevel")]
    public class PipesLevelData : ScriptableObject
    {
        public int Rows;
        public int Columns;
        public List<int> Data = new List<int>();
        private void OnValidate()
        {
            // Если значения Row или Col изменились, обновить размер списка Data
            int newSize = Rows * Columns;
            if (Data.Count != newSize)
            {
                while (Data.Count < newSize)
                {
                    Data.Add(0);
                }
                if (Data.Count > newSize)
                {
                    Data.RemoveRange(newSize, Data.Count - newSize);
                }
            }
        }
    }
}