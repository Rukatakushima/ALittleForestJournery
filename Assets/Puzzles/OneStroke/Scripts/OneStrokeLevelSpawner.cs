using System.Collections.Generic;
using UnityEngine;

namespace OneStroke
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        [SerializeField] private Point pointPrefab;
        [SerializeField] private Edge edgePrefab;
        
        private Dictionary<int, Point> _points;
        private Dictionary<Vector2Int, Edge> _edges;

        public override void SpawnLevel()
        {
            SpawnPoints();
            SpawnEdges();

            GameManager.Instance.Initialize(_edges);
            OnLevelSpawned?.Invoke();
        }

        private void SpawnPoints()
        {
            _points = new Dictionary<int, Point>();

            foreach (var positionData in level.points)
            {
                Vector2 spawnPosition = new Vector2(positionData.x, positionData.y);
                int id = (int)positionData.z;
                _points[id] = Instantiate(pointPrefab);
                _points[id].SetPoint(id, spawnPosition);
            }
        }

        private void SpawnEdges()
        {
            _edges = new Dictionary<Vector2Int, Edge>();

            foreach (var normalEdge in level.edges)
            {
                Vector2Int reversedEdge = new Vector2Int(normalEdge.y, normalEdge.x);

                Edge spawnEdge = Instantiate(edgePrefab);
                _edges[normalEdge] = spawnEdge;
                _edges[reversedEdge] = spawnEdge;

                spawnEdge.SetEmptyEdge(_points[normalEdge.x].Position, _points[normalEdge.y].Position);
            }
        }
    }
}