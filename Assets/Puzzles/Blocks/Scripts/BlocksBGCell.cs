using UnityEngine;

namespace Blocks
{
    public class BGCell : MonoBehaviour
    {
        [HideInInspector] public bool isBlocked, isFilled;

        [SerializeField] private SpriteRenderer bgSprite;
        [SerializeField] private Sprite emptySprite, blockedSprite;
        [SerializeField] private Color startColor, correctColor, incorrectColor;

        public void Init(int blockValue)
        {
            isBlocked = blockValue == -1;
            if (isBlocked)
            {
                isFilled = true;
            }
            bgSprite.sprite = isBlocked ? blockedSprite : emptySprite;
        }

        public void ResetHighLight()
        {
            bgSprite.color = startColor;
        }

        public void UpdateHighlight(bool isCorrect)
        {
            bgSprite.color = isCorrect ? correctColor : incorrectColor;
        }
    }
}