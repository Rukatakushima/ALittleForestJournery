using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Escape
{
    public class EscapeGenerator : MonoBehaviour
    {
        public static EscapeGenerator Instance;
        public static int PieceId = 0;

        [SerializeField] private int _row;
        [SerializeField] private int _col;
        [SerializeField] private Level _level;
        [SerializeField] private SpriteRenderer _bgPrefab;
        [SerializeField] private LevelPiece _piecePrefab;
        [SerializeField] private LevelPiece _winPrefab;

        private bool isStartPiece;

        private LevelPiece winPiece;
        private LevelPiece curPiece;

        private void Awake()
        {
            Instance = this;
            isStartPiece = true;
            CreateLevel();
            SpawnLevel();
        }

        private void CreateLevel()
        {
            if (_row == _level.Row && _col == _level.Col)
            {
                return;
            }

            _level.Row = _row;
            _level.Col = _col;
            _level.Pieces = new List<Piece>();
            _level.WinPiece = new Piece();
            EditorUtility.SetDirty(_level);
        }

        private void SpawnLevel()
        {
            //Set Up BG
            SpriteRenderer bg = Instantiate(_bgPrefab);
            bg.size = new Vector2(_level.Col, _level.Row);
            bg.transform.position = new Vector3(_level.Col, _level.Row, 0) * 0.5f;

            SpawnWinPiece();

            //Spawn All Pieces
            for (int i = 0; i < _level.Pieces.Count; i++)
            {
                Piece piece = _level.Pieces[i];
                piece.Id = PieceId++;
                _level.Pieces[i] = piece;
                Vector3 spawnPos = new Vector3(
                    piece.Start.y + 0.5f,
                    piece.Start.x + 0.5f, 0f
                    );
                LevelPiece temp = Instantiate(_piecePrefab);
                temp.transform.position = spawnPos;
                temp.Init(piece);
            }

            //Set Up Camera
            Camera.main.orthographicSize = Mathf.Max(_level.Col, _level.Row) * 1.2f + 2f;
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = _level.Col * 0.5f;
            camPos.y = _level.Row * 0.5f;
            Camera.main.transform.position = camPos;
        }

        private void SpawnWinPiece()
        {
            winPiece = Instantiate(_winPrefab);
            Vector3 spawnPos = new Vector3(
                _level.WinPiece.Start.y + 0.5f,
                _level.WinPiece.Start.x + 0.5f, 0
                );
            winPiece.transform.position = spawnPos;
            _level.WinPiece.Id = 0;
            winPiece.Init(_level.WinPiece);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                isStartPiece = !isStartPiece;
            }

            if (isStartPiece)
            {
                UpdateStartPiece();
            }
            else
            {
                UpdateGamePiece();
            }
        }

        private void UpdateStartPiece()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int mouseGrid = new Vector2Int(
                    Mathf.FloorToInt(mousePos.y),
                    Mathf.FloorToInt(mousePos.x)
                    );
                if (!IsValidPos(mouseGrid))
                {
                    return;
                }

                if (winPiece != null)
                {
                    Destroy(winPiece.gameObject);
                }

                _level.WinPiece = new Piece()
                {
                    Id = 0,
                    IsVertical = false,
                    Size = 1,
                    Start = mouseGrid,
                };

                EditorUtility.SetDirty(_level);
                SpawnWinPiece();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (winPiece != null)
                {
                    Destroy(winPiece.gameObject);
                    winPiece = null;
                }

                _level.WinPiece = new Piece();
                EditorUtility.SetDirty(_level);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (winPiece == null)
                {
                    return;
                }

                Destroy(winPiece.gameObject);
                _level.WinPiece.Size++;
                SpawnWinPiece();
                EditorUtility.SetDirty(_level);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (winPiece == null)
                {
                    return;
                }

                Destroy(winPiece.gameObject);
                _level.WinPiece.Size--;
                if (_level.WinPiece.Size == 0)
                {
                    _level.WinPiece.Size = 1;
                }

                SpawnWinPiece();
                EditorUtility.SetDirty(_level);
            }
        }

        private void UpdateGamePiece()
        {
            if (!Input.anyKeyDown)
            {
                return;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            Vector2Int mouseGrid = new Vector2Int(
                    Mathf.FloorToInt(mousePos.y),
                    Mathf.FloorToInt(mousePos.x)
                    );

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (!hit || !IsValidPos(mouseGrid)) return;
                if (hit.collider.transform.parent.TryGetComponent(out curPiece))
                {
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (!IsValidPos(mouseGrid)) return;
                SpawnGamePiece(true);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (!IsValidPos(mouseGrid)) return;
                SpawnGamePiece(false);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (curPiece == null)
                {
                    return;
                }

                int removeId = -1;

                for (int i = 0; i < _level.Pieces.Count; i++)
                {
                    if (_level.Pieces[i].Id == curPiece.Id)
                    {
                        removeId = i;
                        break;
                    }
                }
                Debug.Log(curPiece.Id);
                Debug.Log(removeId);
                if (removeId != -1)
                {
                    _level.Pieces.RemoveAt(removeId);
                }

                if (curPiece != null)
                {
                    Destroy(curPiece.gameObject);
                    curPiece = null;
                }

                EditorUtility.SetDirty(_level);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (curPiece == null)
                {
                    return;
                }

                for (int i = 0; i < _level.Pieces.Count; i++)
                {
                    Piece piece = _level.Pieces[i];
                    if (piece.Id == curPiece.Id)
                    {
                        piece.Size++;
                        _level.Pieces[i] = piece;
                        curPiece.Init(piece);
                        break;
                    }
                }

                EditorUtility.SetDirty(_level);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (curPiece == null)
                {
                    return;
                }

                for (int i = 0; i < _level.Pieces.Count; i++)
                {
                    Piece piece = _level.Pieces[i];
                    if (piece.Id == curPiece.Id)
                    {
                        piece.Size--;
                        if (piece.Size == 0)
                        {
                            piece.Size = 1;
                        }
                        _level.Pieces[i] = piece;
                        curPiece.Init(piece);
                        break;
                    }
                }

                EditorUtility.SetDirty(_level);
            }
        }

        private void SpawnGamePiece(bool vertical)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int mouseGrid = new Vector2Int(
                    Mathf.FloorToInt(mousePos.y),
                    Mathf.FloorToInt(mousePos.x)
                    );
            Piece spawnPiece = new Piece();
            spawnPiece.Id = PieceId++;
            spawnPiece.IsVertical = vertical;
            spawnPiece.Start = mouseGrid;
            spawnPiece.Size = 1;
            _level.Pieces.Add(spawnPiece);
            SpawnGamePiece(spawnPiece);
            EditorUtility.SetDirty(_level);
        }

        private void SpawnGamePiece(Piece piece)
        {
            Vector3 spawnPos = new Vector3(
                piece.Start.y + 0.5f,
                piece.Start.x + 0.5f, 0f
                );
            LevelPiece temp = Instantiate(_piecePrefab);
            temp.transform.position = spawnPos;
            temp.Init(piece);
            curPiece = temp;
        }

        private bool IsValidPos(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < _row && pos.y < _col;
        }
    }
}