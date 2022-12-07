using System;
using System.Collections.Generic;
using UnityEngine;


public class AStarPathfinder : MonoBehaviour
{
    // Fields

    [SerializeField] private Grid _grid;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private bool _canMoveDiagonally;

    const float STRAIGHT_COST = 10;
    const float DIAGNOL_COST = 14;


    // Properties

    public int pathfindingCycle { get; private set; }


    // Public methods

    public bool BeginPathfinding()
    {
        Tuple<Vector2, Vector2> points = _gridManager.GetPoints();
        List<Vector2> pathPoints = new List<Vector2>();

        if (points == null || getPath(points.Item1, points.Item2, out pathPoints) == false)
            return false;

        for (int i = 0; i < pathPoints.Count; ++i)
            _gridManager.ModifyPoint(pathPoints[i], Color.Lerp(Color.green, Color.red, (float)i / pathPoints.Count));

        return true;
    }


    // Private methods

    private bool getPath(Vector2 startWorldPosition, Vector2 endWorldPosition, out List<Vector2> pathPoints)
    {
        if (!_grid.isGridReady)
        {
            pathPoints = null;
            print("Grid not ready");
            return false;
        }

        // initialize start and end node
        Cell startNode = _grid.GetCell(startWorldPosition);
        Cell endNode = _grid.GetCell(endWorldPosition);

        // check if the cells are valid or not
        if (startNode == null || endNode == null)
        {
            pathPoints = null;
            print("Cells invalid");
            return false;
        }

        // initialize open and closed list
        List<Cell> openList = new List<Cell> { startNode };
        List<Cell> closedList = new List<Cell>();

        startNode.gCost = 0;
        startNode.hCost = getHCost(startNode.index, endNode.index, 1);

        while (openList.Count > 0)
        {
            Cell currentNode = getLowestFCostNode(openList);

            // check if the path is complete
            if (currentNode.index == endNode.index)
            {
                print("Paths found");
                pathPoints = tracePath(endNode);
                return true;
            }

            // remove from open list and add to closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            /// get all the neighbours
            var neighbours = getNeighbours(currentNode.index);

            foreach (Cell neighbour in neighbours)
            {
                if (neighbour == null || closedList.Contains(neighbour))
                    continue;

                if (neighbour.isValid == false)
                {
                    closedList.Add(neighbour);
                    continue;
                }

                float gCost = currentNode.gCost + getHCost(currentNode.index, neighbour.index, neighbour.costMultiplier);
                float hCost = getHCost(neighbour.index, endNode.index, 1);
                if (gCost < neighbour.gCost)
                {
                    neighbour.fromNode = currentNode;
                    neighbour.gCost = gCost;
                    neighbour.hCost = hCost;

                    if (openList.Contains(neighbour) == false)
                        openList.Add(neighbour);

                    _gridManager.ModifyPoint(neighbour.cellOrigin, Color.cyan);
                }
            }
        }

        // No path found
        print("Searched everything");
        pathPoints = null;
        return false;
    }

    private float getHCost(Vector2Int startIndex, Vector2Int endIndex, float costMultiplier)
    {
        int xDistance = Mathf.Abs(startIndex.x - endIndex.x);
        int yDistance = Mathf.Abs(startIndex.y - endIndex.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return (DIAGNOL_COST * (1 + costMultiplier) * Mathf.Min(xDistance, yDistance)) +
               (STRAIGHT_COST * (1 + costMultiplier) * remaining);
    }

    private Cell getLowestFCostNode(List<Cell> nodes)
    {
        var lowestFCostNode = nodes[0];

        for (int i = 1; i < nodes.Count; ++i)
            if (nodes[i].fCost < lowestFCostNode.fCost)
                lowestFCostNode = nodes[i];

        return lowestFCostNode;
    }

    private List<Vector2> tracePath(Cell endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Cell currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.cellOrigin);
            currentNode = currentNode.fromNode;
        }

        path.Reverse();
        return path;
    }

    private List<Cell> getNeighbours(Vector2Int index)
    {
        List<Cell> neighbours = new List<Cell>();

        Cell up, upRight, upLeft, down, downRight, downLeft, left, right;

        up = _grid.GetCell(new Vector2Int(index.x, index.y + 1));
        down = _grid.GetCell(new Vector2Int(index.x, index.y - 1));
        left = _grid.GetCell(new Vector2Int(index.x - 1, index.y));
        right = _grid.GetCell(new Vector2Int(index.x + 1, index.y));

        upRight = _grid.GetCell(new Vector2Int(index.x + 1, index.y + 1));
        upLeft = _grid.GetCell(new Vector2Int(index.x - 1, index.y + 1));
        downRight = _grid.GetCell(new Vector2Int(index.x + 1, index.y - 1));
        downLeft = _grid.GetCell(new Vector2Int(index.x - 1, index.y - 1));

        // Add straight paths
        neighbours.Add(up);
        neighbours.Add(down);
        neighbours.Add(left);
        neighbours.Add(right);

        // Add diagonal paths
        if (_canMoveDiagonally)
        {
            if (up != null && right != null && up.isValid && right.isValid)
                neighbours.Add(upRight);
            if (up != null && left != null && up.isValid && left.isValid)
                neighbours.Add(upLeft);
            if (down != null && right != null && down.isValid && right.isValid)
                neighbours.Add(downRight);
            if (down != null && left != null && down.isValid && left.isValid)
                neighbours.Add(downLeft);
        }

        return neighbours;
    }
}
