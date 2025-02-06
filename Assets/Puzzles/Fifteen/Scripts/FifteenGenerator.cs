using UnityEngine;
using Random = UnityEngine.Random;

namespace Fifteen
{
    public class Generator : MonoBehaviour
    {
        private int maxSize = 4;
        private int minSize = 0;
        private Vector2 lastPosition;

        [SerializeField] private int shuffleTimes = 6;

        private void Start()
        {
            for (int i = 0; i < shuffleTimes; i++)
            {
                Shuffle();
            }
        }

        public void Shuffle()
        {
            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    if (GameManager.Instance.boxes[i, j].isEmpty())
                    {
                        Vector2 position = getValidNumberBoxPosition(i, j);
                        GameManager.Instance.Swap(i, j, (int)position.x, (int)position.y);
                    }
                }
            }
        }

        private Vector2 getValidNumberBoxPosition(int x, int y)
        {
            Vector2 position = new Vector2();

            do
            {
                int randomNumber = Random.Range(minSize, maxSize);
                switch (randomNumber)
                {
                    case 0:
                        position = Vector2.left;
                        break;
                    case 1:
                        position = Vector2.right;
                        break;
                    case 2:
                        position = Vector2.up;
                        break;
                    case 3:
                    default:
                        position = Vector2.down;
                        break;
                }
            } while (!(IsValidRange(x + (int)position.x) && IsValidRange(y + (int)position.y)) || isOppositePosition(position));
            // does not exceed the acceptable range

            lastPosition = position;
            return position;
        }

        private bool IsValidRange(int randomNumber)
        {
            return randomNumber >= minSize + 1 && randomNumber <= maxSize - 1;
        }

        private bool isOppositePosition(Vector2 position)
        {
            return position *-1 == lastPosition;
        }
    }
}