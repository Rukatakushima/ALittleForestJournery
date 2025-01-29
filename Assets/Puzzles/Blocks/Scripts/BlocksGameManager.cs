using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectsPool;

namespace Blocks
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private Level level;
        [SerializeField] private BGCell bGCellPrefab;
        [SerializeField] private Block blockPrefab;
        [SerializeField] private float blockSpawnSize, blockHighlightSize, blockPutSize;

        private BGCell[,] bGCellGrid;
        [SerializeField] private bool hasGameFinished;
        private Block currentBloack;
        private Vector2 curPos, prevPos;
        private List<Block> gridBlocks;

        public BGCell PreloadEdge() => Instantiate(bGCellPrefab);
        public void GetAction(BGCell bGCellPrefab) => bGCellPrefab.gameObject.SetActive(true);
        public void ReturnAction(BGCell bGCellPrefab) => bGCellPrefab.gameObject.SetActive(false);
        public PoolBase<BGCell> bGCellPrefabObjectPool;
        [SerializeField] private int bGCellPrefabPreloadCount = 1;

        public float bgCellPositionRate = 0.5f;

        private void Awake()
        {
            Instance = this;
            hasGameFinished = false;
            gridBlocks = new List<Block>();
            bGCellPrefabPreloadCount = level.Rows * level.Columns;
            bGCellPrefabObjectPool = new PoolBase<BGCell>(PreloadEdge, GetAction, ReturnAction, bGCellPrefabPreloadCount);
            SpawnGrid();
            SpawnBlocks();
        }

        private void SpawnGrid()
        {
            bGCellGrid = new BGCell[level.Rows, level.Columns];
            Debug.Log(bGCellGrid);
            for (int i = 0; i < level.Rows; i++)
            {
                for (int j = 0; j < level.Columns; j++)
                {
                    BGCell bgCell = bGCellPrefabObjectPool.GetFromPool();
                    bgCell.transform.position = new Vector2(j + bgCellPositionRate, i + bgCellPositionRate);
                    bgCell.Init(level.Data[i * level.Columns + j]); // i = стока, размер строки = кол-во столбцов
                    bGCellGrid[i, j] = bgCell;
                    Debug.Log(bGCellGrid);
                }
            }
        }

        private void SpawnBlocks()
        {
            Debug.Log("SpawnBlocks");
        }

        private void Update()
        {
            if (hasGameFinished)
            {
                StartCoroutine(GameFinished());
                return;
            }
        }

        private IEnumerator GameFinished()
        {
            yield return new WaitForSeconds(1f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}