using System;
using System.Collections.Generic;
using UnityEngine;

namespace Escape
{
    [CreateAssetMenu(fileName = "EscapeLevel", menuName = "Levels/EscapeLevel", order = 0)]
    public class LevelData : ScriptableObject
    {
        public int rows, columns;
        public Piece winPiece;
        public List<Piece> pieces;
    }

    [Serializable]
    public struct Piece
    {
        public int id;
        public bool isVertical;
        public int size;
        public Vector2Int start;
    }
}