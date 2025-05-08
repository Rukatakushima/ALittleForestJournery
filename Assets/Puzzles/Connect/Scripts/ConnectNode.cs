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
            if (connectedNode.ConnectedNodes.Count == 1 && connectedNode.IsEndNode)
            {
                Debug.Log("Удаление, если соединяемый узел — конечный, но уже связан");
                DeleteConnectedNode(connectedNode);
            }
            
            if (connectedNode.ConnectedNodes.Count > 0 && connectedNode.ColorId != ColorId)
            {
                // Debug.Log("Проверка несовместимости цветов");
                return;
            }
            
            if (!_connectedEdges.ContainsKey(connectedNode))
            {
                // Debug.Log("Нельзя соединять, если нет edge в нужном направлении");
                return;
            }
            
            if (_connectedNodes.Contains(connectedNode))
            {
                // Debug.Log("Клик на уже соединённый, но не последний — сброс");
                DeleteStartingNode(connectedNode);
                return;
            }
            
            if (_connectedNodes.Count > 0 && connectedNode == _connectedNodes[^1])
            {
                // Debug.Log("Шаг назад — удаляем только последнюю связь");
                RemoveLastConnection(connectedNode);
                return;
            }
            
            if (_connectedNodes.Count == 2)
            {
                // Debug.Log("Удаление соединения, если текущий узел уже имеет 2 связи");
                HandleStartingNodeWithTwoEdges();
            }
            
            if (_connectedNodes.Count == 1 && IsEndNode)
            {
                // Debug.Log("Удаление старого соединения с конца, если текущее — конечный узел");
                DeleteStartingNode(_connectedNodes[0]);
            }
            
            if (connectedNode.ConnectedNodes.Count == 2)
            {
                // Debug.Log("Удаление соединения у подключаемого узла, если он уже имеет 2 связи");
                connectedNode.DeleteNode();
            }
        
            // Debug.Log("Добавляем новое соединение");
            AddEdge(connectedNode);
        }
        
        private void RemoveLastConnection(Node node)
        {
            _connectedNodes.RemoveAt(_connectedNodes.Count - 1);
            node.RemoveConnectedNode(this);
            RemoveEdge(node);
        }

        private void HandleStartingNodeWithTwoEdges()
        {
            Node tempNode = _connectedNodes[0];
            if (tempNode == null) return;
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

            var connectedEdge = _connectedEdges[connectedNode];
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
        
            // Копия, чтобы избежать модификации во время итерации
            foreach (var connected in new List<Node>(_connectedNodes)) 
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