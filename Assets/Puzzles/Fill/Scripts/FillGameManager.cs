using System.Collections.Generic;
using UnityEngine;
using ObjectsPool;

namespace Fill
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;

        private Cell[,] _cells;
        private List<Vector2Int> _filledPoints;
        private List<Transform> _edges;
        private Vector2Int _startPosition, _endPosition;
        private readonly List<Vector2Int> _directions = new () {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
        private PoolBase<Transform> _edgePrefabObjectPool;
        
        public LevelData Level { get; private set; }
        private bool IsNeighbour() => IsValid(_startPosition) && IsValid(_endPosition) && _directions.Contains(_startPosition - _endPosition);
        private bool IsValid(Vector2Int pos) => pos is { x: >= 0, y: >= 0 } && pos.x < Level.rows && pos.y < Level.columns;
        private bool ToAddEmpty()
        {
            if (_edges.Count > 0) return false;
            if (_cells[_startPosition.x, _startPosition.y].filled) return false;
            return !IsFilled(_endPosition);
        }
        private bool ToAddToEnd() => ValidatePosition(_filledPoints.Count - 1);
        private bool ToAddToStart() => ValidatePosition(0);
        private bool ValidatePosition(int index)
        {
            if (_filledPoints.Count < 2) return false;
            Vector2Int pos = _filledPoints[index];
            return IsValidPosition(pos, _startPosition) && !IsFilled(_endPosition);
        }
        private bool IsValidPosition(Vector2Int pos, Vector2Int curPosition) => _cells[curPosition.x, curPosition.y] == _cells[pos.x, pos.y];
        private bool IsFilled(Vector2Int curPosition) => _cells[curPosition.x, curPosition.y].filled;
        private bool ToRemoveFromEnd() => ToRemoveFromPosition(_filledPoints.Count - 1);
        private bool ToRemoveFromStart() => ToRemoveFromPosition(0);
        private bool ToRemoveFromPosition(int index)
        {
            if (_filledPoints.Count < 2) return false;

            Vector2Int pos = _filledPoints[index];
            if (!IsValidPosition(pos, _startPosition)) return false;

            pos = _filledPoints[index > 0 ? index - 1 : index + 1];
            return IsValidPosition(pos, _endPosition);
        }

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        public void Initialize(LevelData level, PoolBase<Transform> edgePrefabObjectPool, Cell[,] cells, List<Vector2Int> filledPoints, List<Transform> edges)
        {
            Level = level;
            _edgePrefabObjectPool = edgePrefabObjectPool;
            _cells = cells;
            _filledPoints = filledPoints;
            _edges = edges;
        }

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            _startPosition = new Vector2Int(Mathf.FloorToInt(mousePosition.y), Mathf.FloorToInt(mousePosition.x));
            _endPosition = _startPosition;
        }

        protected override void HandleInputUpdate(Vector2 mousePosition)
        {
            _endPosition = new Vector2Int(Mathf.FloorToInt(mousePosition.y), Mathf.FloorToInt(mousePosition.x));

            if (!IsNeighbour()) return;

            if (ToAddEmpty())
            {
                _filledPoints.Add(_startPosition);
                _filledPoints.Add(_endPosition);
                _cells[_startPosition.x, _startPosition.y].Add();
                _cells[_endPosition.x, _endPosition.y].Add();
                DrawEdge(false);
            }
            else if (ToAddToEnd())
            {
                _filledPoints.Add(_endPosition);
                _cells[_endPosition.x, _endPosition.y].Add();
                DrawEdge(false);
            }
            else if (ToAddToStart())
            {
                _filledPoints.Insert(0, _endPosition);
                _cells[_endPosition.x, _endPosition.y].Add();
                DrawEdge(true);
            }
            else if (ToRemoveFromEnd())
            {
                RemoveFromPosition(_edges.Count - 1);
                _filledPoints.RemoveAt(_filledPoints.Count - 1);
            }
            else if (ToRemoveFromStart())
            {
                RemoveFromPosition(0);
                _filledPoints.RemoveAt(0);
            }

            RemoveEmpty();
            CheckWinCondition();
            _startPosition = _endPosition;
        }

        protected override void HandleInputEnd() { }

        private void RemoveFromPosition(int index)
        {
            Transform removeEdge = _edges[index];
            _edges.RemoveAt(index);
            _edgePrefabObjectPool.ReturnToPool(removeEdge);
            _cells[_startPosition.x, _startPosition.y].Remove();
        }

        private void DrawEdge(bool insertAtStart)
        {
            Transform edge = _edgePrefabObjectPool.GetFromPool();

            if (insertAtStart)
                _edges.Insert(0, edge);
            else
                _edges.Add(edge);

            edge.transform.position = new Vector3(
                _startPosition.y * 0.5f + 0.5f + _endPosition.y * 0.5f,
                _startPosition.x * 0.5f + 0.5f + _endPosition.x * 0.5f,
                0f);

            bool isHorizontal = (_endPosition.y - _startPosition.y) < 0 || (_endPosition.y - _startPosition.y) > 0;
            edge.transform.eulerAngles = new Vector3(0, 0, isHorizontal ? 90f : 0);
        }

        private void RemoveEmpty()
        {
            if (_filledPoints.Count != 1) return;

            _cells[_filledPoints[0].x, _filledPoints[0].y].Remove();
            _filledPoints.RemoveAt(0);
        }

        public override void CheckWinCondition()
        {
            for (int i = 0; i < Level.rows; i++)
            {
                for (int j = 0; j < Level.columns; j++)
                {
                    if (!_cells[i, j].filled)
                        return;
                }
            }

            OnWin?.Invoke();
        }
    }
}