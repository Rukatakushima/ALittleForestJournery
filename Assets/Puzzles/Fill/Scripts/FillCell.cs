using UnityEngine;

namespace Fill
{
    public class Cell : MonoBehaviour
    {
        [HideInInspector] public bool Blocked;
        [HideInInspector] public bool Filled;
        [SerializeField] private Color _blockedColor;
        [SerializeField] private Color _emptyColor;
        [SerializeField] private Color _filledColor;
        [SerializeField] private SpriteRenderer _cellRender;

        public void Init(int fill)
        {
            Blocked = fill == 1;
            Filled = Blocked;
            _cellRender.color = Blocked ? _blockedColor : _emptyColor;
        }

        public void Add()
        {
            Filled = true;
            _cellRender.color = _filledColor;
        }
        public void Remove()
        {
            Filled = false;
            _cellRender.color = _emptyColor;
        }
        public void ChangeState()
        {
            Blocked = !Blocked;
            Filled = Blocked;
            _cellRender.color = Blocked ? _blockedColor : _emptyColor;
        }
    }
}
