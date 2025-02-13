using System;
using System.Collections;
using System.Collections.Generic;
using Blocks;
using UnityEngine;

namespace OneStroke
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private LevelData level;
        [SerializeField] private Edge edgePrefab;
        [SerializeField] private Point pointPrefab;
        [SerializeField] private LineRenderer highlight;

        private Dictionary<int, Point> points;
        private Dictionary<Vector2Int, Edge> edges;
        private Point startPoint, endPoint;
        private int currentId;
        private const int startId = -1;

        [SerializeField] private float cameraSizeController = 0.5f;
        [SerializeField] private float cameraPositionController = 2f;

        private bool hasGameFinished = false;
        [SerializeField] private float sceneReloadDelay = 1f;

        // [Header("Line Renderer Parameters")]
        private int positionCount = 2;
        [SerializeField] private int startPositionIndex = 0;
        [SerializeField] private int endPositionIndex = 1;

        private void Awake()
        {
            hasGameFinished = false;
            points = new Dictionary<int, Point>();
            edges = new Dictionary<Vector2Int, Edge>();
            highlight.gameObject.SetActive(false);
            currentId = startId;
            SpawnLevel();
        }

        private void SpawnLevel()
        {
            SetCamera();

            for (int i = 0; i < level.Points.Count; i++)
            {
                Vector4 positionData = level.Points[i];
                Vector2 spawnPosition = new Vector3(positionData.x, positionData.y);
                int id = (int)positionData.z;
                points[id] = Instantiate(pointPrefab);
                points[id].Initialize(id, spawnPosition);
            }

            for (int i = 0; i < level.Edges.Count; i++)
            {
                Vector2Int normalEdge = level.Edges[i];
                Vector2Int reversedEdge = new Vector2Int(normalEdge.y, normalEdge.x);
                Edge spawnEdge = Instantiate(edgePrefab);
                edges[normalEdge] = spawnEdge;
                edges[reversedEdge] = spawnEdge;

                Debug.Log("points[normalEdge.x].Position = " + points[normalEdge.x].Position);
                Debug.Log("normalEdge.x = " + normalEdge.x);
                Debug.Log("points[normalEdge.y].Position = " + points[normalEdge.y].Position);
                Debug.Log("normalEdge.y = " + normalEdge.y);

                spawnEdge.Initialize(points[normalEdge.x].Position, points[normalEdge.y].Position);
            }
        }

        private void Update()
        {
            if (hasGameFinished)
            {
                StartCoroutine(GameFinished());
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                // Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                // RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                RaycastHit2D hit = GetHit();

                if (!hit) return;

                startPoint = hit.collider.gameObject.GetComponent<Point>();

                highlight.gameObject.SetActive(true);
                highlight.positionCount = positionCount;
                UpdateHighlightPosition();
                // SetHighlightPosition(startPoint.Position, startPoint.Position);
                // highlight.SetPosition(startPositionIndex, startPoint.Position);
                // highlight.SetPosition(endPositionIndex, startPoint.Position);
            }
            else if (Input.GetMouseButton(0) && startPoint != null)
            {
                Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                // RaycastHit2D hit = GetHit();
                if (hit)
                {
                    endPoint = hit.collider.gameObject.GetComponent<Point>();
                }
                highlight.SetPosition(endPositionIndex, mousePos);

                if (startPoint == endPoint || endPoint == null) return;

                if (IsStartAdd())
                {
                    currentId = endPoint.Id;
                    edges[new Vector2Int(startPoint.Id, endPoint.Id)].Add();
                    startPoint = endPoint;
                    UpdateHighlightPosition();
                    // SetHighlightPosition(startPoint.Position, endPoint.Position);
                }
                else if (IsEndAdd())
                {
                    currentId = endPoint.Id;
                    edges[new Vector2Int(startPoint.Id, endPoint.Id)].Add();
                    CheckWin();
                    startPoint = endPoint;
                    UpdateHighlightPosition();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                highlight.gameObject.SetActive(false);
                startPoint = null;
                endPoint = null;
                CheckWin();
            }
        }

        private RaycastHit2D GetHit()
        {
            Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return Physics2D.Raycast(mousePos, Vector2.zero);
        }

        // private void SetHighlightPosition(Vector2 startPosition, Vector2 endPosition)
        // {
        //     highlight.SetPosition(startPositionIndex, startPosition);
        //     highlight.SetPosition(endPositionIndex, endPosition);
        // }

        private void UpdateHighlightPosition()
        {
            highlight.SetPosition(startPositionIndex, startPoint.Position);
            highlight.SetPosition(endPositionIndex, endPoint.Position);
        }

        private bool IsStartAdd()
        {
            if (currentId != -1) return false;
            Vector2Int edge = new Vector2Int(startPoint.Id, endPoint.Id);
            if (!edges.ContainsKey(edge)) return false;
            return true;
        }

        private bool IsEndAdd()
        {
            if (currentId != startPoint.Id) return false;
            Vector2Int edge = new Vector2Int(endPoint.Id, startPoint.Id);
            if (edges.TryGetValue(edge, out Edge result))
            {
                if (result.isFilled) return false;
            }
            else
            {
                return false;
            }
            return true;
        }

        private void SetCamera()
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            cameraPosition.x = level.Columns / cameraPositionController;
            cameraPosition.y = level.Rows / cameraPositionController;
            Camera.main.transform.position = cameraPosition;

            Camera.main.orthographicSize = Mathf.Max(level.Columns, level.Rows) * cameraSizeController;
        }

        private void CheckWin()
        {
            foreach (var item in edges)
            {
                if (!item.Value.isFilled)
                {
                    return;
                }
            }
            hasGameFinished = true;
        }

        private IEnumerator GameFinished()
        {
            yield return new WaitForSeconds(sceneReloadDelay);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}