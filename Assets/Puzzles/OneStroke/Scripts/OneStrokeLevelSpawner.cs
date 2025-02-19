using System.Collections.Generic;
using UnityEngine;

namespace OneStroke
{
    public class LevelSpawner : BaseLevelSpawner
    {
        private LevelData level;

        [SerializeField] private Point pointPrefab;
        [SerializeField] private Edge edgePrefab;
        private Dictionary<int, Point> points;
        private Dictionary<Vector2Int, Edge> edges;

        public void Initialize(LevelData level)
        {
            this.level = level;
        }

        public override void SpawnLevel()
        {
            SpawnPoints();
            SpawnEdges();
        }

        private void SpawnPoints()
        {
            points = new Dictionary<int, Point>();

            for (int i = 0; i < level.Points.Count; i++)
            {
                Vector3 positionData = level.Points[i];
                Vector2 spawnPosition = new Vector2(positionData.x, positionData.y);
                int id = (int)positionData.z;
                points[id] = Instantiate(pointPrefab);
                points[id].SetPoint(id, spawnPosition);
            }
        }

        private void SpawnEdges()
        {
            edges = new Dictionary<Vector2Int, Edge>();

            for (int i = 0; i < level.Edges.Count; i++)
            {
                Vector2Int normalEdge = level.Edges[i];
                Vector2Int reversedEdge = new Vector2Int(normalEdge.y, normalEdge.x);

                Edge spawnEdge = Instantiate(edgePrefab);
                edges[normalEdge] = spawnEdge;
                edges[reversedEdge] = spawnEdge;

                spawnEdge.SetEmptyEdge(points[normalEdge.x].Position, points[normalEdge.y].Position);
            }

            GameManager.Instance.SetEdges(edges);
        }
    }
}