using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Fifteen
{
    public class Box : MonoBehaviour
    {
        public int index = 0;
        public int x = 0;
        public int y = 0;
        public bool isEmpty() => index == GameManager.Instance.boxes.Length;
        public Vector3 rightPosition;
        public bool isInCorrectPosition = false;

        // public UnityEvent winCheck;

        public void Init(int i, int j, int index, Sprite sprite)
        {
            this.index = index;
            x = i;
            y = j;
            ChangeSprite(sprite);
            name = index.ToString();
            rightPosition = transform.position;
            UpdateCorrectPosition();
        }

        public void UpdatePos(int i, int j)
        {
            x = i;
            y = j;

            StartCoroutine(AnimateNumberBoxMove());
        }

        public void UpdateCorrectPosition() => isInCorrectPosition = rightPosition == gameObject.transform.position;

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
            // winCheck?.Invoke();
            GameManager.Instance.CheckWinCondition();
        }

        public void ChangeSprite(Sprite sprite) => GetComponent<SpriteRenderer>().sprite = sprite;

        public void ClickToSwap()
        {
            int xDirection = GetDirectionX();
            int yDirection = GetDirectionY();

            if (xDirection != 0 || yDirection != 0)
            {
                GameManager.Instance.SwapBoxes(x, y, xDirection, yDirection);
            }
        }

        private int GetDirectionX()
        {
            if ((x < 3) && GameManager.Instance.boxes[x + 1, y].isEmpty())
            {
                return 1;
            }

            if ((x > 0) && GameManager.Instance.boxes[x - 1, y].isEmpty())
            {
                return -1;
            }

            return 0;
        }

        private int GetDirectionY()
        {
            if (y < 3 && GameManager.Instance.boxes[x, y + 1].isEmpty())
            {
                return 1;
            }

            if (y > 0 && GameManager.Instance.boxes[x, y - 1].isEmpty())
            {
                return -1;
            }

            return 0;
        }
    }
}