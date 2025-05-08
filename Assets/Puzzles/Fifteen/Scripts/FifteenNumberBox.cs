using System.Collections;
using UnityEngine;

namespace Fifteen
{
    public class Box : MonoBehaviour
    {
        [SerializeField] private int index,x,y;
        [SerializeField] private Vector3 rightPosition;
        
        public bool IsInCorrectPosition { get; private set; }
        public bool IsEmpty() => index == GameManager.Instance.Boxes.Length;

        public void Init(int i, int j, int id, Sprite sprite)
        {
            index = id;
            x = i;
            y = j;
            ChangeSprite(sprite);
            rightPosition = transform.position;
            UpdateCorrectPosition();
        }

        public void UpdatePos(int i, int j)
        {
            x = i;
            y = j;
            StartCoroutine(AnimateNumberBoxMove());
        }

        private void UpdateCorrectPosition() => IsInCorrectPosition = rightPosition == gameObject.transform.position;

        private IEnumerator AnimateNumberBoxMove()
        {
            float elapsedTime = 0;

            Vector2 startPosition = gameObject.transform.position;
            Vector2 endPosition = new Vector2(x, y);

            while (elapsedTime < GameManager.Instance.boxMoveDuration)
            {
                gameObject.transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / GameManager.Instance.boxMoveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            gameObject.transform.position = endPosition;
            UpdateCorrectPosition();
            GameManager.Instance.CheckWinCondition();
        }

        private void ChangeSprite(Sprite sprite) => GetComponent<SpriteRenderer>().sprite = sprite;

        public void Swap()
        {
            int xDirection = GetDirectionX();
            int yDirection = GetDirectionY();

            if (xDirection != 0 || yDirection != 0)
                GameManager.Instance.SwapBoxes(x, y, xDirection, yDirection);
        }

        private int GetDirectionX()
        {
            return x switch
            {
                < 3 when GameManager.Instance.Boxes[x + 1, y].IsEmpty() => 1,
                > 0 when GameManager.Instance.Boxes[x - 1, y].IsEmpty() => -1,
                _ => 0
            };
        }

        private int GetDirectionY()
        {
            return y switch
            {
                < 3 when GameManager.Instance.Boxes[x, y + 1].IsEmpty() => 1,
                > 0 when GameManager.Instance.Boxes[x, y - 1].IsEmpty() => -1,
                _ => 0
            };
        }
    }
}