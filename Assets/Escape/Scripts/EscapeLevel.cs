using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EscapeLevel", menuName = "Levels/EscapeLevel", order = 0)]
public class EscapeLevel : ScriptableObject 
{
   public int Row;
   public int Col;
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