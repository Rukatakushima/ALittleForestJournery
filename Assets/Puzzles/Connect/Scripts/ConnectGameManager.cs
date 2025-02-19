using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class GameManager : BaseGameManager<LevelSpawner, DefaultCameraController, WinConditionChecker, LevelData>
    {
        public static GameManager Instance;

        [SerializeField] private int levelSize = 5;
        private List<Node> _nodes;
        public Dictionary<Vector2Int, Node> _nodeGrid;
        public List<Color> NodeColors;
        private Node startNode;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        protected override void SetupManagers()
        {
            cameraController.SetupCamera(levelSize);

            levelSpawner.Initialize(levelSize);
            levelSpawner.SpawnLevel();

            winConditionChecker.Initialize(_nodes);
        }

        public void SetNodes(List<Node> _nodes, Dictionary<Vector2Int, Node> _nodeGrid)
        {
            this._nodes = _nodes;
            this._nodeGrid = _nodeGrid;
        }

        protected override void HandleMouseDown(Vector2 mousePosition) => startNode = null;

        protected override void HandleMouseDrag(Vector2 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (startNode == null)
            {
                if (hit && hit.collider.gameObject.TryGetComponent(out Node tNode) && tNode.IsClickable)
                    startNode = tNode;

                return;
            }

            if (hit && hit.collider.gameObject.TryGetComponent(out Node tempNode) && startNode != tempNode)
            {
                if (startNode.colorId != tempNode.colorId && tempNode.IsEndNode)
                    return;

                startNode.UpdateInput(tempNode);
                CheckWinCondition();
                startNode = null;
            }
        }

        protected override void HandleMouseUp() => startNode = null;
    }
}