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
                if(_point.activeSelf) 
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

        public void SolveHighlight()
        {
            throw new NotImplementedException();
        }

    }
}