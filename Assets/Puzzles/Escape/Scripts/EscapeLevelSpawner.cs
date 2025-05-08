using System.Collections.Generic;
using UnityEngine;

namespace Escape
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        [SerializeField] private SpriteRenderer backgroundPrefab;
        [SerializeField] private GamePiece blockPiecePrefab, winPiecePrefab;
        [SerializeField] private float offsetPosition = 0.5f;
        
        private GamePiece _winPiece;
        private List<GamePiece> _gamePieces;

        public override void SpawnLevel()
        {
            SpawnBackground();

            _gamePieces = new List<GamePiece>();
            SpawnWinPiece();
            SpawnAllPieces();

            GameManager.Instance.Initialize(level, _winPiece, _gamePieces);
            OnLevelSpawned?.Invoke();
        }

        private void SpawnBackground()
        {
            SpriteRenderer bg = Instantiate(backgroundPrefab);
            bg.size = new Vector2(level.columns, level.rows);
            bg.transform.position = new Vector2(level.columns, level.rows) * offsetPosition;
        }

        private void SpawnWinPiece()
        {
            _winPiece = Instantiate(winPiecePrefab);
            Vector2 spawnPos = new Vector2(level.winPiece.start.y + _winPiece.offsetPosition, level.winPiece.start.x + _winPiece.offsetPosition);
            _winPiece.transform.position = spawnPos;
            _winPiece.Init(level.winPiece);
            _gamePieces.Add(_winPiece);
        }

        private void SpawnAllPieces()
        {
            foreach (var piece in level.pieces)
            {
                GamePiece temp = Instantiate(blockPiecePrefab);
                Vector2 spawnPos = new Vector2(piece.start.y + temp.offsetPosition, piece.start.x + temp.offsetPosition);
                temp.transform.position = spawnPos;
                temp.Init(piece);
                _gamePieces.Add(temp);
            }
        }
    }
}