using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private GameObject _point, _topEdge, _bottomEdge, _leftEdge, _rightEdge;
        private Dictionary<Node, GameObject> _connectedEdges;
        private List<Node> _connectedNodes;
        private SpriteRenderer _pointRenderer;

        [HideInInspector] public int colorId;
        public Vector2Int Pos2D { get; set; }

        public bool IsWin => _point.activeSelf ? _connectedNodes.Count == 1 : _connectedNodes.Count == 2;
        public bool IsClickable => _point.activeSelf || _connectedNodes.Count > 0;
        public bool IsEndNode => _point.activeSelf;
        public IReadOnlyList<Node> ConnectedNodes => _connectedNodes.AsReadOnly();

        public event System.Action<Node> OnNodeConnected;
        public event System.Action<Node> OnNodeDisconnected;

        private void Awake()
        {
            _pointRenderer = _point.GetComponent<SpriteRenderer>();
        }

        public void Init()
        {
            _point.SetActive(false);
            _topEdge.SetActive(false);
            _bottomEdge.SetActive(false);
            _leftEdge.SetActive(false);
            _rightEdge.SetActive(false);
            _connectedEdges = new Dictionary<Node, GameObject>();
            _connectedNodes = new List<Node>();
        }

        public void SetColorForPoint(int colorIdForSpawnedNode)
        {
            colorId = colorIdForSpawnedNode;
            _point.SetActive(true);
            _pointRenderer.color = GameManager.Instance.NodeColors[colorId];
        }

        public void SetEdge(Vector2Int offset, Node node)
        {
            switch ((offset.x, offset.y))
            {
                case (0, 1): // Vector2Int.up
                    _connectedEdges[node] = _topEdge;
                    break;
                case (0, -1): // Vector2Int.down
                    _connectedEdges[node] = _bottomEdge;
                    break;
                case (1, 0): //  Vector2Int.right
                    _connectedEdges[node] = _rightEdge;
                    break;
                case (-1, 0): // Vector2Int.left
                    _connectedEdges[node] = _leftEdge;
                    break;
                default:
                    Debug.Log("Invalid direction vector at SetEdge");
                    break;
            }
        }

        public void UpdateInput(Node connectedNode)
        {
            if (!_connectedEdges.ContainsKey(connectedNode))
                return;

            if (_connectedNodes.Contains(connectedNode))
            {
                DeleteStartingNode(connectedNode);
                return;
            }

            if (_connectedNodes.Count == 2)
            {
                HandleStartingNodeWithTwoEdges();
            }

            if (connectedNode.ConnectedNodes.Count == 2)
            {
                DeleteConnectedNode(connectedNode);
            }

            if (connectedNode.ConnectedNodes.Count == 1 && connectedNode.colorId != colorId)
            {
                DeleteConnectedNode(connectedNode);
            }

            if (_connectedNodes.Count == 1 && IsEndNode)
            {
                DeleteStartingNode(_connectedNodes[0]);
            }

            if (connectedNode.ConnectedNodes.Count == 1 && connectedNode.IsEndNode)
            {
                DeleteConnectedNode(connectedNode);
            }

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
            {
                node.DeleteNode();
            }
        }

        private void DeleteStartingNode(Node connectedNode)
        {
            DeleteNodeConnection(connectedNode, true);
        }

        private void DeleteConnectedNode(Node connectedNode)
        {
            DeleteNodeConnection(connectedNode, false);
        }

        private void AddEdge(Node connectedNode)
        {
            connectedNode.colorId = colorId;
            connectedNode.AddConnectedNode(this);
            AddConnectedNode(connectedNode);

            GameObject connectedEdge = _connectedEdges[connectedNode];
            connectedEdge.SetActive(true);
            connectedEdge.GetComponent<SpriteRenderer>().color = GameManager.Instance.NodeColors[colorId];

            OnNodeConnected?.Invoke(connectedNode);
        }

        private void RemoveEdge(Node node)
        {
            GameObject edge = _connectedEdges[node];
            edge.SetActive(false);
            edge = node._connectedEdges[this];
            edge.SetActive(false);

            OnNodeDisconnected?.Invoke(node);
        }

        private void DeleteNode()
        {
            Node startNode = this;

            if (startNode.IsConnectedToEndNode())
            {
                return;
            }

            while (startNode != null)
            {
                Node tempNode = null;
                if (startNode._connectedNodes.Count != 0)
                {
                    tempNode = startNode._connectedNodes[0];
                    startNode._connectedNodes.Clear();
                    tempNode.RemoveConnectedNode(startNode);
                    startNode.RemoveEdge(tempNode);
                }
                startNode = tempNode;
            }
        }

        public void AddConnectedNode(Node node)
        {
            _connectedNodes.Add(node);
        }

        public void RemoveConnectedNode(Node node)
        {
            _connectedNodes.Remove(node);
        }

        // public bool IsEndNode => _point.activeSelf;

        public bool IsConnectedToEndNode(List<Node> checkedNode = null)
        {
            if (checkedNode == null)
            {
                checkedNode = new List<Node>();
            }

            if (IsEndNode)
            {
                return true;
            }

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
/*
using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private GameObject _point, _topEdge, _bottomEdge, _leftEdge, _rightEdge;
        private Dictionary<Node, GameObject> _connectedEdges;
        [HideInInspector] public int colorId;

        public bool IsWin
        {
            get
            {
                if (_point.activeSelf)
                {
                    return ConnectedNodes.Count == 1;
                }

                return ConnectedNodes.Count == 2;
            }
        }

        public bool IsClickable
        {
            get
            {
                //если point существует, то IsClickable, а если нет, 
                //то тогда проверяем IsClickable наличием ConnectedNodes
                if (_point.activeSelf)
                    return true;
                return ConnectedNodes.Count > 0;
            }
        }

        public bool IsEndNode => _point.activeSelf;
        public Vector2Int Pos2D { get; set; }
        [HideInInspector] public List<Node> ConnectedNodes;

        public bool IsConnectedToEndNode(List<Node> checkedNode = null)
        {
            if (checkedNode == null)
            {
                checkedNode = new List<Node>();
            }

            if (IsEndNode)
            {
                return true;
            }

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

        public void Init()
        {
            _point.SetActive(false);
            _topEdge.SetActive(false);
            _bottomEdge.SetActive(false);
            _leftEdge.SetActive(false);
            _rightEdge.SetActive(false);
            _connectedEdges = new Dictionary<Node, GameObject>();
            ConnectedNodes = new List<Node>();
        }

        public void SetColorForPoint(int colorIdForSpawnedNode)
        {
            colorId = colorIdForSpawnedNode;
            _point.SetActive(true);
            _point.GetComponent<SpriteRenderer>().color =
            GameManager.Instance.NodeColors[colorId];
        }

        public void SetEdge(Vector2Int offset, Node node)
        {
            if (offset == Vector2Int.up)
            {
                _connectedEdges[node] = _topEdge;
                return;
            }
            if (offset == Vector2Int.down)
            {
                _connectedEdges[node] = _bottomEdge;
                return;
            }
            if (offset == Vector2Int.right)
            {
                _connectedEdges[node] = _rightEdge;
                return;
            }
            if (offset == Vector2Int.left)
            {
                _connectedEdges[node] = _leftEdge;
                return;
            }
        }

        public void UpdateInput(Node connectedNode)
        {
            //Неверный ввод
            if (!_connectedEdges.ContainsKey(connectedNode))
                return;

            //Уже сущетсвующий node: удалить edge и части
            if (ConnectedNodes.Contains(connectedNode))
                DeleteStartingNode(connectedNode);

            // start node имеет 2 edges
            if (ConnectedNodes.Count == 2)
            {
                Node tempNode = ConnectedNodes[0];

                if (tempNode.IsConnectedToEndNode())
                    tempNode = ConnectedNodes[1];
                DeleteStartingNode(tempNode);
            }

            // end node имеет 2 edges
            if (connectedNode.ConnectedNodes.Count == 2)
                DeleteConnectedNode(connectedNode);


            // другой start node (другого цвета) и connected node имеет 1 edge => убирается node (окрашенный в изначальный цвет edge) 
            if (connectedNode.ConnectedNodes.Count == 1 && connectedNode.colorId != colorId)
                DeleteConnectedNode(connectedNode);

            //end node с уже существующим 1 edge
            if (ConnectedNodes.Count == 1 && IsEndNode)
                DeleteStartingNode(ConnectedNodes[0]);

            //одсоединяемся к EndNode, у которого уже есть edge
            //удаляем его edge
            if (connectedNode.ConnectedNodes.Count == 1 && connectedNode.IsEndNode)
                DeleteConnectedNode(connectedNode);

            AddEdge(connectedNode);
        }

        private void DeleteStartingNode(Node connectedNode)
        {
            ConnectedNodes.Remove(connectedNode);
            connectedNode.ConnectedNodes.Remove(this);
            RemoveEdge(connectedNode);
            DeleteNode();
            connectedNode.DeleteNode();

        }

        private void DeleteConnectedNode(Node connectedNode)
        {
            Node tempNode = connectedNode.ConnectedNodes[0];
            connectedNode.ConnectedNodes.Remove(tempNode);
            tempNode.ConnectedNodes.Remove(connectedNode);
            connectedNode.RemoveEdge(tempNode);
            tempNode.DeleteNode();

        }

        private void AddEdge(Node connectedNode)
        {
            connectedNode.colorId = colorId;
            connectedNode.ConnectedNodes.Add(this);
            ConnectedNodes.Add(connectedNode);
            GameObject connectedEdge = _connectedEdges[connectedNode];
            connectedEdge.SetActive(true);
            connectedEdge.GetComponent<SpriteRenderer>().color =
            GameManager.Instance.NodeColors[colorId];
        }

        private void RemoveEdge(Node node)
        {
            GameObject edge = _connectedEdges[node];
            edge.SetActive(false);
            edge = node._connectedEdges[this];
            edge.SetActive(false);
        }

        private void DeleteNode()
        {
            Node startNode = this;

            if (startNode.IsConnectedToEndNode())
            {
                return;
            }

            while (startNode != null)
            {
                Node tempNode = null;
                if (startNode.ConnectedNodes.Count != 0)
                {
                    tempNode = startNode.ConnectedNodes[0];
                    startNode.ConnectedNodes.Clear();
                    tempNode.ConnectedNodes.Remove(startNode);
                    startNode.RemoveEdge(tempNode);
                }
                startNode = tempNode;
            }
        }
    }
}
*/