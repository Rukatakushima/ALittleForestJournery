using ObjectsPool;
using UnityEngine;

namespace Fifteen
{
    public class LevelSpawner : BaseLevelSpawner<LevelData>
    {
        [SerializeField] private Box boxPrefab;
        [SerializeField] private Sprite[] sprites;
        
        private Vector2 _lastPosition;
        private const int MAX_SIZE = 4;
        private const int MIN_SIZE = 0;
        private PoolBase<Box> _boxPrefabObjectPool;


        private bool IsValidRange(int randomNumber) => randomNumber is >= MIN_SIZE + 1 and <= MAX_SIZE - 1;
        private bool IsOppositePosition(Vector2 position) => position * -1 == _lastPosition;
        private Box Preload() => Instantiate(boxPrefab);
        private void GetAction(Box box) => box.gameObject.SetActive(true);
        private void ReturnAction(Box box) => box.gameObject.SetActive(false);

        public override void SpawnLevel()
        {
            Init();
            ShuffleLevel();
            OnLevelSpawned?.Invoke();
        }

        private void Init()
        {
            _boxPrefabObjectPool = new PoolBase<Box>(Preload, GetAction, ReturnAction, GameManager.Instance.Boxes.Length);

            int index = 0;
            for (int j = 3; j >= 0; j--)
            {
                for (int i = 0; i < GameManager.Instance.Boxes.GetLength(1); i++)
                {
                    if (GameManager.Instance.Boxes[i, j] != null) continue;
                    Box box = _boxPrefabObjectPool.GetFromPool();
                    box.gameObject.transform.position = new Vector2(i, j);
                    box.Init(i, j, index + 1, sprites[index]);
                    GameManager.Instance.Boxes[i, j] = box;
                    index++;
                }
            }
        }

        private void ShuffleLevel()
        {
            for (int i = 0; i < level.shuffleTimes; i++)
            {
                Shuffle();
            }
        }

        private void Shuffle()
        {
            for (int i = 0; i < MAX_SIZE; i++)
            {
                for (int j = 0; j < MAX_SIZE; j++)
                {
                    if (GameManager.Instance.Boxes[i, j].IsEmpty())
                    {
                        Vector2 position = GetValidNumberBoxPosition(i, j);
                        GameManager.Instance.SwapBoxes(i, j, (int)position.x, (int)position.y);
                    }
                }
            }
        }

        private Vector2 GetValidNumberBoxPosition(int x, int y)
        {
            Vector2 position;

            do
            {
                int randomNumber = Random.Range(MIN_SIZE, MAX_SIZE);
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
            } while (!(IsValidRange(x + (int)position.x) && IsValidRange(y + (int)position.y)) || IsOppositePosition(position));

            _lastPosition = position;
            return position;
        }
    }
}