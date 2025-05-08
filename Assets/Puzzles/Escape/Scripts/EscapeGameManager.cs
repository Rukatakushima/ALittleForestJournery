using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Escape
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;
        
        [SerializeField] private LevelData level;
        [SerializeField] private GamePiece winPiece;

        private List<GamePiece> _gamePieces;
        private GamePiece _currentPiece;
        private Vector2 _currentPosition, _previousPosition;
        private List<Vector2> _offsets;
        private bool[,] _pieceCollision;
        
        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        public void Initialize(LevelData levelData, GamePiece win, List<GamePiece> pieces)
        {
            level = levelData;
            winPiece = win;
            _gamePieces = pieces;
        }

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (!hit || !hit.collider.transform.parent.TryGetComponent(out _currentPiece)) return;

            _currentPosition = mousePosition;
            _previousPosition = _currentPosition;

            CalculateCollision();

            _offsets = new List<Vector2>();
        }

        protected override void HandleInputUpdate(Vector2 mousePosition)
        {
            if (_currentPiece == null) return;
            {
                _currentPosition = mousePosition;
                Vector2 offset = _currentPosition - _previousPosition;
                _offsets.Add(offset);

                if (_currentPiece.IsVertical)
                {
                    Vector2 piecePos = _currentPiece.CurrentPos;
                    piecePos.y += (IsMovingOpposite() ? -0.5f : 0.5f);
                    Vector2Int pieceGridPos = new Vector2Int(
                        Mathf.FloorToInt(piecePos.y),
                        Mathf.FloorToInt(piecePos.x)
                        );
                    if (!CanMovePiece(pieceGridPos)) return;
                    _currentPiece.CurrentGridPos = pieceGridPos;
                    _currentPiece.UpdatePos(offset.y);
                }
                else
                {
                    Vector2 piecePos = _currentPiece.CurrentPos;
                    piecePos.x += (IsMovingOpposite() ? -0.5f : 0.5f);
                    Vector2Int pieceGridPos = new Vector2Int(
                        Mathf.FloorToInt(piecePos.y),
                        Mathf.FloorToInt(piecePos.x)
                        );
                    if (!CanMovePiece(pieceGridPos)) return;
                    _currentPiece.CurrentGridPos = pieceGridPos;
                    _currentPiece.UpdatePos(offset.x);
                }

                _previousPosition = _currentPosition;
            }
        }

        protected override void HandleInputEnd()
        {
            if (_currentPiece == null) return;
            {
                _currentPiece.transform.position = new Vector3(
                    _currentPiece.CurrentGridPos.y + 0.5f,
                    _currentPiece.CurrentGridPos.x + 0.5f,
                    0);
                _currentPiece = null;
                _currentPosition = Vector2.zero;
                _previousPosition = Vector2.zero;

                CheckWinCondition();
            }
        }

        private void CalculateCollision()
        {
            _pieceCollision = new bool[level.rows, level.columns];
            for (int i = 0; i < level.rows; i++)
            {
                for (int j = 0; j < level.columns; j++)
                {
                    _pieceCollision[i, j] = false;
                }
            }

            foreach (var piece in _gamePieces)
            {
                for (int i = 0; i < piece.Size; i++)
                {
                    _pieceCollision[
                        piece.CurrentGridPos.x + (piece.IsVertical ? i : 0),
                        piece.CurrentGridPos.y + (piece.IsVertical ? 0 : i)
                        ] = true;
                }
            }

            for (int i = 0; i < _currentPiece.Size; i++)
            {
                _pieceCollision[
                    _currentPiece.CurrentGridPos.x + (_currentPiece.IsVertical ? i : 0),
                    _currentPiece.CurrentGridPos.y + (_currentPiece.IsVertical ? 0 : i)
                    ] = false;

            }
        }

        private bool CanMovePiece(Vector2Int pieceGridPos)
        {
            List<Vector2Int> piecePos = new List<Vector2Int>();
            for (int i = 0; i < _currentPiece.Size; i++)
            {
                piecePos.Add(pieceGridPos +
                    (_currentPiece.IsVertical ? Vector2Int.right : Vector2Int.up) * i);
            }

            return piecePos.All(pos => IsValidPos(pos) && !_pieceCollision[pos.x, pos.y]);
        }

        private bool IsMovingOpposite()
        {
            Vector2 result = Vector2.zero;
            for (int i = Mathf.Max(0, _offsets.Count - 20); i < _offsets.Count; i++)
            {
                result += _offsets[i];
            }

            float val = _currentPiece.IsVertical ? result.y : result.x;
            if (Mathf.Abs(val) > 0.2f)
                return val < 0;

            return true;
        }

        private bool IsValidPos(Vector2Int pos) => pos is { x: >= 0, y: >= 0 } && pos.x < level.rows && pos.y < level.columns;

        public override void CheckWinCondition()
        {
            if (winPiece.CurrentGridPos.y + winPiece.Size < level.columns)
                return;

            OnWin?.Invoke();
        }
    }
}