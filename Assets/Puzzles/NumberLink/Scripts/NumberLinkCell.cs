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

        public const int LINK_DIRECTIONS = 4;
        private const int RIGHT_DIRECTION_ID = 0;
        private const int TOP_DIRECTION_ID = 1;
        private const int LEFT_DIRECTION_ID = 2;
        private const int BOTTOM_DIRECTION_ID = 3;

        private const int ZERO_LINKS = 0;
        private const int ONE_LINK = 1;
        private const int TWO_LINKS = 2;
        private const int THREE_LINKS = 3;

        public Dictionary<int, int> OppositeDirections = new()
        {
            { RIGHT_DIRECTION_ID, LEFT_DIRECTION_ID },
            { TOP_DIRECTION_ID, BOTTOM_DIRECTION_ID },
            { LEFT_DIRECTION_ID, RIGHT_DIRECTION_ID },
            { BOTTOM_DIRECTION_ID, TOP_DIRECTION_ID }
        };

        public void InitializeCellData(int row, int column, int number)
        {
            Number = number;
            rowCoordinate = row;
            columnCoordinate = column;

            linksCounts = new()
        {
            { RIGHT_DIRECTION_ID, 0 },
            { LEFT_DIRECTION_ID, 0 },
            { TOP_DIRECTION_ID, 0 },
            { BOTTOM_DIRECTION_ID, 0 }
        };

            linkedCell = new()
        {
            { LEFT_DIRECTION_ID, null },
            { TOP_DIRECTION_ID, null },
            { BOTTOM_DIRECTION_ID, null },
            { RIGHT_DIRECTION_ID, null }
        };

            links = new Dictionary<int, Dictionary<int, GameObject>>
        {
            { RIGHT_DIRECTION_ID, new Dictionary<int, GameObject> { { ONE_LINK, rightOneLink }, { TWO_LINKS, rightTwoLinks } } },
            { TOP_DIRECTION_ID, new Dictionary<int, GameObject> { { ONE_LINK, topOneLink }, { TWO_LINKS, topTwoLinks } } },
            { LEFT_DIRECTION_ID, new Dictionary<int, GameObject> { { ONE_LINK, leftOneLink }, { TWO_LINKS, leftTwoLinks } } },
            { BOTTOM_DIRECTION_ID, new Dictionary<int, GameObject> { { ONE_LINK, bottomOneLink }, { TWO_LINKS, bottomTwoLinks } } }
        };
        }

        public void InitializeCell()
        {
            for (int i = 0; i < LINK_DIRECTIONS; i++)
            {
                linkedCell[i] = GameManager.Instance.GetLinkedCell(rowCoordinate, columnCoordinate, i);
                if (linkedCell[i] == null) continue;

                Vector2Int linkOffset = new Vector2Int(linkedCell[i].rowCoordinate - rowCoordinate, linkedCell[i].columnCoordinate - columnCoordinate);
                float linkSize = Mathf.Abs(linkOffset.x) > Mathf.Abs(linkOffset.y) ? Mathf.Abs(linkOffset.x) : Mathf.Abs(linkOffset.y);
                linkSize *= GameManager.Instance.EdgeSize;

                SpriteRenderer singleLink = links[i][ONE_LINK].GetComponentInChildren<SpriteRenderer>();
                SpriteRenderer[] doubleLinks = links[i][TWO_LINKS].GetComponentsInChildren<SpriteRenderer>();

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

            if (linksCounts[direction] == THREE_LINKS)
            {
                RemoveLink(direction);
                return;
            }

            linksCounts[direction]++;
            Number--;

            DisplayLinks(direction, linksCounts[direction]);
        }

        private void ToggleLinksDisplay(int direction, bool isActive)
        {
            links[direction][TWO_LINKS].SetActive(isActive);
            links[direction][ONE_LINK].SetActive(isActive);
        }

        private void DisplayLinks(int direction, int displayingLinksCount)
        {
            ToggleLinksDisplay(direction, false);

            switch (displayingLinksCount)
            {
                case ONE_LINK:
                case TWO_LINKS:
                    links[direction][displayingLinksCount].SetActive(true);
                    break;
                case THREE_LINKS:
                    ToggleLinksDisplay(direction, true);
                    break;
            }
        }

        public void RemoveLink(int direction)
        {
            if (linkedCell[direction] == null || linksCounts[direction] == ZERO_LINKS) return;

            linksCounts[direction]--;
            Number++;

            DisplayLinks(direction, linksCounts[direction]);
        }

        public void RemoveAllLinks()
        {
            for (int i = 0; i < LINK_DIRECTIONS; i++)
            {
                // Удаляем все соединения на текущем направлении
                while (linksCounts[i] > ZERO_LINKS)
                {
                    RemoveLink(i);
                }

                RemoveLinkFromLinkedCells(i);
            }
        }

        public void RemoveLinkFromLinkedCells(int linkedCellID)
        {
            if (linkedCell[linkedCellID] != null)
            {
                int oppositeDirection = OppositeDirections[linkedCellID]; // Противоположное направление
                while (linkedCell[linkedCellID].linksCounts[oppositeDirection] > ZERO_LINKS)
                {
                    linkedCell[linkedCellID].RemoveLink(oppositeDirection);
                }
            }
        }

        private void ChangeSpriteSize(SpriteRenderer sprite, float size) => sprite.size = new Vector2(sprite.size.x, size);

        public bool IsValidCell(Cell cell, int direction) => linkedCell[direction] == cell;
    }
}