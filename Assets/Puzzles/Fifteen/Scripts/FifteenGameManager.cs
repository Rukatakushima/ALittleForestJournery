using UnityEngine;
using ObjectsPool;
using System.Collections;

namespace Fifteen
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public Box[,] boxes = new Box[4, 4];
        [SerializeField] private Sprite[] sprites;

        [SerializeField] private Box boxPrefab;
        private Box Preload() => Instantiate(boxPrefab);
        private void GetAction(Box box) => box.gameObject.SetActive(true);
        private void ReturnAction(Box box) => box.gameObject.SetActive(false);
        private PoolBase<Box> boxPrefabObjectPool;

        [SerializeField] private float cameraSizeController = 0.1f;
        [SerializeField] private float cameraPositionController = 0.375f;

        // public bool hasGameFinished;

        private void Awake()
        {
            Instance = this;
            // hasGameFinished = false;

            boxPrefabObjectPool = new PoolBase<Box>(Preload, GetAction, ReturnAction, boxes.Length);
            Init();
            SetCamera();
        }

        /*
                private void OnValidate()
                {
                    if (initInIspector && boxPrefabObjectPool == null)
                    {
                        boxPrefabObjectPool = new PoolBase<NumberBox>(Preload, GetAction, ReturnAction, boxes.Length);
                        Init();
                    }
                }
        */

        private void Init()
        {
            int index = 0;
            for (int j = 3; j >= 0; j--)
            {
                for (int i = 0; i < boxes.GetLength(1); i++)
                {
                    if (boxes[i, j] == null)
                    {
                        Box box = boxPrefabObjectPool.GetFromPool();
                        box.gameObject.transform.position = new Vector2(i, j);
                        box.Init(i, j, index + 1, sprites[index], ClickToSwap);
                        boxes[i, j] = box;
                        index++;
                    }
                }
            }
        }

        private void ClickToSwap(int x, int y)
        {
            int xDirection = getDirectionX(x, y);
            int yDirection = getDirectionY(x, y);
            Swap(x, y, xDirection, yDirection);
        }

        public void Swap(int x, int y, int xDirection, int yDirection)
        {
            var from = boxes[x, y];
            var target = boxes[x + xDirection, y + yDirection];

            boxes[x, y] = target;
            boxes[x + xDirection, y + yDirection] = from;

            from.UpdatePos(x + xDirection, y + yDirection);
            target.UpdatePos(x, y);
        }

        private int getDirectionX(int x, int y)
        {
            if ((x < 3) && boxes[x + 1, y].isEmpty())
            {
                return 1;
            }

            if ((x > 0) && boxes[x - 1, y].isEmpty())
            {
                return -1;
            }

            return 0;
        }

        private int getDirectionY(int x, int y)
        {
            if (y < 3 && boxes[x, y + 1].isEmpty())
            {
                return 1;
            }

            if (y > 0 && boxes[x, y - 1].isEmpty())
            {
                return -1;
            }

            return 0;
        }

        private void SetCamera()
        {
            float columns = boxes.GetLength(0);
            float rows = boxes.GetLength(1);

            Camera.main.orthographicSize = Mathf.Max(columns, rows) * cameraSizeController;
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = rows * cameraPositionController;
            camPos.y = columns * cameraPositionController;
            Camera.main.transform.position = camPos;
        }

        public void CheckAllBoxesCorrectPosition()
        {
            for (int i = 0; i < boxes.GetLength(0); i++)
            {
                for (int j = 0; j < boxes.GetLength(1); j++)
                {
                    if (!boxes[i, j].isInCorrectPosition)
                    {
                        // hasGameFinished = false;
                        return;
                    }
                }
            }

            // hasGameFinished = true;
            StartCoroutine(GameFinished());
        }

        private IEnumerator GameFinished()
        {
            yield return new WaitForSeconds(1f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}