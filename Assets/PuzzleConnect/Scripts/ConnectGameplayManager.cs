using Connect.Common;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Connect.Core
{
    public class ConnectGameplayManager : MonoBehaviour
    {
        #region START_METHODS
        #region START_VARIABLES
        public static ConnectGameplayManager Instance;
        [HideInInspector] public bool hasGameFinished;
        [SerializeField] private GameObject _winText;
        //[SerializeField] private SpriteRenderer _clickHighlight;

        private void Awake()
        {
            Instance = this;

            hasGameFinished = false;
            _winText.SetActive(false);

            CurrentLevelData = ConnectGameManager.Instance.GetLevel();

            SpawnBoard();

            SpawnNodes();
        }
        #endregion

        #region BOARD_SPAWN

        [SerializeField] private SpriteRenderer _boardPrefab, _bgCellPrefab;
        private void SpawnBoard()
        {
            int currentLevelSize = 4;//ConnectGameManager.Instance.CurrentStage + 4;

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

            Camera.main.orthographicSize = currentLevelSize - 1.5f;// + 2f;
            Camera.main.transform.position = new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, -10f);
        }

        #endregion

        #region NODES_SPAWN

        private ConnectLevelData CurrentLevelData;
        [SerializeField] private ConnectNode _nodePrefab;
        private List<ConnectNode> _nodes;

        public Dictionary<Vector2Int, ConnectNode> _nodeGrid;

        private void SpawnNodes()
        {
            _nodes = new List<ConnectNode>();
            _nodeGrid = new Dictionary<Vector2Int, ConnectNode>();

            int currentLevelSize = 4;//ConnectGameManager.Instance.CurrentStage + 4;
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

        public int GetColorId(int i,int j)
        {
            List<Edge> edges = CurrentLevelData.Edges;
            Vector2Int point = new Vector2Int(i,j);

            for (int colorId = 0; colorId < edges.Count; colorId++)
            {
                if (edges[colorId].StartPoint == point ||
                    edges[colorId].EndPoint == point)
                {
                    return colorId;
                }
            }

            return -1;
        }

        public Color GetHighlightColor(int colorID)
        {
            Color result = NodeColors[colorID];
            result.a = 0.4f; //Alpha component of the color (0 is transparent, 1 is opaque).
            return result;
        }

        #endregion
        #endregion

        #region UPDATE_METHODS
        #endregion

        #region WIN_CONDITIONS
        #endregion

        /*
                #region BUTTON_FUNCS
                #endregion
                */

    }
}
