using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fifteen
{
    public class NumberBox : MonoBehaviour
    {
        public int index = 0;
        private int x = 0;
        private int y = 0;
        public bool isEmpty()
        {
            return index == 16;
        }

        private Action<int, int> swapFunc = null;

        public void Init(int i, int j, int index, Sprite sprite, Action<int, int> swapFunc)
        {
            this.index = index;
            UpdatePos(i, j);
            ChangeSprite(sprite);
            this.swapFunc = swapFunc;
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
            this.gameObject.transform.position = new Vector2(i, j); //localPosition
        }

        public void ChangeSprite(Sprite sprite)
        {
            this.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}