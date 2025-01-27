using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class ConnectGameplayManager : MonoBehaviour
    {
        #region START_METHODS

        #region START_VARIABLES
        public static ConnectGameplayManager Instance;
        public bool hasGameFinished;

        private void Awake()
        {
            Instance = this;

            hasGameFinished = false;

            // CurrentLevelData = DefaultLevel;

            SpawnBoard();
            SpawnNodes();
        }

        #endregion

        #region BOARD_SPAWN

        [SerializeField] private SpriteRenderer _boardPrefab, _bgCellPrefab;
        [SerializeField] public int CurrentStage = 5;

        private void SpawnBoard()
        {
            int currentLevelSize = CurrentStage;

            //инициализация подложки: префаб, положение (делим размер на 2, чтобы разместить в середине), поворот
            var board = Instantiate(_boardPrefab,
                new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, 0f),
                Quaternion.identity);

            // добавлеяем к размеру "обводку\рамку"
            board.size = new Vector2(currentLevelSize + 0.1f, currentLevelSize + 0.1f);

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    Instantiate(_bgCellPrefab,
                        new Vector3(i + 0.5f, j + 0.5f, 0f),
                        Quaternion.identity);
                }
            }

            Camera.main.orthographicSize = currentLevelSize - 1f;// + 2f;
            Camera.main.transform.position = new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, -10f);

        }

        #endregion

        #region NODES_SPAWN
        [SerializeField] public ConnectLevelData CurrentLevelData;
        [SerializeField] private ConnectNode _nodePrefab;
        public List<ConnectNode> _nodes;//private
        public Dictionary<Vector2Int, ConnectNode> _nodeGrid;
        private void SpawnNodes()
        {
            _nodes = new List<ConnectNode>();
            _nodeGrid = new Dictionary<Vector2Int, ConnectNode>();
            int currentLevelSize = CurrentStage;
            ConnectNode spawnedNode;
            Vector3 spawnPos;

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    spawnPos = new Vector3(i + 0.5f, j + 0.5f, 0f);
                    spawnedNode = Instantiate(_nodePrefab, spawnPos, Quaternion.identity);
                    spawnedNode.Init();

                    int colorIdForSpawnedNode = GetColorId(i, j);
                    //окрас point в цвет, который будем соединять

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

            //установка сетки: сетка = nodes, у каждого node есть список offsetPos, т.е. 4 edge (верх, них, лево, право)
            List<Vector2Int> offsetPos = new List<Vector2Int>()
            { Vector2Int.up, Vector2Int.down,Vector2Int.left, Vector2Int.right};

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
            List<Edge> edges = CurrentLevelData.Connections;
            Vector2Int point = new Vector2Int(i, j);

            for (int colorId = 0; colorId < edges.Count; colorId++)
            {
                if (edges[colorId].StartPoint == point || edges[colorId].EndPoint == point) return colorId;
            }

            return -1;
        }

        #endregion

        #endregion

        #region UPDATE_METHODS

        private ConnectNode startNode;
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
                    if (hit && hit.collider.gameObject.TryGetComponent(out ConnectNode tNode)
                    && tNode.IsClickable)
                    {
                        startNode = tNode;
                    }

                    return;
                }

                if (hit && hit.collider.gameObject.TryGetComponent(out ConnectNode tempNode) && startNode != tempNode)
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

        #region WIN_CONDITIONS
        
        private void CheckWin()
        {
            bool IsWinning = true;

            foreach (var item in _nodes)
            {
                IsWinning &= item.IsWin;
                if(!IsWinning)
                {
                    return;
                }
            }

            hasGameFinished = true;
        }
        
        private IEnumerator GameFinished()
        {
            yield return new WaitForSeconds(1f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        #endregion
    }
}