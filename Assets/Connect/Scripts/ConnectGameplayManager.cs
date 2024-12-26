using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.TreeGrouper;
using Connect.Common;
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

            SpawnBoard();
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

            Camera.main.orthographicSize = currentLevelSize-1.5f;// + 2f;
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

        }

        public List<Color> NodeColors;

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
