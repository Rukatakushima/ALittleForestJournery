using UnityEngine;

namespace Blocks
{
    public class BGCell : MonoBehaviour
    {
        [HideInInspector] public bool isBlocked, isFilled;

        [SerializeField] private SpriteRenderer bgSprite;
        [SerializeField] private Color emptyColor, blockedColor, correctColor, incorrectColor;

        public void Init(int blockValue)
        {
            isBlocked = blockValue == -1;
            if (isBlocked)
            {
                isFilled = true;
            }
            bgSprite.color = isBlocked ? blockedColor : emptyColor;
        }

        public void ResetHighLight()
        {
            bgSprite.color = emptyColor;
        }

        public void UpdateHighlight(bool isCorrect)
        {
            bgSprite.color = isCorrect ? correctColor : incorrectColor;
        }
    }
}