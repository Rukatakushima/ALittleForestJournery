using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NumberLink
{
    public class Cell : MonoBehaviour
    {
        [HideInInspector]
        public int Number
        {
            get => number;
            set
            {
                number = value;
                numberText.text = number.ToString();
                if (number == 0)
                {
                    cellSprite.color = solvedColor;
                    numberText.gameObject.SetActive(false);
                }
                else if (number < 0)
                {
                    cellSprite.color = overloadedColor;
                    numberText.gameObject.SetActive(false);
                }
                else
                {
                    cellSprite.color = defaultColor;
                    numberText.gameObject.SetActive(true);
                }
            }

        }

        [HideInInspector] public int row;
        [HideInInspector] public int column;

        [SerializeField] private TMP_Text numberText;
        [SerializeField] private SpriteRenderer cellSprite;
        [SerializeField] private Color defaultColor, solvedColor, overloadedColor;

        [SerializeField] private GameObject right1, right2, top1, top2, left1, left2, bottom1, bottom2;

        private int number;
        private Dictionary<int, Dictionary<int, GameObject>> edges;
        private Dictionary<int, int> edgeCounts;
        private Dictionary<int, Cell> connectedCell;

        private const int RIGHT = 0;
        private const int TOP = 1;
        private const int LEFT = 2;
        private const int BOTTOM = 3;

        public void Init(int row, int col, int num)
        {
            Number = num;
            this.row = row;
            column = col;

            edgeCounts = new()
            {
                { RIGHT, 0 },
                { LEFT, 0 },
                { TOP, 0 },
                { BOTTOM, 0 }
            };

            connectedCell = new()
            {
                { LEFT, null },
                { TOP, null },
                { BOTTOM, null },
                { RIGHT, null }
            };

            edges = new Dictionary<int, Dictionary<int, GameObject>>
            {
                { RIGHT, new Dictionary<int, GameObject> { { 1, right1 }, { 2, right2 } } },
                { TOP, new Dictionary<int, GameObject> { { 1, top1 }, { 2, top2 } } },
                { LEFT, new Dictionary<int, GameObject> { { 1, left1 }, { 2, left2 } } },
                { BOTTOM, new Dictionary<int, GameObject> { { 1, bottom1 }, { 2, bottom2 } } }
            };
        }

        public void Init()
        {
            for (int i = 0; i < 4; i++)
            {
                connectedCell[i] = GameManager.Instance.GetAdjacentCell(row, column, i);
                if (connectedCell[i] == null) continue;

                Vector2Int edgeOffset = new Vector2Int(connectedCell[i].row - row, connectedCell[i].column - column);
                float edgeSize = Mathf.Abs(edgeOffset.x) > Mathf.Abs(edgeOffset.y) ? Mathf.Abs(edgeOffset.x) : Mathf.Abs(edgeOffset.y);
                edgeSize *= GameManager.Instance.EdgeSize;

                SpriteRenderer singleEdge = edges[i][1].GetComponentInChildren<SpriteRenderer>();
                SpriteRenderer[] doubleEdges = edges[i][2].GetComponentsInChildren<SpriteRenderer>();

                ChangeSpriteSize(singleEdge, edgeSize);
                foreach (var item in doubleEdges)
                {
                    ChangeSpriteSize(item, edgeSize);
                }
            }

            right1.SetActive(false);
            right2.SetActive(false);
            bottom1.SetActive(false);
            bottom2.SetActive(false);
            left1.SetActive(false);
            left2.SetActive(false);
            top1.SetActive(false);
            top2.SetActive(false);
        }

        public void AddEdge(int direction)
        {
            if (connectedCell[direction] == null) return;

            if (edgeCounts[direction] == 2)
            {
                RemoveEdge(direction);
                return;
            }

            edgeCounts[direction]++;
            Number--;

            edges[direction][1].SetActive(false);
            edges[direction][2].SetActive(false);
            edges[direction][edgeCounts[direction]].SetActive(true);
        }

        public void RemoveEdge(int direction)
        {
            if (connectedCell[direction] == null || edgeCounts[direction] == 0) return;

            edgeCounts[direction]--;
            Number++;

            edges[direction][1].SetActive(false);
            edges[direction][2].SetActive(false);

            if (edgeCounts[direction] != 0)
                edges[direction][edgeCounts[direction]].SetActive(true);
        }

        public void RemoveAllEdges()
        {
            for (int i = 0; i < 4; i++)
            {
                RemoveEdge(i);
            }
        }

        private void ChangeSpriteSize(SpriteRenderer sprite, float size) => sprite.size = new Vector2(sprite.size.x, size);

        public bool IsValidCell(Cell cell, int direction) => connectedCell[direction] == cell;
    }
}