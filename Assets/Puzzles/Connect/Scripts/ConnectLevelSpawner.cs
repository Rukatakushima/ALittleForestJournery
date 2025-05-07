using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        [SerializeField] private int levelSize = 5;
        [SerializeField] private float boardTracingSize = 0.1f;
        [SerializeField] private float boardPositionController = 2f;
        [SerializeField] private SpriteRenderer boardPrefab, backgroundCellPrefab;
        [SerializeField] private Node nodePrefab;
        
        private List<Node> _nodes;
        private Dictionary<Vector2Int, Node> _nodeGrid;

        public override void SpawnLevel()
        {
            SpawnBoard();
            SpawnNodes();

            GameManager.Instance.Initialize(_nodes);
            OnLevelSpawned?.Invoke();
        }

        private void SpawnBoard()
        {
            //инициализация подложки: префаб, положение (делим размер на 2, чтобы разместить в середине), поворот
            var board = Instantiate(boardPrefab, new Vector2(levelSize / boardPositionController, levelSize / boardPositionController), Quaternion.identity);
            board.size = new Vector2(levelSize + boardTracingSize, levelSize + boardTracingSize);

            for (int i = 0; i < levelSize; i++)
            {
                for (int j = 0; j < levelSize; j++)
                {
                    Instantiate(backgroundCellPrefab, new Vector2(i + backgroundCellPrefab.transform.localScale.x, j + backgroundCellPrefab.transform.localScale.y), Quaternion.identity);
                }
            }
        }

        private void SpawnNodes()
        {
            _nodes = new List<Node>();
            _nodeGrid = new Dictionary<Vector2Int, Node>();

            for (int i = 0; i < levelSize; i++)
            {
                for (int j = 0; j < levelSize; j++)
                {
                    Vector3 spawnPos = new Vector2(i + nodePrefab.transform.localScale.x, j + nodePrefab.transform.localScale.y);
                    var spawnedNode = Instantiate(nodePrefab, spawnPos, Quaternion.identity);
                    spawnedNode.Init();

                    //окрас point в цвет, который будем соединять
                    int colorIdForSpawnedNode = GetColorId(i, j);

                    if (colorIdForSpawnedNode != -1)
                        spawnedNode.SetColorForPoint(colorIdForSpawnedNode);

                    _nodes.Add(spawnedNode);
                    _nodeGrid.Add(new Vector2Int(i, j), spawnedNode);
                }
            }

            //установка сетки: сетка = nodes, у каждого node есть список offsetPos, т.е. 4 edge (верх, них, лево, право)
            List<Vector2Int> offsetPos = new List<Vector2Int>()
            { Vector2Int.up, Vector2Int.down,Vector2Int.left, Vector2Int.right};

            foreach (var item in _nodeGrid)
            {
                foreach (var offset in offsetPos)
                {
                    var checkPos = item.Key + offset;
                    if (_nodeGrid.TryGetValue(checkPos, out var node))
                        item.Value.SetEdge(offset, node);
                }
            }
        }

        private int GetColorId(int i, int j)
        {
            List<Edge> edges = level.connections;
            Vector2Int point = new Vector2Int(i, j);

            for (int colorId = 0; colorId < edges.Count; colorId++)
            {
                if (edges[colorId].StartPoint == point || edges[colorId].EndPoint == point)
                    return colorId;
            }

            return -1;
        }
    }
}