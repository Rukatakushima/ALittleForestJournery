using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private GameObject _point, _topEdge, _bottomEdge, _leftEdge, _rightEdge;
        private Dictionary<Node, GameObject> ConnectedEdges;
        [HideInInspector] public int colorId;
        // public bool IsWin = false;
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
                { return true; }
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
        private List<Vector2Int> directionCheck = new List<Vector2Int>()
        { Vector2Int.up,Vector2Int.left,Vector2Int.down,Vector2Int.right };
        public bool IsDegreeThree(List<Node> resultNodes)
        {
            bool isdegreethree = false;

            int numOfNeighbours = 0;

            for (int i = 0; i < directionCheck.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2Int checkingPos = Pos2D + directionCheck[(i + j) % directionCheck.Count];

                    if (GameManager.Instance._nodeGrid.TryGetValue(checkingPos, out Node result))
                    {
                        if (resultNodes.Contains(result))
                        {
                            numOfNeighbours++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (numOfNeighbours == 3)
                {
                    break;
                }

                numOfNeighbours = 0;
            }

            if (numOfNeighbours >= 3)
            {
                isdegreethree = true;
            }

            return isdegreethree;
        }

        public void Init()
        {
            _point.SetActive(false);
            _topEdge.SetActive(false);
            _bottomEdge.SetActive(false);
            _leftEdge.SetActive(false);
            _rightEdge.SetActive(false);
            ConnectedEdges = new Dictionary<Node, GameObject>();
            ConnectedNodes = new List<Node>();
        }
        public void SetColorForPoint(int colorIdForSpawnedNode)
        {
            this.colorId = colorIdForSpawnedNode;
            _point.SetActive(true);
            _point.GetComponent<SpriteRenderer>().color =
            GameManager.Instance.NodeColors[colorId];
        }
        public void SetEdge(Vector2Int offset, Node node)
        {
            if (offset == Vector2Int.up)
            {
                ConnectedEdges[node] = _topEdge;
                return;
            }
            if (offset == Vector2Int.down)
            {
                ConnectedEdges[node] = _bottomEdge;
                return;
            }
            if (offset == Vector2Int.right)
            {
                ConnectedEdges[node] = _rightEdge;
                return;
            }
            if (offset == Vector2Int.left)
            {
                ConnectedEdges[node] = _leftEdge;
                return;
            }
        }
        public void UpdateInput(Node connectedNode)
        {
            //Неверный ввод
            if (!ConnectedEdges.ContainsKey(connectedNode))
            {
                return;
            }
            // IsWin = CheckIsWin();

            //Уже сущетсвующий node: удалить edge и части
            if (ConnectedNodes.Contains(connectedNode))
            {
                DeleteStartingNode(connectedNode);
            }

            // start node имеет 2 edges
            if (ConnectedNodes.Count == 2)
            {
                Node tempNode = ConnectedNodes[0];

                if (tempNode.IsConnectedToEndNode())
                {
                    tempNode = ConnectedNodes[1];
                }
                DeleteStartingNode(tempNode);
            }

            // end node имеет 2 edges
            if (connectedNode.ConnectedNodes.Count == 2)
            {

                DeleteConnectedNode(connectedNode);
            }

            // другой start node (другого цвета) и connected node имеет 1 edge => убирается node (окрашенный в изначальный цвет edge) 
            if (connectedNode.ConnectedNodes.Count == 1 && connectedNode.colorId != colorId)
            {
                DeleteConnectedNode(connectedNode);
            }

            //end node с уже существующим 1 edge
            if (ConnectedNodes.Count == 1 && IsEndNode)
            {
                DeleteStartingNode(ConnectedNodes[0]);
            }

            //одсоединяемся к EndNode, у которого уже есть edge
            //удаляем его edge
            if (connectedNode.ConnectedNodes.Count == 1 && connectedNode.IsEndNode)
            {
                DeleteConnectedNode(connectedNode);
            }

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
            GameObject connectedEdge = ConnectedEdges[connectedNode];
            connectedEdge.SetActive(true);
            connectedEdge.GetComponent<SpriteRenderer>().color =
            GameManager.Instance.NodeColors[colorId];
        }
        private void RemoveEdge(Node node)
        {
            GameObject edge = ConnectedEdges[node];
            edge.SetActive(false);
            edge = node.ConnectedEdges[this];
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