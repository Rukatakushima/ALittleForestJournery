using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    [CreateAssetMenu(fileName = "BlocksLevel", menuName = "Levels/BlocksLevel")]
    public class LevelData : ScriptableObject
    {
        public int gridRows, gridColumns;
        public int blockRows, blockColumns;
        public List<int> data;
        public List<BlockPiece> blocks;

        private void OnValidate()
        {
            int newSize = gridRows * gridColumns;
            if (data.Count == newSize) return;
            
            while (data.Count < newSize)
            {
                data.Add(0);
            }
            if (data.Count > newSize)
            {
                data.RemoveRange(newSize, data.Count - newSize);
            }
        }
    }

    [Serializable]
    public struct BlockPiece
    {
        public Vector2Int startPosition;
        public List<Vector2Int> blockPositions;
    }
}
