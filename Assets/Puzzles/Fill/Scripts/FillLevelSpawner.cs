using System.Collections.Generic;
using ObjectsPool;
using UnityEngine;

namespace Fill
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        // private LevelData level;

        // private Cell[,] cells;
        private List<Vector2Int> filledPoints;
        private List<Transform> edges;
        private Cell[,] cells;

        [SerializeField] private Cell cellPrefab;
        public Cell PreloadCell() => Instantiate(cellPrefab);
        public void GetAction(Cell cellPrefab) => cellPrefab.gameObject.SetActive(true);
        public void ReturnAction(Cell cellPrefab) => cellPrefab.gameObject.SetActive(false);
        public PoolBase<Cell> cellPrefabObjectPool;
        [SerializeField] private int cellPrefabPreloadCount = 16;

        [SerializeField] private Transform edgePrefab;
        public Transform PreloadEdge() => Instantiate(edgePrefab);
        public void GetAction(Transform edgePrefab) => edgePrefab.gameObject.SetActive(true);
        public void ReturnAction(Transform edgePrefab) => edgePrefab.gameObject.SetActive(false);
        public PoolBase<Transform> edgePrefabObjectPool;
        [SerializeField] private int edgePrefabPreloadCount = 8;

        // public void Initialize(LevelData level, Cell[,] cells)
        // {
        //     this.level = level;
        //     this.cells = cells;
        // }

        public override void SpawnLevel()
        {
            filledPoints = new List<Vector2Int>();
            cells = new Cell[level.Rows, level.Columns];
            edges = new List<Transform>();

            CreateObjectPools();

            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    cells[i, j] = cellPrefabObjectPool.GetFromPool();
                    cells[i, j].Init(level.Data[i * level.Columns + j]);
                    cells[i, j].transform.position = new Vector2(j + 0.5f, i + 0.5f);
                }
            }

            GameManager.Instance.Initialize(level, edgePrefabObjectPool, cells, filledPoints, edges);
            // OnLevelSpawned?.Invoke();
        }

        private void CreateObjectPools()
        {
            cellPrefabObjectPool = new PoolBase<Cell>(PreloadCell, GetAction, ReturnAction, cellPrefabPreloadCount);
            edgePrefabObjectPool = new PoolBase<Transform>(PreloadEdge, GetAction, ReturnAction, edgePrefabPreloadCount);
        }
    }
}