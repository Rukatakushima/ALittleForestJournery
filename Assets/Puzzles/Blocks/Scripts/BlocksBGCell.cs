using UnityEngine;

namespace Blocks
{
    public class BGCell : MonoBehaviour
    {
        [HideInInspector] public bool isBlocked, isFilled, isCorrect;

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

        public void ResetHightlight()
        {
            bgSprite.color = startColor;
        }

        public void UpdateHightlight()
        {
            bgSprite.color = isCorrect ? correctColor : incorrectColor;
        }
    }
}