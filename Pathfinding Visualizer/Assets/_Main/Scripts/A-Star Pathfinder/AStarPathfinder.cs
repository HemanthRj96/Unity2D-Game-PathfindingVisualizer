using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class AStarPathfinder : MonoBehaviour
{
    // Fields

    [Header("Pathfinder settings")]
    public bool _canMoveDiagonally;

    const float STRAIGHT_COST = 10;
    const float DIAGNOL_COST = 14;


    // Public methods

    public bool ComputePath(Vector2 startWorldPosition, Vector2 endWorldPosition)
    {
        if (!Grid.grid.isGridReady)
        {
            return false;
        }

        // initialize start and end node
        Cell startNode = Grid.grid.GetCell(startWorldPosition);
        Cell endNode = Grid.grid.GetCell(endWorldPosition);

        // check if the cells are valid or not
        if (startNode == null || endNode == null)
        {
            return false;
        }

        var updateData = GridManager.manager.updateData;

        // initialize open and closed list
        List<Cell> openList = new List<Cell> { startNode };
        List<Cell> closedList = new List<Cell>();

        startNode.gCost = 0;
        startNode.hCost = getHCost(startNode.index, endNode.index, 1);

        int cycle = 1;

        while (openList.Count > 0)
        {
            Cell currentNode = getLowestFCostNode(openList);

            // check if the path is complete
            if (currentNode.Equals(endNode))
            {
                updateData.pathpoints = tracePath(endNode);
                return true;
            }

            // remove from open list and add to closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            updateData.cellUpdateCycle.Add(cycle, new List<Cell>());
            /// get all the neighbours
            var neighbours = getNeighbours(currentNode.index);

            foreach (Cell neighbour in neighbours)
            {
                if (neighbour == null || closedList.Contains(neighbour))
                    continue;

                if (neighbour.canWalk == false)
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

                    if (neighbour.index != endNode.index && neighbour.index != startNode.index)
                        updateData.cellUpdateCycle[cycle].Add(neighbour);
                }
            }
            cycle++;
        }

        // No path found
        return false;
    }


    // Private methods

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

        up = Grid.grid.GetCell(new Vector2Int(index.x, index.y + 1));
        down = Grid.grid.GetCell(new Vector2Int(index.x, index.y - 1));
        left = Grid.grid.GetCell(new Vector2Int(index.x - 1, index.y));
        right = Grid.grid.GetCell(new Vector2Int(index.x + 1, index.y));

        upRight = Grid.grid.GetCell(new Vector2Int(index.x + 1, index.y + 1));
        upLeft = Grid.grid.GetCell(new Vector2Int(index.x - 1, index.y + 1));
        downRight = Grid.grid.GetCell(new Vector2Int(index.x + 1, index.y - 1));
        downLeft = Grid.grid.GetCell(new Vector2Int(index.x - 1, index.y - 1));

        // Add straight paths
        neighbours.Add(up);
        neighbours.Add(down);
        neighbours.Add(left);
        neighbours.Add(right);

        // Add diagonal paths
        if (_canMoveDiagonally)
        {
            if (up != null && right != null && up.canWalk && right.canWalk)
                neighbours.Add(upRight);
            if (up != null && left != null && up.canWalk && left.canWalk)
                neighbours.Add(upLeft);
            if (down != null && right != null && down.canWalk && right.canWalk)
                neighbours.Add(downRight);
            if (down != null && left != null && down.canWalk && left.canWalk)
                neighbours.Add(downLeft);
        }

        return neighbours;
    }
}
