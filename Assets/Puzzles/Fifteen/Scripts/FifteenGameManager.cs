using UnityEngine;
using ObjectsPool;

namespace Fifteen
{
    public class GameManager : MonoBehaviour
    {
        public NumberBox numberBoxPrefab;
        public NumberBox[,] numberBoxes = new NumberBox[4, 4];
        public Sprite[] sprites;


        public NumberBox Preload() => Instantiate(numberBoxPrefab);
        public void GetAction(NumberBox numberBox) => numberBox.gameObject.SetActive(true);
        public void ReturnAction(NumberBox numberBox) => numberBox.gameObject.SetActive(false);
        public PoolBase<NumberBox> numberBoxPrefabObjectPool;

        public float cameraSizeController = 0.1f;
        public float cameraPositionController = 0.375f;

        private void Awake()
        {
            numberBoxPrefabObjectPool = new PoolBase<NumberBox>(Preload, GetAction, ReturnAction, numberBoxes.Length);
            // cameraSizeController = numberBoxPrefab.transform.localScale.x;
        }

        private void Start()
        {
            Init();
            SetCamera();
        }

        private void Init()
        {
            int index = 0;
            for (int j = 3; j >= 0; j--)
            {
                for (int i = 0; i < 4; i++)
                {
                    NumberBox box = numberBoxPrefabObjectPool.GetFromPool();
                    box.gameObject.transform.position = new Vector2(i, j);
                    box.Init(i, j, index + 1, sprites[index], Swap);
                    numberBoxes[i, j] = box;
                    index++;
                }
            }
        }

        private void Swap(int x, int y)
        {
            int dx = getDx(x, y);
            int dy = getDy(x, y);

            var from = numberBoxes[x, y];
            var target = numberBoxes[x + dx, y + dy];

            numberBoxes[x, y] = target;
            numberBoxes[x + dx, y + dy] = from;

            from.UpdatePos(x + dx, y + dy);
            target.UpdatePos(x, y);
        }

        private int getDx(int x, int y)
        {
            if ((x < 3) && numberBoxes[x + 1, y].isEmpty())
            {
                return 1;
            }

            if ((x > 0) && numberBoxes[x - 1, y].isEmpty())
            {
                return -1;
            }

            return 0;
        }

        private int getDy(int x, int y)
        {
            if (y < 3 && numberBoxes[x, y + 1].isEmpty())
            {
                return 1;
            }

            if (y > 0 && numberBoxes[x, y - 1].isEmpty())
            {
                return -1;
            }

            return 0;
        }

        private void SetCamera()
        {
            float columns = 4;
            float rows = 4;

            Camera.main.orthographicSize = Mathf.Max(columns, rows) * cameraSizeController;
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = rows * cameraPositionController;
            camPos.y = columns * cameraPositionController;
            Camera.main.transform.position = camPos;
        }
    }
}