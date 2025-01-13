using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Connect.Core
{
    public class ConnectNode : MonoBehaviour
    {
        [SerializeField] private GameObject _point;
        [SerializeField] private GameObject _topEdge;
        [SerializeField] private GameObject _bottomEdge;
        [SerializeField] private GameObject _leftEdge;
        [SerializeField] private GameObject _rightEdge;
        [SerializeField] private GameObject _highLight;

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

        public bool isClickable
        {
            get
            {
                //если point существует, то isClickable, а если нет, то тогда isClickable наличием ConnectedNodes
                if (_point.activeSelf)
                { return true; }

                return ConnectedNodes.Count > 0;
            }
        }

        public bool IsEndNode => _point.activeSelf;

        public Vector2Int Pos2D { get; set; }

        public void Init()
        {
            _point.SetActive(false);
            _topEdge.SetActive(false);
            _bottomEdge.SetActive(false);
            _leftEdge.SetActive(false);
            _rightEdge.SetActive(false);
            _highLight.SetActive(false);
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

        [HideInInspector] public List<ConnectNode> ConnectedNodes;

        public void UpdateInput(ConnectNode connectedNode)
        {
            //Неверный ввод
            if (!ConnectedEdges.ContainsKey(connectedNode))
            {
                return;
            }
            //уже сущетсвующий node
            //удалить edge и части
            if (ConnectedNodes.Contains(connectedNode))
            {
                ConnectedNodes.Remove(connectedNode);
                connectedNode.ConnectedNodes.Remove(this);
                RemoveEdge(connectedNode);
                DeleteNode();
                connectedNode.DeleteNode();
            }

            // start node имеет 2 edges
            if (ConnectedNodes.Count == 2)
            {
                ConnectNode tempNode = ConnectedNodes[0];

                if (tempNode.IsConnectedToEndNode())
                {
                    tempNode = ConnectedNodes[1];

                }
                ConnectedNodes.Remove(tempNode);
                tempNode.ConnectedNodes.Remove(this);
                RemoveEdge(tempNode);
                tempNode.DeleteNode();
            }

            // end node имеет 2 edges
            if (connectedNode.ConnectedNodes.Count == 2)
            {
                ConnectNode tempNode = connectedNode.ConnectedNodes[0];
                connectedNode.ConnectedNodes.Remove(tempNode);
                tempNode.ConnectedNodes.Remove(connectedNode);
                connectedNode.RemoveEdge(tempNode);
                tempNode.DeleteNode();

                tempNode = connectedNode.ConnectedNodes[0];
                connectedNode.ConnectedNodes.Remove(tempNode);
                tempNode.ConnectedNodes.Remove(connectedNode);
                connectedNode.RemoveEdge(tempNode);
                tempNode.DeleteNode();

            }

            AddEdge(connectedNode);
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

        public void SolveHighlight()
        {
        }

    }
}