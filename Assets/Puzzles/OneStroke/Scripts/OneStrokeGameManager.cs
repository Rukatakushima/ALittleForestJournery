using System.Collections.Generic;
using UnityEngine;

namespace OneStroke
{
    public class GameManager : BaseGameManager<LevelSpawner, DefaultCameraController, WinConditionChecker, LevelData>
    {
        public static GameManager Instance;

        [SerializeField] private Edge mainEdge;

        private Dictionary<Vector2Int, Edge> edges;
        private Point startPoint, endPoint;
        private Vector2 previousHit;
        private int currentId;
        private const int startId = -1;

        protected override void Awake()
        {
            Instance = this;
            currentId = startId;
            base.Awake();
        }

        protected override void SetupManagers()
        {
            cameraController.SetupCamera(Mathf.Max(level.Columns, level.Rows));

            levelSpawner.Initialize(level);
            levelSpawner.SpawnLevel();
            
            winConditionChecker.Initialize(edges);
        }

        public void SetEdges(Dictionary<Vector2Int, Edge> edges)
        {
            this.edges = edges;
        }

        protected override void HandleMouseDown(Vector2 mousePosition)
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

        protected override void HandleMouseDrag(Vector2 mousePosition)
        {
            if (!startPoint) return;

            // Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit)
            {
                endPoint = hit.collider.gameObject.GetComponent<Point>();
            }

            mainEdge.SetEndPosition(mousePosition);

            if (startPoint == endPoint || endPoint == null) return;

            if (IsStartFilled())
            {
                FillEdge();
            }
            else if (IsEndFilled())
            {
                FillEdge();
                CheckWinCondition();
            }
        }

        protected override void HandleMouseUp()
        {
            mainEdge.TurnOff();

            startPoint = null;
            endPoint = null;

            CheckWinCondition();
        }
        
        private void FillEdge()
        {
            currentId = endPoint.Id;
            edges[new Vector2Int(startPoint.Id, endPoint.Id)].FillEdge();
            startPoint = endPoint;
            mainEdge.UpdateHighlightPosition(startPoint.Position);
            previousHit = endPoint.transform.position;
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