using System.Collections.Generic;
using UnityEngine;

namespace NumberLink
{
    [CreateAssetMenu(fileName = "NumberLinkLevel", menuName = "Levels/NumberLinkLevel")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] public int rows, columns;
        [SerializeField] public List<int> data;

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