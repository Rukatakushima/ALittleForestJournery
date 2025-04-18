using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blocks
{

    [CreateAssetMenu(fileName = "BlocksLevel", menuName = "Levels/BlocksLevel")]
    public class LevelData : ScriptableObject
    {
        public int GridRows, GridColumns, BlockRows, BlockColumns;
        public List<int> Data;
        public List<BlockPiece> Blocks;

        private void OnValidate()
        {
            int newSize = GridRows * GridColumns;
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

        public int CalculateTotalBlockPositions => Blocks.Sum(block => block.BlockPositions.Count);
    }

    [Serializable]
    public struct BlockPiece
    {
        public int Id;
        public Vector2Int StartPosition, CenterPosition;
        public List<Vector2Int> BlockPositions;
    }
}
