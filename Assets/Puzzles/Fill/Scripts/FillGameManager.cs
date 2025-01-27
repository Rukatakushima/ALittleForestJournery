using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectsPool;

namespace Fill
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [SerializeField] private Level _level;
        [SerializeField] private Cell _cellPrefab;
        [SerializeField] private Transform _edgePrefab;
        private bool hasGameFinished;
        private Cell[,] cells;
        private List<Vector2Int> filledPoints;
        private List<Transform> edges;
        private Vector2Int startPos, endPos;
        private List<Vector2Int> directions = new List<Vector2Int>()
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

        public float cameraSizeController = 2f;

        public Cell PreloadCell() => Instantiate(_cellPrefab);
        public void GetAction(Cell cell) => cell.gameObject.SetActive(true);
        public void ReturnAction(Cell cell) => cell.gameObject.SetActive(false);
        public PoolBase<Cell> cellPrefabObjectPool;
        [SerializeField] private int cellPrefabPreloadCount = 16;

        public Transform PreloadEdge() => Instantiate(_edgePrefab);
        public void GetAction(Transform _edgePrefab) => _edgePrefab.gameObject.SetActive(true);
        public void ReturnAction(Transform _edgePrefab) => _edgePrefab.gameObject.SetActive(false);
        public PoolBase<Transform> edgePrefabObjectPool;
        [SerializeField] private int edgePrefabPreloadCount = 8;

        private void Awake()
        {
            Instance = this;
            hasGameFinished = false;
            filledPoints = new List<Vector2Int>();
            cells = new Cell[_level.Row, _level.Col];
            edges = new List<Transform>();

            cellPrefabObjectPool = new PoolBase<Cell>(PreloadCell, GetAction, ReturnAction, cellPrefabPreloadCount);
            edgePrefabObjectPool = new PoolBase<Transform>(PreloadEdge, GetAction, ReturnAction, edgePrefabPreloadCount);

            SpawnLevel();
        }

        private void SpawnLevel()
        {
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = _level.Col * 0.5f;
            camPos.y = _level.Row * 0.5f;
            Camera.main.transform.position = camPos;
            Camera.main.orthographicSize = Mathf.Max(_level.Row, _level.Col) / cameraSizeController;

            for (int i = 0; i < _level.Row; i++)
            {
                for (int j = 0; j < _level.Col; j++)
                {
                    // cells[i, j] = Instantiate(_cellPrefab);
                    cells[i, j] = cellPrefabObjectPool.GetFromPool();
                    cells[i, j].Init(_level.Data[i * _level.Col + j]);
                    cells[i, j].transform.position = new Vector3(j + 0.5f, i + 0.5f);
                }
            }
        }
        private void Update()
        {
            if (hasGameFinished) return;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                startPos = new Vector2Int(Mathf.FloorToInt(mousePos.y), Mathf.FloorToInt(mousePos.x));
                endPos = startPos;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                endPos = new Vector2Int(Mathf.FloorToInt(mousePos.y), Mathf.FloorToInt(mousePos.x));
                if (!IsNeighbour()) return;
                if (toAddEmpty())
                {
                    filledPoints.Add(startPos);
                    filledPoints.Add(endPos);
                    cells[startPos.x, startPos.y].Add();
                    cells[endPos.x, endPos.y].Add();
                    DrawEdge(false);
                }
                else if (toAddToEnd())
                {
                    filledPoints.Add(endPos);
                    cells[endPos.x, endPos.y].Add();
                    DrawEdge(false);
                }
                else if (toAddToStart())
                {
                    filledPoints.Insert(0, endPos);
                    cells[endPos.x, endPos.y].Add();
                    DrawEdge(true);
                }
                else if (toRemoveFromEnd())
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
                CheckWin();
                startPos = endPos;
            }
        }
        private bool IsNeighbour()
        {
            return IsValid(startPos) && IsValid(endPos) && directions.Contains(startPos - endPos);
        }
        private bool IsValid(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < _level.Row && pos.y < _level.Col;
        }
        private bool toAddEmpty()
        {
            if (edges.Count > 0) return false;
            if (cells[startPos.x, startPos.y].Filled) return false;
            if (IsFilled(endPos)) return false;
            return true;
        }
        private bool toAddToEnd()
        {
            return ValidatePosition(filledPoints.Count - 1);
        }

        private bool toAddToStart()
        {
            return ValidatePosition(0);
        }
        private bool ValidatePosition(int index)
        {
            if (filledPoints.Count < 2) return false;
            Vector2Int pos = filledPoints[index];
            return IsValidPosition(pos, startPos) && !IsFilled(endPos);
        }

        private bool IsValidPosition(Vector2Int pos, Vector2Int curPos)
        {
            return cells[curPos.x, curPos.y] == cells[pos.x, pos.y];
        }
        private bool IsFilled(Vector2Int curPos) //endPos
        {
            return cells[curPos.x, curPos.y].Filled;
        }
        private bool toRemoveFromEnd()
        {
            return toRemoveFromPosition(filledPoints.Count - 1);
        }

        private bool toRemoveFromStart()
        {
            return toRemoveFromPosition(0);
        }

        private bool toRemoveFromPosition(int index)
        {
            if (filledPoints.Count < 2) return false;
            Vector2Int pos = filledPoints[index];
            if (!IsValidPosition(pos, startPos)) return false;
            pos = filledPoints[index > 0 ? index - 1 : index + 1];
            if (!IsValidPosition(pos, endPos)) return false;
            return true;
        }

        private void RemoveFromPosition(int index)
        {
            Transform removeEdge = edges[index];
            edges.RemoveAt(index);
            // Destroy(removeEdge.gameObject);
            edgePrefabObjectPool.ReturnToPool(removeEdge);
            cells[startPos.x, startPos.y].Remove();
        }

        private void DrawEdge(bool InsertAtStart)
        {
            // Transform edge = Instantiate(_edgePrefab);
            Transform edge = edgePrefabObjectPool.GetFromPool();
            if (InsertAtStart)
            {
                edges.Insert(0, edge);
            }
            else
            {
                edges.Add(edge);
            }
            edge.transform.position = new Vector3(
                startPos.y * 0.5f + 0.5f + endPos.y * 0.5f,
                startPos.x * 0.5f + 0.5f + endPos.x * 0.5f,
                0f
            );
            bool horizintal = (endPos.y - startPos.y) < 0 || (endPos.y - startPos.y) > 0;
            edge.transform.eulerAngles = new Vector3(0, 0, horizintal ? 90f : 0);
        }

        private void RemoveEmpty()
        {
            if (filledPoints.Count != 1) return;
            cells[filledPoints[0].x, filledPoints[0].y].Remove();
            filledPoints.RemoveAt(0);
        }

        private void CheckWin()
        {
            for (int i = 0; i < _level.Row; i++)
            {
                for (int j = 0; j < _level.Col; j++)
                {
                    if (!cells[i, j].Filled)
                        return;
                }
            }
            hasGameFinished = true;
            StartCoroutine(GameFinished());
        }

        private IEnumerator GameFinished()
        {
            yield return new WaitForSeconds(1f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}