using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OneStroke
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;

        [SerializeField] private Edge mainEdge;
        
        private Dictionary<Vector2Int, Edge> _edges;
        private Point _startPoint, _endPoint;
        private Vector2 _previousHit;
        private int _currentId;
        private const int START_ID = -1;

        protected override void Awake()
        {
            Instance = this;
            _currentId = START_ID;
            base.Awake();
        }

        public void Initialize(Dictionary<Vector2Int, Edge> edges) => this._edges = edges;

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (!hit) return;

            _startPoint = hit.collider.gameObject.GetComponent<Point>();
            if (!_startPoint) return;

            if (_currentId == START_ID || (Vector2)hit.transform.position == _previousHit)
                mainEdge.SetFilledGradient();
            else
                mainEdge.SetWrongGradient();

            mainEdge.SetStartHighlight(_startPoint.Position);
        }

        protected override void HandleInputUpdate(Vector2 mousePosition)
        {
            if (!_startPoint) return;

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit) _endPoint = hit.collider.gameObject.GetComponent<Point>();

            mainEdge.SetEndPosition(mousePosition);

            if (_startPoint == _endPoint || _endPoint == null) return;

            if (IsStartFilled()) FillEdge();
            else if (IsEndFilled())
            {
                FillEdge();
                CheckWinCondition();
            }
        }

        protected override void HandleInputEnd()
        {
            mainEdge.TurnOff();

            _startPoint = null;
            _endPoint = null;

            CheckWinCondition();
        }

        private void FillEdge()
        {
            _currentId = _endPoint.Id;
            _edges[new Vector2Int(_startPoint.Id, _endPoint.Id)].FillEdge();
            _startPoint = _endPoint;
            mainEdge.UpdateHighlightPosition(_startPoint.Position);
            _previousHit = _endPoint.transform.position;
        }

        private bool IsStartFilled()
        {
            if (_currentId != START_ID) return false;

            Vector2Int edge = new Vector2Int(_startPoint.Id, _endPoint.Id);
            return _edges.ContainsKey(edge);
        }

        private bool IsEndFilled()
        {
            if (_currentId != _startPoint.Id) return false;

            Vector2Int edgeId = new Vector2Int(_endPoint.Id, _startPoint.Id);
            if (_edges.TryGetValue(edgeId, out Edge result))
                return result != null && !result.IsFilled;

            return false;
        }

        public override void CheckWinCondition()
        {
            if (_edges == null) return;
            if (_edges.Any(item => !item.Value.IsFilled)) return;
            OnWin?.Invoke();
        }
    }
}