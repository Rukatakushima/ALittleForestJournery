using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;

        private Node startNode;
        public List<Node> nodes { get; private set; }
        public List<Color> NodeColors;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        public void Initialize(List<Node> nodes) => this.nodes = nodes;

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

        public override void CheckWinCondition()
        {
            bool IsWinning = true;

            foreach (var item in nodes)
            {
                IsWinning &= item.IsWin;
                if (!IsWinning) return;
            }

            OnWin?.Invoke();
        }
    }
}