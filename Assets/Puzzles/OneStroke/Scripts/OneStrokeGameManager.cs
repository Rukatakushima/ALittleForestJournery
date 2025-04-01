using System.Collections.Generic;
using UnityEngine;

namespace OneStroke
{
    public class GameManager : BaseGameManager/*<LevelSpawner, DefaultCameraController, WinConditionChecker, LevelData>*/
    {
        public static GameManager Instance;
        // private LevelData level;

        [SerializeField] private Edge mainEdge;

        public Dictionary<Vector2Int, Edge> edges { get; private set; }
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

        // protected override void SetupManagers()
        // {
        //     cameraController.SetupCamera(Mathf.Max(level.Columns, level.Rows));

        //     levelSpawner.Initialize(level);
        //     levelSpawner.SpawnLevel();

        //     winConditionChecker.Initialize(edges);
        // }

        public void Initialize(Dictionary<Vector2Int, Edge> edges) => this.edges = edges;

        protected override void HandleInputStart(Vector2 mousePosition)
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

        protected override void HandleInputUpdate(Vector2 mousePosition)
        {
            if (!startPoint) return;

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

        protected override void HandleInputEnd()
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

        public override void CheckWinCondition()
        {
            if (edges == null) return;

            foreach (var item in edges)
            {
                if (!item.Value.IsFilled)
                {
                    return;
                }
            }

            OnWin?.Invoke();
        }
    }
}