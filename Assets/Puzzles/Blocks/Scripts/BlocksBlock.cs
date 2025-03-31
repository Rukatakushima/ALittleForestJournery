using System.Collections.Generic;
using UnityEngine;
using ObjectsPool;

namespace Blocks
{
    public class Blocks : MonoBehaviour
    {
        #region SPAWN
        // [SerializeField] private SpriteRenderer blockSpritePrefab;
        // [SerializeField] private List<Color> blocksColors;
        // [SerializeField] private float blocksSpawnSize;

        // public SpriteRenderer PreloadSpriteRenderer() => Instantiate(blockSpritePrefab);
        // public void GetAction(SpriteRenderer blockSpritePrefab) => blockSpritePrefab.gameObject.SetActive(true);
        // public void ReturnAction(SpriteRenderer blockSpritePrefab) => blockSpritePrefab.gameObject.SetActive(false);
        // public PoolBase<SpriteRenderer> blockSpritePrefabObjectPool;

        // public void CreateMainBlock(List<Vector2Int> blocksPositions, Vector2 start, int blockNumber)
        // {
        //     startPos = start;
        //     prevPos = start;
        //     curPos = start;

        //     blockPositions = blocksPositions;
        //     CreateObjectPools();

        //     blockSpriteRenderers = new List<SpriteRenderer>();
        //     CreateChildrenBlocks(blockNumber);

        //     transform.localScale = Vector2.one * blocksSpawnSize;
        //     ElevateSprites(true);
        // }

        // private void CreateObjectPools()
        // {
        //     blockSpritePrefabObjectPool = new PoolBase<SpriteRenderer>(PreloadSpriteRenderer, GetAction, ReturnAction, blockPositions.Count);
        // }

        // private void CreateChildrenBlocks(int blockNumber)
        // {
        //     foreach (var pos in blockPositions)
        //     {
        //         SpriteRenderer spawnedBlock = blockSpritePrefabObjectPool.GetFromPool();
        //         spawnedBlock.name = "Child Block: " + blockNumber.ToString();
        //         spawnedBlock.transform.SetParent(transform);
        //         spawnedBlock.color = blocksColors[blockNumber + 1];
        //         spawnedBlock.transform.localPosition = new Vector2(pos.y, pos.x);
        //         blockSpriteRenderers.Add(spawnedBlock);
        //     }
        // }
        #endregion

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
            transform.localScale = Vector2.one * blocksSpawnSize;//new Vector2(blocksSpawnSize, blocksSpawnSize);
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