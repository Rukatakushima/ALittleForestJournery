using UnityEngine;

namespace Fifteen
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;

        public Box[,] boxes { get; private set; } = new Box[4, 4];
        public float boxMoveDuration = 0.2f;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        public void SwapBoxes(int x, int y, int xDirection, int yDirection)
        {
            var from = boxes[x, y];
            var target = boxes[x + xDirection, y + yDirection];

            boxes[x, y] = target;
            boxes[x + xDirection, y + yDirection] = from;

            from.UpdatePos(x + xDirection, y + yDirection);
            target.UpdatePos(x, y);
        }

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit && hit.collider.gameObject.TryGetComponent(out Box tBox))
                tBox.ClickToSwap();
        }

        protected override void HandleInputUpdate(Vector2 mousePosition) { }

        protected override void HandleInputEnd() { }

        public override void CheckWinCondition()
        {
            for (int i = 0; i < boxes.GetLength(0); i++)
            {
                for (int j = 0; j < boxes.GetLength(1); j++)
                {
                    if (!boxes[i, j].isInCorrectPosition)
                    {
                        return;
                    }
                }
            }

            OnWin?.Invoke();
        }
    }
}