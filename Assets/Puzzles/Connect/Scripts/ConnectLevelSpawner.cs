using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class LevelSpawner : BaseLevelSpawner
    {
        [SerializeField] public LevelData CurrentLevelData;
        [SerializeField] private SpriteRenderer _boardPrefab, _bgCellPrefab;
        [SerializeField] private Node _nodePrefab;
        public List<Node> _nodes;//private
        public Dictionary<Vector2Int, Node> _nodeGrid;
        [SerializeField] private int currentLevelSize = 5;

        // public void Initialize(int currentLevelSize)
        // {
        //     this.currentLevelSize = currentLevelSize;
        // }

        public override void SpawnLevel()
        {
            SpawnBoard();
            SpawnNodes();
        }

        private void SpawnBoard()
        {
            //инициализация подложки: префаб, положение (делим размер на 2, чтобы разместить в середине), поворот
            var board = Instantiate(_boardPrefab,
                new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, 0f),
                Quaternion.identity);

            // добавлеяем к размеру "обводку\рамку"
            board.size = new Vector2(currentLevelSize + 0.1f, currentLevelSize + 0.1f);

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    Instantiate(_bgCellPrefab,
                        new Vector3(i + 0.5f, j + 0.5f, 0f),
                        Quaternion.identity);
                }
            }

            // Camera.main.orthographicSize = currentLevelSize / cameraSizeController;
            // Camera.main.transform.position = new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, -10f);

        }

        private void SpawnNodes()
        {
            _nodes = new List<Node>();
            _nodeGrid = new Dictionary<Vector2Int, Node>();

            Node spawnedNode;
            Vector3 spawnPos;

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    spawnPos = new Vector3(i + 0.5f, j + 0.5f, 0f);
                    spawnedNode = Instantiate(_nodePrefab, spawnPos, Quaternion.identity);
                    spawnedNode.Init();

                    int colorIdForSpawnedNode = GetColorId(i, j);
                    //окрас point в цвет, который будем соединять

                    if (colorIdForSpawnedNode != -1)
                    {
                        spawnedNode.SetColorForPoint(colorIdForSpawnedNode);
                    }

                    _nodes.Add(spawnedNode);
                    _nodeGrid.Add(new Vector2Int(i, j), spawnedNode);
                    spawnedNode.gameObject.name = i.ToString() + j.ToString();
                    spawnedNode.Pos2D = new Vector2Int(i, j);
                }

                GameManager.Instance.SetNodes(_nodes, _nodeGrid, currentLevelSize);
            }

            //установка сетки: сетка = nodes, у каждого node есть список offsetPos, т.е. 4 edge (верх, них, лево, право)
            List<Vector2Int> offsetPos = new List<Vector2Int>()
            { Vector2Int.up, Vector2Int.down,Vector2Int.left, Vector2Int.right};

            foreach (var item in _nodeGrid)
            {
                foreach (var offset in offsetPos)
                {
                    var checkPos = item.Key + offset;
                    if (_nodeGrid.ContainsKey(checkPos))
                    {
                        item.Value.SetEdge(offset, _nodeGrid[checkPos]);
                    }
                }
            }
        }

        public int GetColorId(int i, int j)
        {
            List<Edge> edges = CurrentLevelData.Connections;
            Vector2Int point = new Vector2Int(i, j);

            for (int colorId = 0; colorId < edges.Count; colorId++)
            {
                if (edges[colorId].StartPoint == point || edges[colorId].EndPoint == point) return colorId;
            }

            return -1;
        }
    }
}