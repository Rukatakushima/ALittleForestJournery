using System.Collections.Generic;
using UnityEngine;

namespace Pipes
{
    [CreateAssetMenu(fileName = "PipesLevel", menuName = "Levels/PipesLevel")]
    public class LevelData : ScriptableObject
    {
        public int row, col;
        public List<int> data = new ();
        
        private void OnValidate()
        {
            int newSize = row * col;
            if (data.Count == newSize) return;
            
            while (data.Count < newSize)
            {
                data.Add(0);
            }

            if (data.Count > newSize) data.RemoveRange(newSize, data.Count - newSize);
        }
    }
}