using System.Collections.Generic;
using UnityEngine;
using ObjectsPool;

namespace Blocks
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer blockPrefab;
        [SerializeField] private List<Color> blockColors;
        [SerializeField] private float blockSpawnSize;

        public Vector2 startPos, prevPos, curPos;
        private List<SpriteRenderer> blockSpriteRenderers;
        private List<Vector2Int> blockPositions;

        private const int TOP = 1;
        private const int BUTTON = 1;

        public SpriteRenderer PreloadSpriteRenderer() => Instantiate(blockPrefab);
        public void GetAction(SpriteRenderer blockPrefab) => blockPrefab.gameObject.SetActive(true);
        public void ReturnAction(SpriteRenderer blockPrefab) => blockPrefab.gameObject.SetActive(false);
        public PoolBase<SpriteRenderer> blockPrefabObjectPool;
        [SerializeField] private int blockPrefabPreloadCount = 10;

        private void Awake()
        {
            blockPrefabObjectPool = new PoolBase<SpriteRenderer>(PreloadSpriteRenderer, GetAction, ReturnAction, blockPrefabPreloadCount);
        }

        public void CreatePools()
        {
            blockPrefabObjectPool = new PoolBase<SpriteRenderer>(PreloadSpriteRenderer, GetAction, ReturnAction, blockPrefabPreloadCount);

        }
        public void Init(List<Vector2Int> blocks, Vector2 start, int blockNum)
        {
            startPos = start;
            prevPos = start;
            curPos = start;
            blockPositions = blocks;
            blockSpriteRenderers = new List<SpriteRenderer>();
            for (int i = 0; i < blockPositions.Count; i++)
            {
                SpriteRenderer spawnedBlock = blockPrefabObjectPool.GetFromPool();
                spawnedBlock.transform.SetParent(transform);
                spawnedBlock.name = i.ToString() + ") SpriteRenderer";
                spawnedBlock.color = blockColors[blockNum + 1];
                spawnedBlock.transform.localPosition = new Vector2(blockPositions[i].y, blockPositions[i].x);
                blockSpriteRenderers.Add(spawnedBlock);
            }
            transform.localScale = Vector2.one * blockSpawnSize;
            ElevateSprites(true);
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

        public List<Vector2Int> BlockPositions()
        {
            List<Vector2Int> result = new();
            foreach (var pos in blockPositions)
            {
                result.Add(pos + new Vector2Int(Mathf.FloorToInt(curPos.y), Mathf.FloorToInt(curPos.x)));
            }
            return result;
        }

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
            curPos.x = Mathf.FloorToInt(curPos.x) + GameManager.Instance.bgCellPositionRate;
            curPos.y = Mathf.FloorToInt(curPos.y) + GameManager.Instance.bgCellPositionRate;
            prevPos = curPos;
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            transform.position = curPos;
        }
    }
}