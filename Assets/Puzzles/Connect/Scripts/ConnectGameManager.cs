using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;

        public List<Color> nodeColors;
        private List<Node> _nodes;
        private Node _startNode;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        public void Initialize(List<Node> nodes) => _nodes = nodes;

        protected override void HandleInputStart(Vector2 mousePosition) => _startNode = null;

        protected override void HandleInputUpdate(Vector2 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (_startNode == null)
            {
                if (hit && hit.collider.gameObject.TryGetComponent(out Node tNode) && tNode.IsClickable)
                    _startNode = tNode;
                return;
            }

            if (!hit || !hit.collider.gameObject.TryGetComponent(out Node tempNode) || _startNode == tempNode)
            {
                return;
            }

            if (_startNode.ColorId != tempNode.ColorId && tempNode.IsEndNode)
            {
                return;
            }

            _startNode.UpdateInput(tempNode);
            CheckWinCondition();
            _startNode = null;
        }

        protected override void HandleInputEnd() => _startNode = null;

        public override void CheckWinCondition()
        {
            bool isWinning = true;

            foreach (var item in _nodes)
            {
                isWinning &= item.IsWin;
                if (!isWinning) return;
            }

            OnWin?.Invoke();
        }
    }
}