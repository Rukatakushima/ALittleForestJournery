using UnityEngine;

namespace Blocks
{
    public class BackgroundCell : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bgSprite;
        public Color emptyColor, blockedColor, correctColor, incorrectColor;
        
        public bool IsBlocked { get; private set; }
        public bool IsFilled { get; private set; }

        public void Init(int blockValue, Color empty, Color blocked, Color correct, Color incorrect)
        {
            emptyColor = empty;
            blockedColor = blocked; 
            correctColor = correct;
            incorrectColor = incorrect;
            
            IsBlocked = blockValue == -1;
            SetFilled(IsBlocked);
        }

        public void SetFilled(bool isFilled)
        {
            IsFilled = isFilled;
            bgSprite.color = IsFilled ? blockedColor : emptyColor;
        }

        public void ResetHighLight()
        {
            if (IsFilled || IsBlocked) return;
            bgSprite.color = emptyColor;
        }

        public void UpdateHighlight(bool isCorrect)
        {
            if (IsFilled || IsBlocked) return;
            bgSprite.color = isCorrect ? correctColor : incorrectColor;
        }
    }
}