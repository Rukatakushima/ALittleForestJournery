using System;
using System.Collections;
using UnityEngine;

namespace Fifteen
{
    public class Box : MonoBehaviour
    {
        public int index = 0;
        private int x = 0;
        private int y = 0;
        public bool isEmpty()
        {
            return index == 16;
        }
        private Action<int, int> swapFunc = null;
        public Vector3 rightPosition;
        // private Vector2 currentPosition;
        public bool isInCorrectPosition;

        [SerializeField] private float duration = 0.2f;

        private void Awake()
        {
            isInCorrectPosition = false;
        }

        public void Init(int i, int j, int index, Sprite sprite, Action<int, int> swapFunc)
        {
            this.index = index;
            // UpdatePos(i, j);
            x = i;
            y = j;
            ChangeSprite(sprite);
            this.swapFunc = swapFunc;
            this.name = index.ToString();
            rightPosition = this.transform.position;
            UpdateCorrectPosition();
        }

        private void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0) && swapFunc != null)
            {
                swapFunc(x, y);
            }
        }

        public void UpdatePos(int i, int j)
        {
            x = i;
            y = j;

            StartCoroutine(AnimateNumberBoxMove());
        }

        public void UpdateCorrectPosition()
        {
            if (rightPosition == this.transform.position)
            {
                isInCorrectPosition = true;
            }
            else
            {
                isInCorrectPosition = false;
            }
            //isInCorrectPosition = (rightPosition == this.transform.position);
        }

        private IEnumerator AnimateNumberBoxMove()
        {
            float elapsedTime = 0;

            Vector2 startPosition = this.gameObject.transform.position;
            Vector2 endPosition = new Vector2(x, y);

            while (elapsedTime < duration)
            {
                this.gameObject.transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            this.gameObject.transform.position = endPosition;
            UpdateCorrectPosition();
            GameManager.Instance.CheckAllBoxesCorrectPosition();
        }

        public void ChangeSprite(Sprite sprite)
        {
            this.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}