using UnityEngine;

namespace Escape
{
    public class GamePiece : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer pieceRenderer;
        [SerializeField] private BoxCollider2D pieceCollider;
        [SerializeField] public float offsetPosition = 0.5f;

        [HideInInspector] public bool IsVertical;
        [HideInInspector] public int Size;
        [HideInInspector] public Vector2Int CurrentGridPos;
        [HideInInspector] public Vector2 CurrentPos;

        public void Init(Piece piece)
        {
            CurrentGridPos = piece.Start;
            IsVertical = piece.IsVertical;
            Size = piece.Size;
            CurrentPos = new Vector2(CurrentGridPos.y + offsetPosition, CurrentGridPos.x + offsetPosition);

            if (piece.IsVertical)
            {
                pieceRenderer.transform.localPosition = new Vector3(0, Size - 1, 0) * offsetPosition;
                pieceRenderer.size = new Vector2(1, Size);
                pieceCollider.size = new Vector2(1, Size);
            }
            else
            {
                pieceRenderer.transform.localPosition = new Vector3(Size - 1, 0, 0) * offsetPosition;
                pieceRenderer.size = new Vector2(Size, 1);
                pieceCollider.size = new Vector2(Size, 1);
            }
        }

        public void UpdatePos(float offset)
        {
            CurrentPos += (IsVertical ? Vector2.up : Vector2.right) * offset;
            transform.position = CurrentPos;
        }
    }
}