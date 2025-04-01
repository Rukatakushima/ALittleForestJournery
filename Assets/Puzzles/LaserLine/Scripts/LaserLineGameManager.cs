using UnityEngine;

namespace LaserLine
{
    public class GameManager : BaseGameManager
    {
        void Start()
        {

        }

        void Update()
        {

        }


        protected override void HandleInputStart(Vector2 mousePosition) { }

        protected override void HandleInputUpdate(Vector2 mousePosition) { }

        protected override void HandleInputEnd() { }

        // public void CheckWinCondition()
        // {
        //     if (winConditionChecker != null)
        //         winConditionChecker.CheckWinCondition();
        //     else
        //         Debug.LogError("WinConditionChecker is not assigned!");
        // }

        public override void CheckWinCondition() { }
    }
}