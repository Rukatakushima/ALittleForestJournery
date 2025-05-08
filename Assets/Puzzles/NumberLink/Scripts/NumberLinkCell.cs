using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NumberLink
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private SpriteRenderer cellSprite;
        [SerializeField] private Color defaultColor, solvedColor, overloadedColor;
        [SerializeField] private GameObject rightOneLink, rightTwoLinks, topOneLink, topTwoLinks, leftOneLink, leftTwoLinks, bottomOneLink, bottomTwoLinks;

        public readonly Dictionary<int, int> OppositeDirections = new()
        {
            { RIGHT_DIRECTION_ID, LEFT_DIRECTION_ID },
            { TOP_DIRECTION_ID, BOTTOM_DIRECTION_ID },
            { LEFT_DIRECTION_ID, RIGHT_DIRECTION_ID },
            { BOTTOM_DIRECTION_ID, TOP_DIRECTION_ID }
        };
        
        private int _rowCoordinate, _columnCoordinate, _number;
        private Dictionary<int, Dictionary<int, GameObject>> _links;
        private Dictionary<int, int> _linksCounts;
        private Dictionary<int, Cell> _linkedCell;
        
        private const int LINK_DIRECTIONS = 4;
        private const int RIGHT_DIRECTION_ID = 0;
        private const int TOP_DIRECTION_ID = 1;
        private const int LEFT_DIRECTION_ID = 2;
        private const int BOTTOM_DIRECTION_ID = 3;

        private const int ZERO_LINKS = 0;
        private const int ONE_LINK = 1;
        private const int TWO_LINKS = 2;
        private const int THREE_LINKS = 3;
        
        public int Number
        {
            get => _number;
            private set
            {
                _number = value;
                numberText.text = _number.ToString();
                switch (_number)
                {
                    case 0:
                        cellSprite.color = solvedColor;
                        numberText.gameObject.SetActive(false);
                        break;
                    case < 0:
                        cellSprite.color = overloadedColor;
                        numberText.gameObject.SetActive(false);
                        break;
                    default:
                        cellSprite.color = defaultColor;
                        numberText.gameObject.SetActive(true);
                        break;
                }
            }
        }
        
        public bool IsValidCell(Cell cell, int direction) => _linkedCell[direction] == cell;

        public void InitializeCellData(int row, int column, int num)
        {
            Number = num;
            _rowCoordinate = row;
            _columnCoordinate = column;

            _linksCounts = new()
        {
            { RIGHT_DIRECTION_ID, 0 },
            { LEFT_DIRECTION_ID, 0 },
            { TOP_DIRECTION_ID, 0 },
            { BOTTOM_DIRECTION_ID, 0 }
        };

            _linkedCell = new()
        {
            { LEFT_DIRECTION_ID, null },
            { TOP_DIRECTION_ID, null },
            { BOTTOM_DIRECTION_ID, null },
            { RIGHT_DIRECTION_ID, null }
        };

            _links = new Dictionary<int, Dictionary<int, GameObject>>
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
                _linkedCell[i] = GameManager.Instance.GetLinkedCell(_rowCoordinate, _columnCoordinate, i);
                if (_linkedCell[i] == null) continue;

                Vector2Int linkOffset = new Vector2Int(_linkedCell[i]._rowCoordinate - _rowCoordinate, _linkedCell[i]._columnCoordinate - _columnCoordinate);
                float linkSize = Mathf.Abs(linkOffset.x) > Mathf.Abs(linkOffset.y) ? Mathf.Abs(linkOffset.x) : Mathf.Abs(linkOffset.y);
                linkSize *= GameManager.Instance.EdgeSize;

                SpriteRenderer singleLink = _links[i][ONE_LINK].GetComponentInChildren<SpriteRenderer>();
                SpriteRenderer[] doubleLinks = _links[i][TWO_LINKS].GetComponentsInChildren<SpriteRenderer>();

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
            if (_linkedCell[direction] == null) return;

            if (_linksCounts[direction] == THREE_LINKS)
            {
                RemoveLink(direction);
                return;
            }

            _linksCounts[direction]++;
            Number--;

            DisplayLinks(direction, _linksCounts[direction]);
        }

        private void ToggleLinksDisplay(int direction, bool isActive)
        {
            _links[direction][TWO_LINKS].SetActive(isActive);
            _links[direction][ONE_LINK].SetActive(isActive);
        }

        private void DisplayLinks(int direction, int displayingLinksCount)
        {
            ToggleLinksDisplay(direction, false);

            switch (displayingLinksCount)
            {
                case ONE_LINK:
                case TWO_LINKS:
                    _links[direction][displayingLinksCount].SetActive(true);
                    break;
                case THREE_LINKS:
                    ToggleLinksDisplay(direction, true);
                    break;
            }
        }

        public void RemoveLink(int direction)
        {
            if (_linkedCell[direction] == null || _linksCounts[direction] == ZERO_LINKS) return;

            _linksCounts[direction]--;
            Number++;

            DisplayLinks(direction, _linksCounts[direction]);
        }

        public void RemoveAllLinks()
        {
            for (int i = 0; i < LINK_DIRECTIONS; i++)
            {
                // Удаляем все соединения на текущем направлении
                while (_linksCounts[i] > ZERO_LINKS)
                {
                    RemoveLink(i);
                }

                RemoveLinkFromLinkedCells(i);
            }
        }

        private void RemoveLinkFromLinkedCells(int linkedCellID)
        {
            if (_linkedCell[linkedCellID] != null)
            {
                int oppositeDirection = OppositeDirections[linkedCellID]; // Противоположное направление
                while (_linkedCell[linkedCellID]._linksCounts[oppositeDirection] > ZERO_LINKS)
                {
                    _linkedCell[linkedCellID].RemoveLink(oppositeDirection);
                }
            }
        }

        private void ChangeSpriteSize(SpriteRenderer sprite, float size) => sprite.size = new Vector2(sprite.size.x, size);
    }
}