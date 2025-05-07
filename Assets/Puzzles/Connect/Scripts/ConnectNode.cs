using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private GameObject point, topEdge, bottomEdge, leftEdge, rightEdge;
        
        private Dictionary<Node, GameObject> _connectedEdges;
        private List<Node> _connectedNodes;
        private SpriteRenderer _pointRenderer;

        public int ColorId { get; private set; }
        public bool IsWin => point.activeSelf ? _connectedNodes.Count == 1 : _connectedNodes.Count == 2;
        public bool IsClickable => point.activeSelf || _connectedNodes.Count > 0;
        public bool IsEndNode => point.activeSelf;
        private IReadOnlyList<Node> ConnectedNodes => _connectedNodes.AsReadOnly();

        private void Awake() => _pointRenderer = point.GetComponent<SpriteRenderer>();

        public void Init()
        {
            point.SetActive(false);
            topEdge.SetActive(false);
            bottomEdge.SetActive(false);
            leftEdge.SetActive(false);
            rightEdge.SetActive(false);
            _connectedEdges = new Dictionary<Node, GameObject>();
            _connectedNodes = new List<Node>();
        }

        public void SetColorForPoint(int colorIdForSpawnedNode)
        {
            ColorId = colorIdForSpawnedNode;
            point.SetActive(true);
            _pointRenderer.color = GameManager.Instance.nodeColors[ColorId];
        }

        public void SetEdge(Vector2Int offset, Node node)
        {
            switch ((offset.x, offset.y))
            {
                case (0, 1):
                    _connectedEdges[node] = topEdge;
                    break;
                case (0, -1):
                    _connectedEdges[node] = bottomEdge;
                    break;
                case (1, 0):
                    _connectedEdges[node] = rightEdge;
                    break;
                case (-1, 0):
                    _connectedEdges[node] = leftEdge;
                    break;
                default:
                    Debug.Log("Invalid direction vector at SetEdge");
                    break;
            }
        }

        public void UpdateInput(Node connectedNode)
        {
            if (!_connectedEdges.ContainsKey(connectedNode)) return;

            if (_connectedNodes.Contains(connectedNode))
            {
                DeleteStartingNode(connectedNode);
                return;
            }

            if (_connectedNodes.Count == 2)
                HandleStartingNodeWithTwoEdges();

            switch (connectedNode.ConnectedNodes.Count)
            {
                case 2:
                case 1 when connectedNode.ColorId != ColorId:
                    DeleteConnectedNode(connectedNode);
                    break;
            }

            if (_connectedNodes.Count == 1 && IsEndNode)
                DeleteStartingNode(_connectedNodes[0]);

            if (connectedNode.ConnectedNodes.Count == 1 && connectedNode.IsEndNode)
                DeleteConnectedNode(connectedNode);

            AddEdge(connectedNode);
        }

        private void HandleStartingNodeWithTwoEdges()
        {
            Node tempNode = _connectedNodes[0];
            if (tempNode.IsConnectedToEndNode())
                tempNode = _connectedNodes[1];

            DeleteStartingNode(tempNode);
        }

        private void DeleteNodeConnection(Node node, bool isStartingNode)
        {
            if (isStartingNode)
            {
                _connectedNodes.Remove(node);
                node.RemoveConnectedNode(this);
                RemoveEdge(node);
                node.DeleteNode();
            }
            else
            {
                node._connectedNodes.Remove(this);
                RemoveEdge(node);
                DeleteNode();
            }
        }

        private void DeleteStartingNode(Node connectedNode) => DeleteNodeConnection(connectedNode, true);

        private void DeleteConnectedNode(Node connectedNode) => DeleteNodeConnection(connectedNode, false);

        private void AddEdge(Node connectedNode)
        {
            connectedNode.ColorId = ColorId;
            connectedNode.AddConnectedNode(this);
            AddConnectedNode(connectedNode);

            GameObject connectedEdge = _connectedEdges[connectedNode];
            connectedEdge.SetActive(true);
            connectedEdge.GetComponent<SpriteRenderer>().color = GameManager.Instance.nodeColors[ColorId];
        }

        private void RemoveEdge(Node node)
        {
            GameObject edge = _connectedEdges[node];
            edge.SetActive(false);
            
            edge = node._connectedEdges[this];
            edge.SetActive(false);
        }

        private void DeleteNode(HashSet<Node> visitedNodes = null)
        {
            visitedNodes ??= new HashSet<Node>();

            if (!visitedNodes.Add(this)) return;

            foreach (var connected in new List<Node>(_connectedNodes)) // Копия, чтобы избежать модификации во время итерации
            {
                connected.RemoveConnectedNode(this);
                RemoveEdge(connected);
                connected.DeleteNode(visitedNodes);
            }

            _connectedNodes.Clear();
        }

        private void AddConnectedNode(Node node) => _connectedNodes.Add(node);

        private void RemoveConnectedNode(Node node) => _connectedNodes.Remove(node);

        private bool IsConnectedToEndNode(List<Node> checkedNode = null)
        {
            checkedNode ??= new List<Node>();

            if (IsEndNode) return true;

            foreach (var item in ConnectedNodes)
            {
                if (checkedNode.Contains(item)) continue;
                checkedNode.Add(item);
                return item.IsConnectedToEndNode(checkedNode);
            }

            return false;
        }
    }
}