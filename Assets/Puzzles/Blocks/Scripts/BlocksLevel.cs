using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    [CreateAssetMenu(fileName = "Level", menuName = "BlocksLevel")]
    public class LevelData : ScriptableObject
    {
        public int Rows, Columns, BlockRows, BlockColumns;
        public List<int> Data;
        public List<BlockPiece> Blocks;
/*
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
                {
                    Data.RemoveRange(newSize, Data.Count - newSize);
                }
            }
        }
    */
    }

    [Serializable]
    public struct BlockPiece
    {
        public int Id;
        public Vector2Int StartPos, CenterPos;
        public List<Vector2Int> BlockPositions;
    }
}
