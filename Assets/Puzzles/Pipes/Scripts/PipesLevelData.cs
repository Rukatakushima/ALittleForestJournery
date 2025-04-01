using System.Collections.Generic;
using UnityEngine;

namespace Pipes
{
    [CreateAssetMenu(fileName = "PipesLevel", menuName = "Levels/PipesLevel")]
    public class LevelData : ScriptableObject
    {
        public int Row;
        public int Col;
        public List<int> Data = new List<int>();
        private void OnValidate()
        {
            int newSize = Row * Col;
            if (Data.Count != newSize)
            {
                while (Data.Count < newSize)
                {
                    Data.Add(0);
                }

                if (Data.Count > newSize) Data.RemoveRange(newSize, Data.Count - newSize);
            }
        }
    }
}