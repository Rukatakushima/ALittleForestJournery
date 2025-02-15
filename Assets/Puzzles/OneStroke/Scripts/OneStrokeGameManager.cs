using System.Collections.Generic;
using UnityEngine;

namespace OneStroke
{
    public class GameManager : BaseGameManager<CameraController, WinConditionChecker>
    {
        [Header("Game Settings")]
        [SerializeField] private LevelData level;
        [SerializeField] private Point pointPrefab;
        [SerializeField] private Edge edgePrefab;
        [SerializeField] private LineRenderer highlight;
        private Edge highlightEdge;
        private Vector2 previousHit;

        private Dictionary<int, Point> points;
        private Dictionary<Vector2Int, Edge> edges;
        private Point startPoint, endPoint;
        private int currentId;

        private const int startId = -1;
        private const int positionCount = 2;
        private const int startPositionIndex = 0;
        private const int endPositionIndex = 1;

        protected override void Awake()
        {
            InitializeComponents();

            currentId = startId;
            points = new Dictionary<int, Point>();
            edges = new Dictionary<Vector2Int, Edge>();
            highlight.gameObject.SetActive(false);
            highlightEdge = highlight.gameObject.GetComponent<Edge>();

            SpawnLevel();
            SetupManagers();
        }

        protected override void SetupManagers()
        {
            cameraController.SetupCamera(Mathf.Max(level.Columns, level.Rows));

            winConditionChecker.Initialize(edges);
            winConditionChecker.OnWin.AddListener(sceneLoader.ChangeScene);
        }

        private void SpawnLevel()
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

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = GetHit();

                if (!hit) return;

                if (currentId == -1 || (Vector2)hit.transform.position == previousHit)
                {
                    highlightEdge.SetFilledGradient();
                }
                else
                {
                    highlightEdge.SetWrongGradient();
                }

                startPoint = hit.collider.gameObject.GetComponent<Point>();

                highlight.gameObject.SetActive(true);
                highlight.positionCount = positionCount;
                UpdateHighlightPosition();
            }
            else if (Input.GetMouseButton(0) && startPoint != null)
            {
                Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit)
                {
                    endPoint = hit.collider.gameObject.GetComponent<Point>();
                }
                highlight.SetPosition(endPositionIndex, mousePos);

                if (startPoint == endPoint || endPoint == null) return;

                if (IsStartFilled())
                {
                    currentId = endPoint.Id;
                    edges[new Vector2Int(startPoint.Id, endPoint.Id)].FillEdge();
                    startPoint = endPoint;
                    UpdateHighlightPosition();
                    previousHit = hit.transform.position;
                }
                else if (IsEndFilled())
                {
                    currentId = endPoint.Id;
                    edges[new Vector2Int(startPoint.Id, endPoint.Id)].FillEdge();

                    winConditionChecker.CheckWinCondition();

                    startPoint = endPoint;
                    UpdateHighlightPosition();

                    previousHit = hit.transform.position;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                highlight.gameObject.SetActive(false);
                startPoint = null;
                endPoint = null;

                winConditionChecker.CheckWinCondition();
            }
        }

        private RaycastHit2D GetHit()
        {
            Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return Physics2D.Raycast(mousePos, Vector2.zero);
        }

        private void UpdateHighlightPosition()
        {
            highlight.SetPosition(startPositionIndex, startPoint.Position);
            highlight.SetPosition(endPositionIndex, startPoint.Position);
        }

        private bool IsStartFilled()
        {
            if (currentId != -1) return false;
            Vector2Int edge = new Vector2Int(startPoint.Id, endPoint.Id);
            if (!edges.ContainsKey(edge)) return false;
            return true;
        }

        private bool IsEndFilled()
        {
            if (currentId != startPoint.Id) return false;

            Vector2Int edge = new Vector2Int(endPoint.Id, startPoint.Id);
            if (edges.TryGetValue(edge, out Edge result))
            {
                if (result == null || result.IsFilled) return false;
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}