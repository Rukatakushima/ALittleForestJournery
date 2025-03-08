using UnityEngine;
using System;

namespace Fifteen
{
    public class GameManager : BaseGameManager<LevelSpawner, DefaultCameraController, WinConditionChecker, LevelData>
    {
        public static GameManager Instance;

        public Box[,] boxes = new Box[4, 4];
        public float boxMoveDuration = 0.2f;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        protected override void SetupManagers()
        {
            cameraController.SetupCamera(boxes.Length);
            levelSpawner.SpawnLevel();
            winConditionChecker.Initialize(boxes);
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
            {
                tBox.ClickToSwap();
            }
        }

        protected override void HandleInputUpdate(Vector2 mousePosition) { }

        protected override void HandleInputEnd() { }

        public void CheckWin()
        {
            CheckWinCondition();
        }
    }
}