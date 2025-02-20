using System.Collections.Generic;
using UnityEngine;

namespace Escape
{
    public class LevelSpawner : BaseLevelSpawner
    {
        private LevelData level;
        private GamePiece winPiece;
        [SerializeField] private SpriteRenderer backgroundPrefab;
        [SerializeField] private GamePiece blockPiecePrefab;
        [SerializeField] private GamePiece mainPiecePrefab;
        private List<GamePiece> gamePieces;

        public void Initialize(LevelData level)
        {
            this.level = level;
        }

        public override void SpawnLevel()
        {
            SpawnBackground();

            gamePieces = new List<GamePiece>();
            SpawnWinPiece();
            SpawnAllPieces();

            GameManager.Instance.SetPieces(winPiece, gamePieces);
        }

        private void SpawnBackground()
        {
            SpriteRenderer bg = Instantiate(backgroundPrefab);
            bg.size = new Vector2(level.Columns, level.Rows);
            bg.transform.position = new Vector3(level.Columns, level.Rows, 0) * 0.5f;
        }

        private void SpawnWinPiece()
        {
            winPiece = Instantiate(mainPiecePrefab);
            Vector3 spawnPos = new Vector3(
                level.WinPiece.Start.y + 0.5f,
                level.WinPiece.Start.x + 0.5f,
                0);
            winPiece.transform.position = spawnPos;
            winPiece.Init(level.WinPiece);
            gamePieces.Add(winPiece);
        }

        private void SpawnAllPieces()
        {
            foreach (var piece in level.Pieces)
            {
                GamePiece temp = Instantiate(blockPiecePrefab);
                Vector3 spawnPos = new Vector3(
                    piece.Start.y + 0.5f,
                    piece.Start.x + 0.5f, 0
                    );
                temp.transform.position = spawnPos;
                temp.Init(piece);
                gamePieces.Add(temp);
            }
        }
    }
}