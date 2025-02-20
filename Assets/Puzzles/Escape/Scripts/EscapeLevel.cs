using System;
using System.Collections.Generic;
using UnityEngine;

namespace Escape
{
    [CreateAssetMenu(fileName = "EscapeLevel", menuName = "Levels/EscapeLevel", order = 0)]
    public class LevelData : ScriptableObject
    {
        public int Rows;
        public int Columns;
        public Piece WinPiece;
        public List<Piece> Pieces;
    }

    [Serializable]
    public struct Piece
    {
        public int Id;
        public bool IsVertical;
        public int Size;
        public Vector2Int Start;
    }
}