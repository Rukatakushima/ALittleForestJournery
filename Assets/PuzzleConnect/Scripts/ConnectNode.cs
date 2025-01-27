using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    public class ConnectNode : MonoBehaviour
    {
        [SerializeField] private GameObject _point, _topEdge, _bottomEdge, _leftEdge, _rightEdge;
        private Dictionary<ConnectNode, GameObject> ConnectedEdges;
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
                { return true; }
                return ConnectedNodes.Count > 0;
            }
        }
        public bool IsEndNode => _point.activeSelf;
        public Vector2Int Pos2D { get; set; }
        [HideInInspector] public List<ConnectNode> ConnectedNodes;
        public bool IsConnectedToEndNode(List<ConnectNode> checkedNode = null)
        {
            if (checkedNode == null)
            {
                checkedNode = new List<ConnectNode>();
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
        public bool IsDegreeThree(List<ConnectNode> resultNodes)
        {
            bool isdegreethree = false;

            int numOfNeighbours = 0;

            for (int i = 0; i < directionCheck.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2Int checkingPos = Pos2D + directionCheck[(i + j) % directionCheck.Count];

                    if (ConnectGameplayManager.Instance._nodeGrid.TryGetValue(checkingPos, out ConnectNode result))
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
            ConnectedEdges = new Dictionary<ConnectNode, GameObject>();
            ConnectedNodes = new List<ConnectNode>();
        }
        public void SetColorForPoint(int colorIdForSpawnedNode)
        {
            this.colorId = colorIdForSpawnedNode;
            _point.SetActive(true);
            _point.GetComponent<SpriteRenderer>().color =
            ConnectGameplayManager.Instance.NodeColors[colorId];
        }
        public void SetEdge(Vector2Int offset, ConnectNode node)
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
        public void UpdateInput(ConnectNode connectedNode)
        {
            //Неверный ввод
            if (!ConnectedEdges.ContainsKey(connectedNode))
            {
                return;
            }

            //Уже сущетсвующий node: удалить edge и части
            if (ConnectedNodes.Contains(connectedNode))
            {
                DeleteStartingNode(connectedNode);
                // ConnectedNodes.Remove(connectedNode);
                // connectedNode.ConnectedNodes.Remove(this);
                // RemoveEdge(connectedNode);
                //DeleteNode();
                //connectedNode.DeleteNode();
            }

            // start node имеет 2 edges
            if (ConnectedNodes.Count == 2)
            {
                ConnectNode tempNode = ConnectedNodes[0];

                if (tempNode.IsConnectedToEndNode())
                {
                    tempNode = ConnectedNodes[1];

                }
                DeleteStartingNode(tempNode);
                // ConnectedNodes.Remove(tempNode);
                // tempNode.ConnectedNodes.Remove(this);
                // RemoveEdge(tempNode);
                // tempNode.DeleteNode();
            }

            // end node имеет 2 edges
            if (connectedNode.ConnectedNodes.Count == 2)
            {
                // DeleteConnectedNode(connectedNode);
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



            //чтобы edges одного не отрезали друг друга при пересечении от стартоых node
            //Dont allow Boxes

            /*

                        if (colorId != connectedNode.colorId)
                        {
                            Debug.Log("colorId != connectedNode.colorId");
                            return;
                        }
                        List<ConnectNode> checkingNodes = new List<ConnectNode>() { this };
                        List<ConnectNode> resultNodes = new List<ConnectNode>() { this };

                        while (checkingNodes.Count > 0)
                        {
                            foreach (var item in checkingNodes[0].ConnectedNodes)
                            {
                                if (!resultNodes.Contains(item))
                                {
                                    resultNodes.Add(item);
                                    checkingNodes.Add(item);
                                }
                            }
                            checkingNodes.Remove(checkingNodes[0]);
                        }

                        foreach (var item in resultNodes)
                        {
                            if (!item.IsEndNode && item.IsDegreeThree(resultNodes))
                            {
                                DeleteConnectedNode(item);
                                /*
                                ConnectNode tempNode = item.ConnectedNodes[0];
                                item.ConnectedNodes.Remove(tempNode);
                                tempNode.ConnectedNodes.Remove(item);
                                item.RemoveEdge(tempNode);
                                tempNode.DeleteNode();///

                                if (item.ConnectedNodes.Count == 0) return;
                                DeleteConnectedNode(item);
                                // tempNode = item.ConnectedNodes[0];
                                // item.ConnectedNodes.Remove(tempNode);
                                // tempNode.ConnectedNodes.Remove(item);
                                // item.RemoveEdge(tempNode);
                                // tempNode.DeleteNode();

                                return;
                            }
                        }*/
        }
        private void DeleteStartingNode(ConnectNode connectedNode)
        {
            ConnectedNodes.Remove(connectedNode);
            connectedNode.ConnectedNodes.Remove(this);
            RemoveEdge(connectedNode);
            DeleteNode();
            connectedNode.DeleteNode();

        }
        private void DeleteConnectedNode(ConnectNode connectedNode)
        {
            ConnectNode tempNode = connectedNode.ConnectedNodes[0];
            connectedNode.ConnectedNodes.Remove(tempNode);
            tempNode.ConnectedNodes.Remove(connectedNode);
            connectedNode.RemoveEdge(tempNode);
            tempNode.DeleteNode();

        }
        private void AddEdge(ConnectNode connectedNode)
        {
            connectedNode.colorId = colorId;
            connectedNode.ConnectedNodes.Add(this);
            ConnectedNodes.Add(connectedNode);
            GameObject connectedEdge = ConnectedEdges[connectedNode];
            connectedEdge.SetActive(true);
            connectedEdge.GetComponent<SpriteRenderer>().color =
            ConnectGameplayManager.Instance.NodeColors[colorId];
        }
        private void RemoveEdge(ConnectNode node)
        {
            GameObject edge = ConnectedEdges[node];
            edge.SetActive(false);
            edge = node.ConnectedEdges[this];
            edge.SetActive(false);
        }
        private void DeleteNode()
        {
            ConnectNode startNode = this;

            if (startNode.IsConnectedToEndNode())
            {
                return;
            }

            while (startNode != null)
            {
                ConnectNode tempNode = null;
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