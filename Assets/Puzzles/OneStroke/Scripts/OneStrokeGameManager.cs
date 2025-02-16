using System.Collections.Generic;
using UnityEngine;

namespace OneStroke
{
    public class GameManager : BaseGameManager<DefaultCameraController, WinConditionChecker, LevelData>
    {
        [SerializeField] private Point pointPrefab;
        [SerializeField] private Edge edgePrefab;
        [SerializeField] private Edge mainEdge;

        private Dictionary<int, Point> points;
        private Dictionary<Vector2Int, Edge> edges;
        private Point startPoint, endPoint;
        private Vector2 previousHit;
        private int currentId;
        private const int startId = -1;

        protected override void Awake()
        {
            currentId = startId;
            points = new Dictionary<int, Point>();
            edges = new Dictionary<Vector2Int, Edge>();

            base.Awake();
        }

        protected override void SetupManagers()
        {
            cameraController.SetupCamera(Mathf.Max(level.Columns, level.Rows));

            winConditionChecker.Initialize(edges);
            winConditionChecker.OnWin.AddListener(sceneLoader.ChangeScene);
        }

        protected override void SpawnLevel()
        {
            for (int i = 0; i < level.Points.Count; i++)
            {
                Vector3 positionData = level.Points[i];
                Vector2 spawnPosition = new Vector2(positionData.x, positionData.y);
                int id = (int)positionData.z;
                points[id] = Instantiate(pointPrefab);
                points[id].SetPoint(id, spawnPosition);
            }

            for (int i = 0; i < level.Edges.Count; i++)
            {
                Vector2Int normalEdge = level.Edges[i];
                Vector2Int reversedEdge = new Vector2Int(normalEdge.y, normalEdge.x);

                Edge spawnEdge = Instantiate(edgePrefab);
                edges[normalEdge] = spawnEdge;
                edges[reversedEdge] = spawnEdge;

                spawnEdge.SetEmptyEdge(points[normalEdge.x].Position, points[normalEdge.y].Position);
            }
        }

        protected override void MouseDown(Vector2 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (!hit) return;

            startPoint = hit.collider.gameObject.GetComponent<Point>();
            if (!startPoint) return;

            if (currentId == startId || (Vector2)hit.transform.position == previousHit)
            {
                mainEdge.SetFilledGradient();
            }
            else
            {
                mainEdge.SetWrongGradient();
            }

            mainEdge.SetStartHighlight(startPoint.Position);
        }

        protected override void MouseDrag(Vector2 mousePosition)
        {
            if (!startPoint) return;

            Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit)
            {
                endPoint = hit.collider.gameObject.GetComponent<Point>();
            }

            mainEdge.SetEndPosition(mousePos);

            if (startPoint == endPoint || endPoint == null) return;

            if (IsStartFilled())
            {
                FillEdge();
            }
            else if (IsEndFilled())
            {
                FillEdge();
                winConditionChecker.CheckWinCondition();
            }
        }

        private void FillEdge()
        {
            currentId = endPoint.Id;
            edges[new Vector2Int(startPoint.Id, endPoint.Id)].FillEdge();
            startPoint = endPoint;
            mainEdge.UpdateHighlightPosition(startPoint.Position);
            previousHit = endPoint.transform.position;
        }

        protected override void OnMouseUp()
        {
            mainEdge.TurnOff();

            startPoint = null;
            endPoint = null;

            winConditionChecker.CheckWinCondition();
        }

        private bool IsStartFilled()
        {
            if (currentId != startId) return false;

            Vector2Int edge = new Vector2Int(startPoint.Id, endPoint.Id);
            return edges.ContainsKey(edge);
        }

        private bool IsEndFilled()
        {
            if (currentId != startPoint.Id) return false;

            Vector2Int edgeId = new Vector2Int(endPoint.Id, startPoint.Id);
            if (edges.TryGetValue(edgeId, out Edge result))
            {
                return result != null && !result.IsFilled;
            }
            return false;
        }
    }
}