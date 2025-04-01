using System.Collections.Generic;
using UnityEngine;
using ObjectsPool;

namespace Fill
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;
        public LevelData level { get; private set; }

        public Cell[,] cells { get; private set; }
        private List<Vector2Int> filledPoints;
        private List<Transform> edges;
        private Vector2Int startPosition, endPosition;
        private List<Vector2Int> directions = new List<Vector2Int>()
            {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
        public PoolBase<Transform> edgePrefabObjectPool;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        public void Initialize(LevelData level, PoolBase<Transform> edgePrefabObjectPool, Cell[,] cells, List<Vector2Int> filledPoints, List<Transform> edges)
        {
            this.level = level;
            this.edgePrefabObjectPool = edgePrefabObjectPool;
            this.cells = cells;
            this.filledPoints = filledPoints;
            this.edges = edges;
        }

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            startPosition = new Vector2Int(Mathf.FloorToInt(mousePosition.y), Mathf.FloorToInt(mousePosition.x));
            endPosition = startPosition;
        }

        protected override void HandleInputUpdate(Vector2 mousePosition)
        {
            endPosition = new Vector2Int(Mathf.FloorToInt(mousePosition.y), Mathf.FloorToInt(mousePosition.x));

            if (!IsNeighbour()) return;

            if (toAddEmpty())
            {
                filledPoints.Add(startPosition);
                filledPoints.Add(endPosition);
                cells[startPosition.x, startPosition.y].Add();
                cells[endPosition.x, endPosition.y].Add();
                DrawEdge(false);
            }
            else if (toAddToEnd())
            {
                filledPoints.Add(endPosition);
                cells[endPosition.x, endPosition.y].Add();
                DrawEdge(false);
            }
            else if (toAddToStart())
            {
                filledPoints.Insert(0, endPosition);
                cells[endPosition.x, endPosition.y].Add();
                DrawEdge(true);
            }
            else if (ToRemoveFromEnd())
            {
                RemoveFromPosition(edges.Count - 1);
                filledPoints.RemoveAt(filledPoints.Count - 1);
            }
            else if (toRemoveFromStart())
            {
                RemoveFromPosition(0);
                filledPoints.RemoveAt(0);
            }

            RemoveEmpty();
            CheckWinCondition();
            startPosition = endPosition;
        }

        protected override void HandleInputEnd() { }

        private bool IsNeighbour() => IsValid(startPosition) && IsValid(endPosition) && directions.Contains(startPosition - endPosition);

        private bool IsValid(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < level.Rows && pos.y < level.Columns;

        private bool toAddEmpty()
        {
            if (edges.Count > 0) return false;
            if (cells[startPosition.x, startPosition.y].Filled) return false;
            if (IsFilled(endPosition)) return false;
            return true;
        }

        private bool toAddToEnd() => ValidatePosition(filledPoints.Count - 1);

        private bool toAddToStart() => ValidatePosition(0);

        private bool ValidatePosition(int index)
        {
            if (filledPoints.Count < 2) return false;
            Vector2Int pos = filledPoints[index];
            return IsValidPosition(pos, startPosition) && !IsFilled(endPosition);
        }

        private bool IsValidPosition(Vector2Int pos, Vector2Int curPosition) => cells[curPosition.x, curPosition.y] == cells[pos.x, pos.y];

        private bool IsFilled(Vector2Int curPosition) => cells[curPosition.x, curPosition.y].Filled;

        private bool ToRemoveFromEnd() => toRemoveFromPosition(filledPoints.Count - 1);

        private bool toRemoveFromStart() => toRemoveFromPosition(0);

        private bool toRemoveFromPosition(int index)
        {
            if (filledPoints.Count < 2) return false;

            Vector2Int pos = filledPoints[index];

            if (!IsValidPosition(pos, startPosition)) return false;

            pos = filledPoints[index > 0 ? index - 1 : index + 1];

            if (!IsValidPosition(pos, endPosition)) return false;

            return true;
        }

        private void RemoveFromPosition(int index)
        {
            Transform removeEdge = edges[index];
            edges.RemoveAt(index);
            edgePrefabObjectPool.ReturnToPool(removeEdge);
            cells[startPosition.x, startPosition.y].Remove();
        }

        private void DrawEdge(bool InsertAtStart)
        {
            Transform edge = edgePrefabObjectPool.GetFromPool();

            if (InsertAtStart)
                edges.Insert(0, edge);
            else
                edges.Add(edge);

            edge.transform.position = new Vector3(
                startPosition.y * 0.5f + 0.5f + endPosition.y * 0.5f,
                startPosition.x * 0.5f + 0.5f + endPosition.x * 0.5f,
                0f);

            bool horizintal = (endPosition.y - startPosition.y) < 0 || (endPosition.y - startPosition.y) > 0;
            edge.transform.eulerAngles = new Vector3(0, 0, horizintal ? 90f : 0);
        }

        private void RemoveEmpty()
        {
            if (filledPoints.Count != 1) return;

            cells[filledPoints[0].x, filledPoints[0].y].Remove();
            filledPoints.RemoveAt(0);
        }

        public override void CheckWinCondition()
        {
            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    if (!cells[i, j].Filled)
                        return;
                }
            }

            OnWin?.Invoke();
        }
    }
}