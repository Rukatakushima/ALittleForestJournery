using System.Collections.Generic;
using UnityEngine;

namespace NumberLink
{
    public class CellFactory : MonoBehaviour
    {
        // private Cell cellPrefab;
        // private float cellSize;
        // private float cellGap;
        // public float EdgeSize => cellGap + cellSize;

        // private const int RIGHT = 0;
        // private const int TOP = 1;
        // private const int LEFT = 2;
        // private const int BOTTOM = 3;

        // public CellFactory(Cell cellPrefab, float cellSize, float cellGap)
        // {
        //     this.cellPrefab = cellPrefab;
        //     this.cellSize = cellSize;
        //     this.cellGap = cellGap;
        // }

        // public Cell CreateCell(int row, int column, int number, Vector2 position)
        // {
        //     Cell cell = Instantiate(cellPrefab, position, Quaternion.identity);
        //     InitializeCell(cell, row, column, number);
        //     cell.SetEdges(EdgeSize);
        //     // InitializeCellEdges(cell);
        //     // cell.InitData(row, column, number);
        //     return cell;
        // }

        // public void InitializeCell(Cell cell, int row, int column, int number)
        // {
        //     cell.Row = row;
        //     cell.Column = column;
        //     cell.Number = number;
        //     // cell.SetNumber(number);

        //     Dictionary<int, int> edgeCounts = new()
        //     {
        //         { RIGHT, 0 },
        //         { LEFT, 0 },
        //         { TOP, 0 },
        //         { BOTTOM, 0 }
        //     };
        //     cell.SetEdgeCounts(edgeCounts);

        //     Dictionary<int, Cell> connectedCell = new()
        //     {
        //         { RIGHT, null },
        //         { LEFT, null },
        //         { TOP, null },
        //         { BOTTOM, null }
        //     };
        //     cell.SetConnectedCells(connectedCell);

        //     // InitializeCellEdges(cell);
        // }
    }
}