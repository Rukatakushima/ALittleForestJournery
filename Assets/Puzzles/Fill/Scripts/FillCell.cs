using UnityEngine;

namespace Fill
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer cellRender;
        [SerializeField] private Color blockedColor, emptyColor, filledColor;
        
        [HideInInspector] public bool blocked, filled;

        public void Init(int fill)
        {
            blocked = fill == 1;
            filled = blocked;
            cellRender.color = blocked ? blockedColor : emptyColor;
        }

        public void Add()
        {
            filled = true;
            cellRender.color = filledColor;
        }
        
        public void Remove()
        {
            filled = false;
            cellRender.color = emptyColor;
        }
        
        public void ChangeState()
        {
            blocked = !blocked;
            filled = blocked;
            cellRender.color = blocked ? blockedColor : emptyColor;
        }
    }
}
