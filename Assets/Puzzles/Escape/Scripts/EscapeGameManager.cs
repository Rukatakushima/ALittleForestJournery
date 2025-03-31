using System.Collections.Generic;
using UnityEngine;

namespace Escape
{
    public class GameManager : BaseGameManager<LevelSpawner, /*DefaultCameraController, WinConditionChecker,*/ LevelData>
    {
        public static GameManager Instance;
        public LevelData level { get; private set; }

        private List<GamePiece> gamePieces;
        public GamePiece winPiece { get; private set; }
        private GamePiece currentPiece;
        private Vector2 currentPosition, previousPosition;
        private List<Vector2> offsets;
        private bool[,] pieceCollision;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        // protected override void SetupManagers()
        // {
        //     cameraController.SetupCamera(Mathf.Max(level.Columns, level.Rows));

        //     // levelSpawner.Initialize(level);
        //     levelSpawner.SpawnLevel();

        //     winConditionChecker.Initialize(winPiece, level.Columns);
        // }

        public void Initialize(LevelData level, GamePiece winPiece, List<GamePiece> gamePieces)
        {
            this.level = level;
            this.winPiece = winPiece;
            this.gamePieces = gamePieces;
            levelSpawner.OnLevelSpawned?.Invoke();
        }

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (!hit || !hit.collider.transform.parent.TryGetComponent(out currentPiece)) return;

            currentPosition = mousePosition;
            previousPosition = currentPosition;

            CalculateCollision();

            offsets = new List<Vector2>();
        }

        protected override void HandleInputUpdate(Vector2 mousePosition)
        {
            if (currentPiece == null) return;
            {
                currentPosition = mousePosition;
                Vector2 offset = currentPosition - previousPosition;
                offsets.Add(offset);

                // bool isMovingOpposite = IsMovingOpposite();

                if (currentPiece.IsVertical)
                {
                    Vector2 piecePos = currentPiece.CurrentPos;
                    piecePos.y += (IsMovingOpposite() ? -0.5f : 0.5f);
                    Vector2Int pieceGridPos = new Vector2Int(
                        Mathf.FloorToInt(piecePos.y),
                        Mathf.FloorToInt(piecePos.x)
                        );
                    if (!CanMovePiece(pieceGridPos)) return;
                    currentPiece.CurrentGridPos = pieceGridPos;
                    currentPiece.UpdatePos(offset.y);
                }
                else
                {
                    Vector2 piecePos = currentPiece.CurrentPos;
                    piecePos.x += (IsMovingOpposite() ? -0.5f : 0.5f);
                    Vector2Int pieceGridPos = new Vector2Int(
                        Mathf.FloorToInt(piecePos.y),
                        Mathf.FloorToInt(piecePos.x)
                        );
                    if (!CanMovePiece(pieceGridPos)) return;
                    currentPiece.CurrentGridPos = pieceGridPos;
                    currentPiece.UpdatePos(offset.x);
                }

                previousPosition = currentPosition;
            }
        }

        protected override void HandleInputEnd()
        {
            if (currentPiece == null) return;
            {
                currentPiece.transform.position = new Vector3(
                    currentPiece.CurrentGridPos.y + 0.5f,
                    currentPiece.CurrentGridPos.x + 0.5f,
                    0);
                currentPiece = null;
                currentPosition = Vector2.zero;
                previousPosition = Vector2.zero;

                CheckWinCondition();
            }
        }

        private void CalculateCollision()
        {
            pieceCollision = new bool[level.Rows, level.Columns];
            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    pieceCollision[i, j] = false;
                }
            }

            foreach (var piece in gamePieces)
            {
                for (int i = 0; i < piece.Size; i++)
                {
                    pieceCollision[
                        piece.CurrentGridPos.x + (piece.IsVertical ? i : 0),
                        piece.CurrentGridPos.y + (piece.IsVertical ? 0 : i)
                        ] = true;
                }
            }

            for (int i = 0; i < currentPiece.Size; i++)
            {
                pieceCollision[
                    currentPiece.CurrentGridPos.x + (currentPiece.IsVertical ? i : 0),
                    currentPiece.CurrentGridPos.y + (currentPiece.IsVertical ? 0 : i)
                    ] = false;

            }
        }

        private bool CanMovePiece(Vector2Int pieceGridPos)
        {
            List<Vector2Int> piecePos = new List<Vector2Int>();
            for (int i = 0; i < currentPiece.Size; i++)
            {
                piecePos.Add(pieceGridPos +
                    (currentPiece.IsVertical ? Vector2Int.right : Vector2Int.up) * i
);
            }

            foreach (var pos in piecePos)
            {
                if (!IsValidPos(pos) || pieceCollision[pos.x, pos.y])
                    return false;
            }

            return true;
        }

        private bool IsMovingOpposite()
        {
            Vector2 result = Vector2.zero;
            for (int i = Mathf.Max(0, offsets.Count - 20); i < offsets.Count; i++)
            {
                result += offsets[i];
            }
            float val = currentPiece.IsVertical ? result.y : result.x;
            if (Mathf.Abs(val) > 0.2f)
            {
                return val < 0;
            }

            return true;
        }

        private bool IsValidPos(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < level.Rows && pos.y < level.Columns;

        protected override void CheckWinCondition()
        {
            if (winPiece.CurrentGridPos.y + winPiece.Size < level.Columns)
                return;

            OnWin?.Invoke();
        }
    }
}