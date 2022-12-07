using UnityEngine;
using System;


public class GridManager : MonoBehaviour
{
    Grid _grid;
    Cell _startingCell = null, _endingCell = null;
    event Action _onGridReset;


    public bool SetStartingPoint(Vector2 worldPosition)
    {
        var cell = _grid.GetCell(worldPosition);

        if (cell == null || (_endingCell != null && _endingCell.index == cell.index))
            return false;

        if (_startingCell != null)
        {
            _startingCell.sr.color = Color.white;
            _startingCell = null;
        }

        cell.sr.color = Color.green;
        _startingCell = cell;
        return true;
    }

    public bool SetEndingPoint(Vector2 worldPosition)
    {
        var cell = _grid.GetCell(worldPosition);

        if (cell == null || (_startingCell != null && _startingCell.index == cell.index))
            return false;

        if (_endingCell != null)
        {
            _endingCell.sr.color = Color.white;
            _endingCell = null;
        }

        cell.sr.color = Color.red;
        _endingCell = cell;
        return true;
    }

    public Tuple<Vector2, Vector2> GetPoints()
    {
        if (_startingCell != null && _endingCell != null)
            return new Tuple<Vector2, Vector2>(_startingCell.cellOrigin, _endingCell.cellOrigin);
        return null;
    }

    public void ModifyPoint(Vector2 point, Color color)
    {
        var cell = _grid.GetCell(point);

        if (cell == null)
            return;

        cell.sr.color = color;
        _onGridReset += cell.OnGridResetCallback;
    }

    public void ResetGrid()
    {
        _startingCell = null;
        _endingCell = null;
        _onGridReset?.Invoke();
        _onGridReset = null;
    }


    // Lifecycle methods

    private void Awake()
    {
        _grid = GetComponent<Grid>();
    }
}
