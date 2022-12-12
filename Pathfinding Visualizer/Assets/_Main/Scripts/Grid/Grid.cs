using System.Collections;
using UnityEngine;
using DG.Tweening;


public class Grid : MonoBehaviour
{
    // Fields

    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private Vector2 _gridDimension;

    Vector2Int _cellCount;
    Vector2 _cellDimension;
    Cell[,] _cellArray;


    // Properties

    public static Grid grid { get; set; }
    public bool isGridReady { get; private set; }
    public Vector2Int cellCount { get { return _cellCount; } }


    // Public methods

    public Cell GetCell(Vector2Int index)
    {
        if (isInside(index))
            return _cellArray[index.x, index.y];
        return null;
    }

    public Cell GetCell(Vector2 worldPosition)
    {
        var index = convertToXY(worldPosition);
        return GetCell(index);
    }

    public Cell[,] GetCellArray()
    {
        return _cellArray;
    }


    // Private methods

    private bool isInside(Vector2Int index)
    {
        if (index.x >= 0 && index.x < cellCount.x && index.y >= 0 && index.y < cellCount.y)
            return true;
        return false;
    }

    private Vector2Int convertToXY(Vector2 worldPosition)
    {
        var origin = gridOrigin();
        int x = Mathf.FloorToInt((worldPosition - origin).x / _cellDimension.x);
        int y = Mathf.FloorToInt((worldPosition - origin).y / _cellDimension.y);

        return new Vector2Int(x, y);
    }

    private Vector2 computeCellOrigin(Vector2Int index)
    {
        return new Vector2(index.x * _cellDimension.x, index.y * _cellDimension.y) + gridOrigin() + (_cellDimension / 2);
    }

    private Vector2 gridOrigin()
    {
        return (Vector2)transform.position - new Vector2(cellCount.x * _cellDimension.x / 2, cellCount.y * _cellDimension.y / 2);
    }


    // Lifecycle methods

    private void Awake()
    {
        if (grid == null)
            grid = this;

        float aspectRatio = _gridDimension.x / _gridDimension.y;

        // Set the ratio as constant
        int y = 28;
        int x = Mathf.FloorToInt(y * aspectRatio);

        _cellCount = new Vector2Int(x, y);
        _cellDimension = new Vector2(_gridDimension.x / cellCount.x, _gridDimension.y / cellCount.y);
        DOTween.SetTweensCapacity(6720, 50);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        Gizmos.color = Color.red;

        Vector2 corner = gridOrigin() + new Vector2(cellCount.x * _cellDimension.x, cellCount.y * _cellDimension.y);

        // Horizontals
        Gizmos.DrawLine(gridOrigin(), new Vector2(corner.x, gridOrigin().y));
        Gizmos.DrawLine(new Vector2(gridOrigin().x, corner.y), corner);

        // Verticals
        Gizmos.DrawLine(gridOrigin(), new Vector2(gridOrigin().x, corner.y));
        Gizmos.DrawLine(new Vector2(corner.x, gridOrigin().y), corner);

        // Horizontal lines
        for (int h = 1; h < cellCount.y; ++h)
            Gizmos.DrawLine
                (
                    gridOrigin() + (Vector2.up * h * _cellDimension.y),
                    new Vector2(corner.x, gridOrigin().y) + (Vector2.up * h * _cellDimension.y)
                );

        // Vertical lines
        for (int w = 1; w < cellCount.x; ++w)
            Gizmos.DrawLine
                (
                    gridOrigin() + (Vector2.right * w * _cellDimension.x),
                    new Vector2(gridOrigin().x, corner.y) + (Vector2.right * w * _cellDimension.x)
                );
    }

    private void OnValidate()
    {
        float aspectRatio = _gridDimension.x / _gridDimension.y;

        // Set the ratio as constant
        int y = 28;
        int x = Mathf.FloorToInt(y * aspectRatio);

        _cellCount = new Vector2Int(x, y);
        _cellDimension = new Vector2(_gridDimension.x / cellCount.x, _gridDimension.y / cellCount.y);
    }

    private void Start()
    {
        // intialize cells
        _cellArray = new Cell[cellCount.x, cellCount.y];
        var t = transform;

        for (int x = 0; x < cellCount.x; ++x)
            for (int y = 0; y < cellCount.y; ++y)
            {
                var cell = Instantiate(_cellPrefab, t);
                var origin = computeCellOrigin(new Vector2Int(x, y));

                cell.gameObject.name = $"cell_{x}\'{y}";
                cell.transform.position = origin;
                cell.transform.localScale = _cellDimension;
                cell.cellSize = _cellDimension.x;

                cell.index = new Vector2Int(x, y);
                cell.cellOrigin = origin;
                cell.canWalk = true;

                cell.gCost = int.MaxValue;
                cell.hCost = int.MaxValue;
                cell.costMultiplier = 1;
                cell.fromNode = null;

                _cellArray[x, y] = cell;
            }

        isGridReady = true;
    }
}
