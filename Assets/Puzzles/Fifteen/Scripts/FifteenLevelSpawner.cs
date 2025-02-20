using ObjectsPool;
using UnityEngine;

namespace Fifteen
{
    public class LevelSpawner : BaseLevelSpawner
    {
        [SerializeField] private int shuffleTimes = 6;
        private int maxSize = 4;
        private int minSize = 0;
        private Vector2 lastPosition;

        [SerializeField] private Box boxPrefab;
        [SerializeField] private Sprite[] sprites;

        private Box Preload() => Instantiate(boxPrefab);
        private void GetAction(Box box) => box.gameObject.SetActive(true);
        private void ReturnAction(Box box) => box.gameObject.SetActive(false);
        private PoolBase<Box> boxPrefabObjectPool;

        public override void SpawnLevel()
        {
            Init();
            ShuffleLevel();
        }

        private void Init()
        {
            boxPrefabObjectPool = new PoolBase<Box>(Preload, GetAction, ReturnAction, GameManager.Instance.boxes.Length);

            int index = 0;
            for (int j = 3; j >= 0; j--)
            {
                for (int i = 0; i < GameManager.Instance.boxes.GetLength(1); i++)
                {
                    if (GameManager.Instance.boxes[i, j] == null)
                    {
                        Box box = boxPrefabObjectPool.GetFromPool();
                        box.gameObject.transform.position = new Vector2(i, j);
                        box.Init(i, j, index + 1, sprites[index]);//, GameManager.Instance.swapFunc
                        GameManager.Instance.boxes[i, j] = box;
                        index++;
                    }
                }
            }
        }

        private void ShuffleLevel()
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
                        GameManager.Instance.SwapBoxes(i, j, (int)position.x, (int)position.y);
                    }
                }
            }
        }

        private Vector2 getValidNumberBoxPosition(int x, int y)
        {
            Vector2 position;

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
            return position * -1 == lastPosition;
        }
    }
}