using System.Collections.Generic;
using UnityEngine;

namespace Fill
{
    [CreateAssetMenu(fileName = "FillLevel", menuName = "Levels/FillLevel")]
    public class LevelData : ScriptableObject
    {
        public int Rows;
        public int Columns;
        public List<int> Data = new List<int>();

        private void OnValidate()
        {
            int newSize = Rows * Columns;
            if (Data.Count != newSize)
            {
                while (Data.Count < newSize)
                {
                    Data.Add(0);
                }

                if (Data.Count > newSize)
                    Data.RemoveRange(newSize, Data.Count - newSize);
            }
        }
    }
}