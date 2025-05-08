using System.Collections.Generic;
using ObjectsPool;
using UnityEngine;

namespace Fill
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        [SerializeField] private Cell cellPrefab;
        [SerializeField] private Transform edgePrefab;
        [SerializeField] private int cellPrefabPreloadCount = 16;
        [SerializeField] private int edgePrefabPreloadCount = 8;
        
        private List<Vector2Int> _filledPoints;
        private List<Transform> _edges;
        private Cell[,] _cells;

        private PoolBase<Cell> _cellPrefabObjectPool;
        private Cell PreloadCell() => Instantiate(cellPrefab);
        private void GetAction(Cell cell) => cell.gameObject.SetActive(true);
        private void ReturnAction(Cell cell) => cell.gameObject.SetActive(false);

        private PoolBase<Transform> _edgePrefabObjectPool;
        private Transform PreloadEdge() => Instantiate(edgePrefab);
        private void GetAction(Transform edge) => edge.gameObject.SetActive(true);
        private void ReturnAction(Transform edge) => edge.gameObject.SetActive(false);

        public override void SpawnLevel()
        {
            CreateObjectPools();

            _filledPoints = new List<Vector2Int>();
            _cells = new Cell[level.rows, level.columns];
            _edges = new List<Transform>();

            for (int i = 0; i < level.rows; i++)
            {
                for (int j = 0; j < level.columns; j++)
                {
                    _cells[i, j] = _cellPrefabObjectPool.GetFromPool();
                    _cells[i, j].Init(level.data[i * level.columns + j]);
                    _cells[i, j].transform.position = new Vector2(j + 0.5f, i + 0.5f);
                }
            }

            GameManager.Instance.Initialize(level, _edgePrefabObjectPool, _cells, _filledPoints, _edges);
            OnLevelSpawned?.Invoke();
        }

        private void CreateObjectPools()
        {
            _cellPrefabObjectPool = new PoolBase<Cell>(PreloadCell, GetAction, ReturnAction, cellPrefabPreloadCount);
            _edgePrefabObjectPool = new PoolBase<Transform>(PreloadEdge, GetAction, ReturnAction, edgePrefabPreloadCount);
        }
    }
}