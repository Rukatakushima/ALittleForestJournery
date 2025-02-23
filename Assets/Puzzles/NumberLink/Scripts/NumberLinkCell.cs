using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NumberLink
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private SpriteRenderer cellSprite;

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

        [HideInInspector] public int rowCoordinate;
        [HideInInspector] public int columnCoordinate;

        [SerializeField] private Color defaultColor, solvedColor, overloadedColor;

        [SerializeField] private GameObject rightOneLink, rightTwoLinks, topOneLink, topTwoLinks, leftOneLink, leftTwoLinks, bottomOneLink, bottomTwoLinks;

        private int number;
        private Dictionary<int, Dictionary<int, GameObject>> links;
        private Dictionary<int, int> linksCounts;
        private Dictionary<int, Cell> linkedCell;

        private const int RIGHT = 0;
        private const int TOP = 1;
        private const int LEFT = 2;
        private const int BOTTOM = 3;

        private const int ZERO_LINKS = 0;
        private const int ONE_LINK = 1;
        private const int TWO_LINKS = 2;

        public const int LINK_DIRECTIONS = 4;

        public void InitializeCellData(int row, int column, int number)
        {
            Number = number;
            rowCoordinate = row;
            columnCoordinate = column;

            linksCounts = new()
            {
                { RIGHT, 0 },
                { LEFT, 0 },
                { TOP, 0 },
                { BOTTOM, 0 }
            };

            linkedCell = new()
            {
                { LEFT, null },
                { TOP, null },
                { BOTTOM, null },
                { RIGHT, null }
            };

            links = new Dictionary<int, Dictionary<int, GameObject>>
            {
                { RIGHT, new Dictionary<int, GameObject> { { ONE_LINK, rightOneLink }, { TWO_LINKS, rightTwoLinks } } },
                { TOP, new Dictionary<int, GameObject> { { ONE_LINK, topOneLink }, { TWO_LINKS, topTwoLinks } } },
                { LEFT, new Dictionary<int, GameObject> { { ONE_LINK, leftOneLink }, { TWO_LINKS, leftTwoLinks } } },
                { BOTTOM, new Dictionary<int, GameObject> { { ONE_LINK, bottomOneLink }, { TWO_LINKS, bottomTwoLinks } } }
            };
        }

        public void InitializeCell()
        {
            for (int i = 0; i < LINK_DIRECTIONS; i++)
            {
                linkedCell[i] = GameManager.Instance.GetAdjacentCell(rowCoordinate, columnCoordinate, i);
                if (linkedCell[i] == null) continue;

                Vector2Int linkOffset = new Vector2Int(linkedCell[i].rowCoordinate - rowCoordinate, linkedCell[i].columnCoordinate - columnCoordinate);
                float linkSize = Mathf.Abs(linkOffset.x) > Mathf.Abs(linkOffset.y) ? Mathf.Abs(linkOffset.x) : Mathf.Abs(linkOffset.y);
                linkSize *= GameManager.Instance.EdgeSize;

                SpriteRenderer singleLink = links[i][1].GetComponentInChildren<SpriteRenderer>();
                SpriteRenderer[] doubleLinks = links[i][2].GetComponentsInChildren<SpriteRenderer>();

                ChangeSpriteSize(singleLink, linkSize);
                foreach (var item in doubleLinks)
                {
                    ChangeSpriteSize(item, linkSize);
                }
            }

            rightOneLink.SetActive(false);
            rightTwoLinks.SetActive(false);
            bottomOneLink.SetActive(false);
            bottomTwoLinks.SetActive(false);
            leftOneLink.SetActive(false);
            leftTwoLinks.SetActive(false);
            topOneLink.SetActive(false);
            topTwoLinks.SetActive(false);
        }

        public void AddLink(int direction)
        {
            if (linkedCell[direction] == null) return;

            if (linksCounts[direction] == TWO_LINKS)
            {
                RemoveLink(direction);
                return;
            }

            linksCounts[direction]++;
            Number--;

            links[direction][ONE_LINK].SetActive(false);
            links[direction][TWO_LINKS].SetActive(false);
            links[direction][linksCounts[direction]].SetActive(true);
        }

        public void RemoveLink(int direction)
        {
            if (linkedCell[direction] == null || linksCounts[direction] == ZERO_LINKS) return;

            linksCounts[direction]--;
            Number++;

            links[direction][ONE_LINK].SetActive(false);
            links[direction][TWO_LINKS].SetActive(false);

            if (linksCounts[direction] != ZERO_LINKS)
                links[direction][linksCounts[direction]].SetActive(true);
        }

        public void RemoveAllLinks()
        {
            for (int i = 0; i < LINK_DIRECTIONS; i++)
            {
                RemoveLink(i);
            }
        }

        private void ChangeSpriteSize(SpriteRenderer sprite, float size) => sprite.size = new Vector2(sprite.size.x, size);

        public bool IsValidCell(Cell cell, int direction) => linkedCell[direction] == cell;
    }
}