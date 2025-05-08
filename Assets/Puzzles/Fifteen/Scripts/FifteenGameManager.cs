using UnityEngine;

namespace Fifteen
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;

        public float boxMoveDuration = 0.2f;
        public Box[,] Boxes { get; } = new Box[4, 4];

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        public void SwapBoxes(int x, int y, int xDirection, int yDirection)
        {
            var from = Boxes[x, y];
            var target = Boxes[x + xDirection, y + yDirection];

            Boxes[x, y] = target;
            Boxes[x + xDirection, y + yDirection] = from;

            from.UpdatePos(x + xDirection, y + yDirection);
            target.UpdatePos(x, y);
        }

        protected override void HandleInputStart(Vector2 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit && hit.collider.gameObject.TryGetComponent(out Box tBox))
                tBox.Swap();
        }

        protected override void HandleInputUpdate(Vector2 mousePosition) { }

        protected override void HandleInputEnd() { }

        public override void CheckWinCondition()
        {
            for (int i = 0; i < Boxes.GetLength(0); i++)
            {
                for (int j = 0; j < Boxes.GetLength(1); j++)
                {
                    if (!Boxes[i, j].IsInCorrectPosition)
                    {
                        return;
                    }
                }
            }

            OnWin?.Invoke();
        }
    }
}