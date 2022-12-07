using System;
using System.Collections;
using UnityEngine;


public class Grid : MonoBehaviour
{
    // Fields

    public Cell cellPrefab;
    public Vector2Int gridDimension;
    public Vector2 cellDimension;

    Cell[,] cellArray;


    // Properties

    public bool isGridReady { get; private set; }


    // Public methods

    public Cell GetCell(Vector2Int index)
    {
        if (isInside(index))
            return cellArray[index.x, index.y];
        return null;
    }

    public Cell GetCell(Vector2 worldPosition)
    {
        var index = convertToXY(worldPosition);
        return GetCell(index);
    }

    public Cell[,] GetCellArray()
    {
        return cellArray;
    }


    // Private methods

    private bool isInside(Vector2Int index)
    {
        if (index.x >= 0 && index.x < gridDimension.x && index.y >= 0 && index.y < gridDimension.y)
            return true;
        return false;
    }

    private Vector2Int convertToXY(Vector2 worldPosition)
    {
        var origin = gridOrigin();
        int x = Mathf.FloorToInt((worldPosition - origin).x / cellDimension.x);
        int y = Mathf.FloorToInt((worldPosition - origin).y / cellDimension.y);

        return new Vector2Int(x, y);
    }

    private Vector2 computeCellOrigin(Vector2Int index)
    {
        return new Vector2(index.x * cellDimension.x, index.y * cellDimension.y) + gridOrigin() + (cellDimension / 2);
    }

    private Vector2 gridOrigin()
    {
        return (Vector2)transform.position - new Vector2(gridDimension.x * cellDimension.x / 2, gridDimension.y * cellDimension.y / 2);
    }


    // Lifecycle methods

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        Gizmos.color = Color.red;

        Vector2 corner = gridOrigin() + new Vector2(gridDimension.x * cellDimension.x, gridDimension.y * cellDimension.y);

        // Horizontals
        Gizmos.DrawLine(gridOrigin(), new Vector2(corner.x, gridOrigin().y));
        Gizmos.DrawLine(new Vector2(gridOrigin().x, corner.y), corner);

        // Verticals
        Gizmos.DrawLine(gridOrigin(), new Vector2(gridOrigin().x, corner.y));
        Gizmos.DrawLine(new Vector2(corner.x, gridOrigin().y), corner);

        // Horizontal lines
        for (int h = 1; h < gridDimension.y; ++h)
            Gizmos.DrawLine
                (
                    gridOrigin() + (Vector2.up * h * cellDimension.y),
                    new Vector2(corner.x, gridOrigin().y) + (Vector2.up * h * cellDimension.y)
                );

        // Vertical lines
        for (int w = 1; w < gridDimension.x; ++w)
            Gizmos.DrawLine
                (
                    gridOrigin() + (Vector2.right * w * cellDimension.x),
                    new Vector2(gridOrigin().x, corner.y) + (Vector2.right * w * cellDimension.x)
                );
    }

    private void OnValidate()
    {
        gridDimension = new Vector2Int(Mathf.Max(1, gridDimension.x), Mathf.Max(1, gridDimension.y));
        cellDimension = new Vector2(Mathf.Max(0.01f, cellDimension.x), Mathf.Max(0.01f, cellDimension.y));
    }

    private IEnumerator Start()
    {
        // intialize cells
        {
            cellArray = new Cell[gridDimension.x, gridDimension.y];
            var t = transform;

            for (int x = 0; x < gridDimension.x; ++x)
                for (int y = 0; y < gridDimension.y; ++y)
                {
                    var cell = Instantiate(cellPrefab, t);
                    var origin = computeCellOrigin(new Vector2Int(x, y));

                    cell.gameObject.name = $"cell_{x}\'{y}";
                    cell.transform.position = origin;

                    cellArray[x, y] = cell;
                    cellArray[x, y].index = new Vector2Int(x, y);
                    cellArray[x, y].cellOrigin = origin;
                    cellArray[x, y].isValid = true;

                    cellArray[x, y].gCost = int.MaxValue;
                    cellArray[x, y].hCost = int.MaxValue;
                    cellArray[x, y].costMultiplier = 1;
                    cellArray[x, y].fromNode = null;
                }
        }

        isGridReady = true;
        yield return null;
    }
}
