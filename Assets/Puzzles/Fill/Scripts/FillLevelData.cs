using System.Collections.Generic;
using UnityEngine;

namespace Fill
{
    [CreateAssetMenu(fileName = "FillLevel", menuName = "Levels/FillLevel")]
    public class LevelData : ScriptableObject
    {
        public int rows, columns;
        public List<int> data = new ();

        private void OnValidate()
        {
            int newSize = rows * columns;
            if (data.Count == newSize) return;
            
            while (data.Count < newSize)
            {
                data.Add(0);
            }

            if (data.Count > newSize)
                data.RemoveRange(newSize, data.Count - newSize);
        }
    }
}