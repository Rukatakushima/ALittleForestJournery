using System.Collections.Generic;
using UnityEngine;

namespace NumberLink
{
    [CreateAssetMenu(fileName = "NumberLinkLevel", menuName = "Levels/NumberLinkLevel")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] public int Rows, Columns;
        [SerializeField] public List<int> Data;

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