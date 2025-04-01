using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Connect
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private GameObject point, topEdge, bottomEdge, leftEdge, rightEdge;
        private Dictionary<Node, GameObject> connectedEdges;
        private List<Node> connectedNodes;
        private SpriteRenderer pointRenderer;

        [HideInInspector] public int colorId { get; private set; }
        public Vector2Int Pos2D { get; set; }

        public bool IsWin => point.activeSelf ? connectedNodes.Count == 1 : connectedNodes.Count == 2;
        public bool IsClickable => point.activeSelf || connectedNodes.Count > 0;
        public bool IsEndNode => point.activeSelf;
        public IReadOnlyList<Node> ConnectedNodes => connectedNodes.AsReadOnly();

        public UnityEvent<Node> OnNodeConnected;
        public UnityEvent<Node> OnNodeDisconnected;

        private void Awake() => pointRenderer = point.GetComponent<SpriteRenderer>();

        public void Init()
        {
            point.SetActive(false);
            topEdge.SetActive(false);
            bottomEdge.SetActive(false);
            leftEdge.SetActive(false);
            rightEdge.SetActive(false);
            connectedEdges = new Dictionary<Node, GameObject>();
            connectedNodes = new List<Node>();
        }

        public void SetColorForPoint(int colorIdForSpawnedNode)
        {
            colorId = colorIdForSpawnedNode;
            point.SetActive(true);
            pointRenderer.color = GameManager.Instance.NodeColors[colorId];
        }

        public void SetEdge(Vector2Int offset, Node node)
        {
            switch ((offset.x, offset.y))
            {
                case (0, 1):
                    connectedEdges[node] = topEdge;
                    break;
                case (0, -1):
                    connectedEdges[node] = bottomEdge;
                    break;
                case (1, 0):
                    connectedEdges[node] = rightEdge;
                    break;
                case (-1, 0):
                    connectedEdges[node] = leftEdge;
                    break;
                default:
                    Debug.Log("Invalid direction vector at SetEdge");
                    break;
            }
        }

        public void UpdateInput(Node connectedNode)
        {
            if (!connectedEdges.ContainsKey(connectedNode)) return;

            if (connectedNodes.Contains(connectedNode))
            {
                DeleteStartingNode(connectedNode);
                return;
            }

            if (connectedNodes.Count == 2)
                HandleStartingNodeWithTwoEdges();

            if (connectedNode.ConnectedNodes.Count == 2)
                DeleteConnectedNode(connectedNode);

            if (connectedNode.ConnectedNodes.Count == 1 && connectedNode.colorId != colorId)
                DeleteConnectedNode(connectedNode);

            if (connectedNodes.Count == 1 && IsEndNode)
                DeleteStartingNode(connectedNodes[0]);

            if (connectedNode.ConnectedNodes.Count == 1 && connectedNode.IsEndNode)
                DeleteConnectedNode(connectedNode);

            AddEdge(connectedNode);
        }

        private void HandleStartingNodeWithTwoEdges()
        {
            Node tempNode = connectedNodes[0];
            if (tempNode.IsConnectedToEndNode())
                tempNode = connectedNodes[1];

            DeleteStartingNode(tempNode);
        }

        private void DeleteNodeConnection(Node node, bool isStartingNode)
        {
            if (isStartingNode)
            {
                connectedNodes.Remove(node);
                node.RemoveConnectedNode(this);
                RemoveEdge(node);
            }
            else
            {
                Node tempNode = node.ConnectedNodes[0];
                node.RemoveConnectedNode(tempNode);
                tempNode.RemoveConnectedNode(node);
                node.RemoveEdge(tempNode);
            }

            DeleteNode();

            if (!isStartingNode)
                node.DeleteNode();
        }

        private void DeleteStartingNode(Node connectedNode) => DeleteNodeConnection(connectedNode, true);

        private void DeleteConnectedNode(Node connectedNode) => DeleteNodeConnection(connectedNode, false);

        private void AddEdge(Node connectedNode)
        {
            connectedNode.colorId = colorId;
            connectedNode.AddConnectedNode(this);
            AddConnectedNode(connectedNode);

            GameObject connectedEdge = connectedEdges[connectedNode];
            connectedEdge.SetActive(true);
            connectedEdge.GetComponent<SpriteRenderer>().color = GameManager.Instance.NodeColors[colorId];

            OnNodeConnected?.Invoke(connectedNode);
        }

        private void RemoveEdge(Node node)
        {
            GameObject edge = connectedEdges[node];
            edge.SetActive(false);
            edge = node.connectedEdges[this];
            edge.SetActive(false);

            OnNodeDisconnected?.Invoke(node);
        }

        private void DeleteNode()
        {
            Node startNode = this;

            if (startNode.IsConnectedToEndNode()) return;

            while (startNode != null)
            {
                Node tempNode = null;
                if (startNode.connectedNodes.Count != 0)
                {
                    tempNode = startNode.connectedNodes[0];
                    startNode.connectedNodes.Clear();
                    tempNode.RemoveConnectedNode(startNode);
                    startNode.RemoveEdge(tempNode);
                }
                startNode = tempNode;
            }
        }

        public void AddConnectedNode(Node node) => connectedNodes.Add(node);

        public void RemoveConnectedNode(Node node) => connectedNodes.Remove(node);

        public bool IsConnectedToEndNode(List<Node> checkedNode = null)
        {
            if (checkedNode == null)
                checkedNode = new List<Node>();

            if (IsEndNode) return true;

            foreach (var item in ConnectedNodes)
            {
                if (!checkedNode.Contains(item))
                {
                    checkedNode.Add(item);
                    return item.IsConnectedToEndNode(checkedNode);
                }
            }

            return false;
        }
    }
}