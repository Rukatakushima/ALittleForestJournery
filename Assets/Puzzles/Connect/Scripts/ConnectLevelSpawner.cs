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
        private List<Node> nodes;
        private Dictionary<Vector2Int, Node> nodeGrid;

        public override void SpawnLevel()
        {
            SpawnBoard();
            SpawnNodes();

            GameManager.Instance.Initialize(nodes);
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
            nodes = new List<Node>();
            nodeGrid = new Dictionary<Vector2Int, Node>();

            Node spawnedNode;
            Vector3 spawnPos;

            for (int i = 0; i < levelSize; i++)
            {
                for (int j = 0; j < levelSize; j++)
                {
                    spawnPos = new Vector2(i + nodePrefab.transform.localScale.x, j + nodePrefab.transform.localScale.y);
                    spawnedNode = Instantiate(nodePrefab, spawnPos, Quaternion.identity);
                    spawnedNode.Init();

                    //окрас point в цвет, который будем соединять
                    int colorIdForSpawnedNode = GetColorId(i, j);

                    if (colorIdForSpawnedNode != -1)
                        spawnedNode.SetColorForPoint(colorIdForSpawnedNode);

                    nodes.Add(spawnedNode);
                    nodeGrid.Add(new Vector2Int(i, j), spawnedNode);
                    spawnedNode.gameObject.name = i.ToString() + j.ToString();
                    spawnedNode.Pos2D = new Vector2Int(i, j);
                }

            }

            //установка сетки: сетка = nodes, у каждого node есть список offsetPos, т.е. 4 edge (верх, них, лево, право)
            List<Vector2Int> offsetPos = new List<Vector2Int>()
            { Vector2Int.up, Vector2Int.down,Vector2Int.left, Vector2Int.right};

            foreach (var item in nodeGrid)
            {
                foreach (var offset in offsetPos)
                {
                    var checkPos = item.Key + offset;
                    if (nodeGrid.ContainsKey(checkPos))
                        item.Value.SetEdge(offset, nodeGrid[checkPos]);
                }
            }
        }

        public int GetColorId(int i, int j)
        {
            List<Edge> edges = /*GameManager.Instance.*/level.Connections;
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