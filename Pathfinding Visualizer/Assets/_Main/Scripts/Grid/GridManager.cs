using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class GridManager : MonoBehaviour
{
    // Fields

    [Header("Grid Settings")]
    public Color startColor;
    public Color endColor;
    public Color scanColor;
    public float updateDelay;
    public Action<int> onCycleUpdate = delegate { };

    [Header("Pathfinders")]
    [SerializeField] private AStarPathfinder _aStar;

    bool _isSimulating = false;
    bool _pauseSimulation = false;
    bool _isReseting = false;


    // Properties

    public static GridManager manager { get; private set; }
    public GridUpdateData updateData { get; set; } = new GridUpdateData();
    public Cell start { get; private set; } = null;
    public Cell end { get; private set; }


    // Public methods

    public bool SetStart(Vector2 worldPosition)
    {
        var cell = Grid.grid.GetCell(worldPosition);

        if (cell == null || cell.Equals(end) || cell.Equals(start))
            return false;

        if (start != null)
            start.SlowReset();

        cell.SelectCell(startColor);
        start = cell;

        return true;
    }

    public bool SetEnd(Vector2 worldPosition)
    {
        var cell = Grid.grid.GetCell(worldPosition);

        if (cell == null || cell.Equals(start) || cell.Equals(end))
            return false;

        if (end != null)
            end.SlowReset();

        cell.SelectCell(endColor);
        end = cell;

        return true;
    }

    public bool SetWalls(Vector2 worldPosition)
    {
        var cell = Grid.grid.GetCell(worldPosition);

        if (cell == null)
            return false;

        cell.BlockCell();
        return true;
    }

    public bool SetWeights(Vector2 worldPosition)
    {
        var cell = Grid.grid.GetCell(worldPosition);

        if (cell == null)
            return false;

        cell.WeightCell();
        return true;
    }

    public void ResetCell(Vector2 worldPosition)
    {
        var cell = Grid.grid.GetCell(worldPosition);
        if (cell)
            cell.SlowReset();
    }

    public void Simulate()
    {
        if (_isSimulating)
            _pauseSimulation = !_pauseSimulation;
        else
        {
            _isSimulating = true;
            _pauseSimulation = false;
            StartCoroutine(cellUpdater());
        }
    }

    public void ClearPath()
    {
        if (_isReseting)
            return;
        _isReseting = true;

        StartCoroutine(clearPath());
    }

    public void ClearWalls()
    {
        var cellCount = Grid.grid.cellCount;
        var cellArray = Grid.grid.GetCellArray();

        for (int i = 0; i < cellCount.x; ++i)
            for (int j = 0; j < cellCount.y; ++j)
                if (cellArray[i, j].cellType == Cell.CellType.wall)
                    cellArray[i, j].SlowReset();
    }

    public void ClearWeights()
    {
        var cellCount = Grid.grid.cellCount;
        var cellArray = Grid.grid.GetCellArray();

        for (int i = 0; i < cellCount.x; ++i)
            for (int j = 0; j < cellCount.y; ++j)
                if (cellArray[i, j].cellType == Cell.CellType.weight)
                    cellArray[i, j].SlowReset();
    }

    public void ClearGrid()
    {
        ClearPath();
        ClearWalls();
        ClearWeights();
    }


    // Private methods

    private IEnumerator cellUpdater()
    {
        yield return new WaitUntil(() => !_isReseting);

        if (start == null || end == null)
        {
            _pauseSimulation = false;
            _isSimulating = false;
            yield break;
        }

        if (_aStar.ComputePath(start.cellOrigin, end.cellOrigin))
        {
            // update all modified cells per cycle
            foreach (var pair in updateData.cellUpdateCycle)
            {
                foreach (var c in pair.Value)
                    c.ModifyCell(scanColor);

                onCycleUpdate(pair.Key);
                yield return new WaitForSeconds(updateDelay);
                yield return new WaitUntil(() => !_pauseSimulation);
            }

            yield return new WaitForSeconds(0.5f);

            // on finish update the path
            var p = updateData.pathpoints;
            for (int i = 0; i < p.Count; ++i)
            {
                Grid.grid.GetCell(p[i]).ModifyCell(Color.Lerp(startColor, endColor, (float)i / p.Count));
                yield return new WaitForSeconds(updateDelay);
                yield return new WaitUntil(() => !_pauseSimulation);
            }
        }
        else
        {
            // update all modified cells per cycle
            foreach (var pair in updateData.cellUpdateCycle)
            {
                foreach (var c in pair.Value)
                    c.ModifyCell(scanColor);

                onCycleUpdate(pair.Key);
                yield return new WaitForSeconds(updateDelay);
                yield return new WaitUntil(() => !_pauseSimulation);
            }

            int tCycles = updateData.cellUpdateCycle.Last().Key;

            foreach (var pair in updateData.cellUpdateCycle)
            {
                foreach (var c in pair.Value)
                    c.ModifyCell(Color.Lerp(Color.red, Color.black, (float)pair.Key / tCycles));
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            ClearPath();
        }

        yield return null;
        _pauseSimulation = false;
        _isSimulating = false;
    }

    private IEnumerator clearPath()
    {
        yield return new WaitUntil(() => !_isSimulating);

        start.SlowReset();
        end.SlowReset();
        start = null;
        end = null;

        foreach (var p in updateData.pathpoints)
        {
            Grid.grid.GetCell(p).SlowReset();
            yield return null;
        }

        foreach (var k in updateData.cellUpdateCycle)
        {
            foreach (var c in k.Value)
                c.SlowReset();
            yield return null;
        }

        updateData.pathpoints.Clear();
        updateData.cellUpdateCycle.Clear();
        _isReseting = false;
    }


    // Lifecycle methods

    private void Awake()
    {
        if (manager == null)
            manager = this;
    }


    // Nested types

    public class GridUpdateData
    {
        public List<Vector2> pathpoints = new List<Vector2>();
        public Dictionary<int, List<Cell>> cellUpdateCycle = new Dictionary<int, List<Cell>>();
    }
}
