using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class GameManager : BaseGameManager<LevelSpawner, /*DefaultCameraController, WinConditionChecker,*/ LevelData>
    {
        public static GameManager Instance;

        public List<Node> nodes { get; private set; }
        public Dictionary<Vector2Int, Node> nodeGrid;
        public List<Color> NodeColors;
        private Node startNode;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        // protected override void SetupManagers()
        // {
        //     cameraController.SetupCamera(levelSize);

        //     levelSpawner.Initialize(levelSize);
        //     levelSpawner.SpawnLevel();

        //     winConditionChecker.Initialize(nodes);
        // }

        public void Initialize(List<Node> nodes, Dictionary<Vector2Int, Node> nodeGrid)
        {
            this.nodes = nodes;
            this.nodeGrid = nodeGrid;
            levelSpawner.OnLevelSpawned?.Invoke();
        }

        protected override void HandleInputStart(Vector2 mousePosition) => startNode = null;

        protected override void HandleInputUpdate(Vector2 mousePosition)
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

        protected override void HandleInputEnd() => startNode = null;

        protected override void CheckWinCondition()
        {
            bool IsWinning = true;

            foreach (var item in nodes)
            {
                IsWinning &= item.IsWin;
                if (!IsWinning)
                {
                    return;
                }
            }

            OnWin?.Invoke();
        }
    }
}