using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    public class Blocks : MonoBehaviour
    {
        public Vector2 startPos, prevPos, curPos;
        public List<SpriteRenderer> blockSpriteRenderers;
        public List<Vector2Int> blockPositions;
        private float backgroundCellPositionRate;

        private const int TOP = 1;
        private const int BUTTON = 1;

        public void SetStartParametrs(Vector2 start, List<Vector2Int> blocksPositions, float blocksSpawnSize)
        {
            startPos = start;
            prevPos = start;
            curPos = start;

            blockPositions = blocksPositions;
            transform.localScale = Vector2.one * blocksSpawnSize;
            backgroundCellPositionRate = GameManager.Instance.backgroundCellPositionRate;

            blockSpriteRenderers = new List<SpriteRenderer>();
        }

        public void ElevateSprites(bool reverse = false)
        {
            foreach (var blockSprite in blockSpriteRenderers)
            {
                blockSprite.sortingOrder = reverse ? BUTTON : TOP;
            }
        }

        public void UpdatePos(Vector2 offset)
        {
            curPos += offset;
            UpdatePosition();
        }

        public List<Vector2Int> BlockPositions => blockPositions.ConvertAll(pos => pos + new Vector2Int(Mathf.FloorToInt(curPos.y), Mathf.FloorToInt(curPos.x)));

        public void UpdateIncorrectMove()
        {
            curPos = prevPos;
            UpdatePosition();
        }

        public void UpdateStartMove()
        {
            curPos = startPos;
            prevPos = startPos;
            UpdatePosition();
        }

        public void UpdateCorrectMove()
        {
            curPos.x = Mathf.FloorToInt(curPos.x) + backgroundCellPositionRate;
            curPos.y = Mathf.FloorToInt(curPos.y) + backgroundCellPositionRate;
            prevPos = curPos;
            UpdatePosition();
        }

        public void UpdatePosition() => transform.position = curPos;
    }
}