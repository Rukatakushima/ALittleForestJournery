using Connect.Common;
<<<<<<< HEAD
using System.Collections;
using System.Collections.Generic;
=======
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
>>>>>>> parent of cfd330e (Puzzle Connect Deleted unnessasary files)
using TMPro;
using UnityEngine;

namespace Connect.Core
{
    public class GameplayManager : MonoBehaviour
    {
        #region START_METHODS

        #region START_VARIABLES
<<<<<<< HEAD
        public static GameplayManager Instance;

        public bool hasGameFinished;
=======
        public static ConnectGameplayManager Instance;
        [HideInInspector] public bool hasGameFinished;
        [SerializeField] private GameObject _winText;
        //[SerializeField] private SpriteRenderer _clickHighlight;
>>>>>>> parent of cfd330e (Puzzle Connect Deleted unnessasary files)

        private void Awake()
        {
            Instance = this;

            hasGameFinished = false;
            _winText.SetActive(false);

<<<<<<< HEAD
=======
            CurrentLevelData = ConnectGameManager.Instance.GetLevel();

>>>>>>> parent of cfd330e (Puzzle Connect Deleted unnessasary files)
            SpawnBoard();

            SpawnNodes();

        }

        #endregion

        #region BOARD_SPAWN

        [SerializeField] private SpriteRenderer _boardPrefab, _bgCellPrefab;
<<<<<<< HEAD

        private void SpawnBoard()
        {
            int currentLevelSize = 5;
=======
        private void SpawnBoard()
        {
            int currentLevelSize = ConnectGameManager.Instance.CurrentStage + 4;
>>>>>>> parent of cfd330e (Puzzle Connect Deleted unnessasary files)

            var board = Instantiate(_boardPrefab,
                new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, 0f),
                Quaternion.identity);

            board.size = new Vector2(currentLevelSize + 0.08f, currentLevelSize + 0.08f);

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    Instantiate(_bgCellPrefab, new Vector3(i + 0.5f, j + 0.5f, 0f), Quaternion.identity);
                }
            }

            Camera.main.orthographicSize = currentLevelSize + 2f;
            Camera.main.transform.position = new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, -10f);
        }

        #endregion

        #region NODE_SPAWN

<<<<<<< HEAD
        [SerializeField] public ConnectLevelData CurrentLevelData;
        [SerializeField] private Node _nodePrefab;
        private List<Node> _nodes;
=======
        private ConnectLevelData CurrentLevelData;
        [SerializeField] private ConnectNode _nodePrefab;
        private List<ConnectNode> _nodes;
>>>>>>> parent of cfd330e (Puzzle Connect Deleted unnessasary files)

        public Dictionary<Vector2Int, Node> _nodeGrid;

        private void SpawnNodes()
        {
            _nodes = new List<Node>();
            _nodeGrid = new Dictionary<Vector2Int, Node>();

<<<<<<< HEAD
            int currentLevelSize = 5;
            Node spawnedNode;
=======
            int currentLevelSize = ConnectGameManager.Instance.CurrentStage + 4;
            ConnectNode spawnedNode;
>>>>>>> parent of cfd330e (Puzzle Connect Deleted unnessasary files)
            Vector3 spawnPos;

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    spawnPos = new Vector3(i + 0.5f, j + 0.5f, 0f);
                    spawnedNode = Instantiate(_nodePrefab, spawnPos, Quaternion.identity);
                    spawnedNode.Init();

                    int colorIdForSpawnedNode = GetColorId(i, j);

                    if (colorIdForSpawnedNode != -1)
                    {
                        spawnedNode.SetColorForPoint(colorIdForSpawnedNode);
                    }

                    _nodes.Add(spawnedNode);
                    _nodeGrid.Add(new Vector2Int(i, j), spawnedNode);
                    spawnedNode.gameObject.name = i.ToString() + j.ToString();
                    spawnedNode.Pos2D = new Vector2Int(i, j);

                }
            }

            List<Vector2Int> offsetPos = new List<Vector2Int>()
            {Vector2Int.up,Vector2Int.down,Vector2Int.left,Vector2Int.right };

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

        public List<Color> NodeColors;

        public int GetColorId(int i, int j)
        {
            List<Edge> edges = CurrentLevelData.Edges;
            Vector2Int point = new Vector2Int(i, j);

            for (int colorId = 0; colorId < edges.Count; colorId++)
            {
<<<<<<< HEAD
                if (edges[colorId].StartPoint == point ||
                    edges[colorId].EndPoint == point)
                {
                    return colorId;
                }
=======
                if (edges[colorId].StartPoint == point || edges[colorId].EndPoint == point) return colorId; 
>>>>>>> parent of cfd330e (Puzzle Connect Deleted unnessasary files)
            }

            return -1;
        }

<<<<<<< HEAD
        public Color GetHighLightColor(int colorID)
        {
            Color result = NodeColors[colorID % NodeColors.Count];
            result.a = 0.4f;
            return result;
        }

=======
        public Color GetHighlightColor(int colorID)
        {
            Color result = NodeColors[colorID];
            result.a = 0.4f; //Alpha component of the color (0 is transparent, 1 is opaque).
            return result;
        }
>>>>>>> parent of cfd330e (Puzzle Connect Deleted unnessasary files)

        #endregion

        #endregion

        #region UPDATE_METHODS

        private Node startNode;

        private void Update()
        {
            if (hasGameFinished) 
            {
                StartCoroutine(GameFinished());
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                startNode = null;
                return;
            }

            if (Input.GetMouseButton(0))
            {

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (startNode == null)
                {
                    if (hit && hit.collider.gameObject.TryGetComponent(out Node tNode)
                        && tNode.IsClickable)
                    {
                        startNode = tNode;
                    }

                    return;
                }

                if (hit && hit.collider.gameObject.TryGetComponent(out Node tempNode)
                    && startNode != tempNode)
                {
                    if (startNode.colorId != tempNode.colorId && tempNode.IsEndNode)
                    {
                        return;
                    }

                    startNode.UpdateInput(tempNode);
                    CheckWin();
                    startNode = null;
                }

                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                startNode = null;
            }

        }

        #endregion

        private void CheckWin()
        {
            bool IsWinning = true;
            // foreach (var item in _nodes)
            // {
            //     item.SolveHighlight();
            // }

            foreach (var item in _nodes)
            {
                item.SolveHighlight();
            }

            foreach (var item in _nodes)
            {
                IsWinning &= item.IsWin;
                if (!IsWinning)
                {
                    return;
                }
            }

            hasGameFinished = true;
            StartCoroutine(GameFinished());
        }
        private IEnumerator GameFinished()
        {
            yield return new WaitForSeconds(1f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
<<<<<<< HEAD
=======
        #endregion

        /*
                #region BUTTON_FUNCS
                #endregion
                */
>>>>>>> parent of cfd330e (Puzzle Connect Deleted unnessasary files)

    }
}