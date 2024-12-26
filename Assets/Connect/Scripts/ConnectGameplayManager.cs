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
        [SerializeField] private SpriteRenderer _clickHighlight;

        private void Awake()
        {
            Instance = this;

            hasGameFinished = false;
            _winText.SetActive(false);
            /*
                    #region BOARD_SPAWN

                        SpawnBoard();
                                #endregion



            */
        }
        #endregion

        #region BOARD_SPAWN

        [SerializeField] private SpriteRenderer _boardPrefab, _bgCellPregab;
        private void SpawnBoard()
        {
            int currentLevelSize = ConnectGameManager.Instance.CurrentStage + 4;
            var board = Instantiate(_boardPrefab,
            new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, 0f),
            Quaternion.identity);

            board.size = new Vector2(currentLevelSize + 0.08f, currentLevelSize + 0.08f);
        }

        #endregion

        #region NODES_SPAWN

        private ConnectLevelData CurrentLevelData;
        [SerializeField] private Node _nodePrefab;
        private List<Node> _nodes;

        public Dictionary<Vector2Int, Node> _nodGrid;

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
