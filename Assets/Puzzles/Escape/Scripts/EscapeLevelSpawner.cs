using System.Collections.Generic;
using UnityEngine;

namespace Escape
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        private GamePiece winPiece;
        [SerializeField] private SpriteRenderer backgroundPrefab;
        [SerializeField] private GamePiece blockPiecePrefab;
        [SerializeField] private GamePiece mainPiecePrefab;
        private List<GamePiece> gamePieces;
        [SerializeField] private float offsetPosition = 0.5f;

        public override void SpawnLevel()
        {
            SpawnBackground();

            gamePieces = new List<GamePiece>();
            SpawnWinPiece();
            SpawnAllPieces();

            GameManager.Instance.Initialize(level, winPiece, gamePieces);
            OnLevelSpawned?.Invoke();
        }

        private void SpawnBackground()
        {
            SpriteRenderer bg = Instantiate(backgroundPrefab);
            bg.size = new Vector2(level.Columns, level.Rows);
            bg.transform.position = new Vector2(level.Columns, level.Rows) * offsetPosition;
        }

        private void SpawnWinPiece()
        {
            winPiece = Instantiate(mainPiecePrefab);
            Vector2 spawnPos = new Vector2(level.WinPiece.Start.y + winPiece.offsetPosition, level.WinPiece.Start.x + winPiece.offsetPosition);
            winPiece.transform.position = spawnPos;
            winPiece.Init(level.WinPiece);
            gamePieces.Add(winPiece);
        }

        private void SpawnAllPieces()
        {
            foreach (var piece in level.Pieces)
            {
                GamePiece temp = Instantiate(blockPiecePrefab);
                Vector2 spawnPos = new Vector2(piece.Start.y + temp.offsetPosition, piece.Start.x + temp.offsetPosition);
                temp.transform.position = spawnPos;
                temp.Init(piece);
                gamePieces.Add(temp);
            }
        }
    }
}