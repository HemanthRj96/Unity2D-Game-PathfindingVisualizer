using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lacobus.Grid
{
    /// <summary>
    /// Use this class to create grids of custom types
    /// </summary>
    public class Grid<TType> : IEnumerable<Cell<TType>> where TType : class, new()
    {
        // Fields

        private Vector2 _gridOrigin;
        private Vector2Int _gridDimension;
        private Vector2 _cellDimension;
        private int _totalCellCount;

        private Cell<TType>[,] _cellArray2D = new Cell<TType>[,] { };
        private Cell<TType>[] _cellArray1D = new Cell<TType>[] { };


        // Constructors

        public Grid()
            : this(Vector3.zero, 1, 1, Vector2.one) { }

        public Grid(Vector3 origin, Vector2Int cellCount, Vector2 cellSize)
            : this(origin, cellCount.x, cellCount.y, cellSize) { }

        public Grid(Vector3 origin, int horizontalCellCount, int verticalCellCount, Vector2 cellSize)
        {
            _gridOrigin = origin;
            _gridDimension = new Vector2Int(Mathf.Max(horizontalCellCount, 1), Mathf.Max(verticalCellCount, 1));
            _cellDimension = new Vector2(Mathf.Max(cellSize.x, 0.01f), Mathf.Max(cellSize.y, 0.01f));
            _totalCellCount = GridDimension.x * GridDimension.y;
        }


        // Properties

        /// <summary>
        /// Returns the origin of the grid
        /// </summary>
        public Vector2 GridOrigin { get { return _gridOrigin; } set { _gridOrigin = value; } }

        /// <summary>
        /// Retuns the dimension of the grid
        /// </summary>
        public Vector2Int GridDimension { get { return _gridDimension; } }

        /// <summary>
        /// Returns the dimension of the cell
        /// </summary>
        public Vector2 CellDimension { get { return _cellDimension; } }

        /// <summary>
        /// Returns the total number of cells
        /// </summary>
        public int TotalCellCount { get { return _totalCellCount; } }


        // Public methods

        /// <summary>
        /// Call this method before using the grid
        /// </summary>
        public void PrepareGrid()
        {
            _cellArray2D = new Cell<TType>[GridDimension.x, GridDimension.y];
            _cellArray1D = new Cell<TType>[TotalCellCount];

            for (int y = 0; y < GridDimension.y; y++)
            {
                for (int x = 0; x < GridDimension.x; x++)
                {
                    var cell = new Cell<TType>();
                    cell.Index = new Vector2Int(x, y);
                    cell.IsValid = true;
                    cell.Data = new TType();

                    _cellArray2D[x, y] = cell;
                    _cellArray1D[x + y * GridDimension.x] = cell;
                }
            }
        }


        /// <summary>
        /// Call this method from OnDrawGizmos to draw gizmos
        /// </summary>
        public void DrawGridLines(Color gridLineColor, Color crossLineColor)
        {
            Gizmos.color = gridLineColor;

            Vector2 corner = GridOrigin + new Vector2(GridDimension.x * CellDimension.x, GridDimension.y * CellDimension.y);

            // Horizontals
            Gizmos.DrawLine(GridOrigin, new Vector2(corner.x, GridOrigin.y));
            Gizmos.DrawLine(new Vector2(GridOrigin.x, corner.y), corner);

            // Verticals
            Gizmos.DrawLine(GridOrigin, new Vector2(GridOrigin.x, corner.y));
            Gizmos.DrawLine(new Vector2(corner.x, GridOrigin.y), corner);

            // Horizontal lines
            for (int h = 1; h < GridDimension.y; ++h)
                Gizmos.DrawLine
                    (
                        GridOrigin + (Vector2.up * h * CellDimension.y),
                        new Vector2(corner.x, GridOrigin.y) + (Vector2.up * h * CellDimension.y)
                    );

            // Vertical lines
            for (int w = 1; w < GridDimension.x; ++w)
                Gizmos.DrawLine
                    (
                        GridOrigin + (Vector2.right * w * CellDimension.x),
                        new Vector2(GridOrigin.x, corner.y) + (Vector2.right * w * CellDimension.x)
                    );

            Gizmos.color = crossLineColor;

            foreach (var cell in this)
            {
                if (cell.IsValid)
                    continue;

                Vector2 cellBL = new Vector2(cell.Index.x * CellDimension.x, cell.Index.y * CellDimension.y) + GridOrigin;
                Vector2 cellBR = new Vector2(cellBL.x + CellDimension.x, cellBL.y);
                Vector2 cellTL = new Vector2(cellBL.x, cellBL.y + CellDimension.y);
                Vector2 cellTR = new Vector2(cellBL.x + CellDimension.x, cellBL.y + CellDimension.y);

                Gizmos.DrawLine(cellBL, cellTR);
                Gizmos.DrawLine(cellBR, cellTL);
            }
        }



        /// <summary>
        /// Returns the cell itself in the grid
        /// </summary>
        /// <param name="x">x index</param>
        /// <param name="y">y index</param>
        public Cell<TType> GetCellRaw(int x, int y)
        {
            if (IsInside(x, y))
                return _cellArray2D[x, y];
            return null;
        }

        /// <summary>
        /// Returns the cell itself in the grid
        /// </summary>
        /// <param name="index">Cell index</param>
        public Cell<TType> GetCellRaw(Vector2Int index)
        {
            return GetCellRaw(index.x, index.y);
        }

        /// <summary>
        /// Returns the cell itself in the grid
        /// </summary>
        /// <param name="x">x index</param>
        /// <param name="y">y index</param>
        public Cell<TType> GetCellRaw(Vector3 worldPosition)
        {
            if (ConvertToXY(worldPosition, out int x, out int y))
                return _cellArray2D[x, y];
            return null;
        }



        /// <summary>
        /// Returns the 2D cell array itself
        /// </summary>
        public Cell<TType>[,] GetCellArray2D()
        {
            return _cellArray2D;
        }

        /// <summary>
        /// Returns the 1D cell array itself
        /// </summary>
        public Cell<TType>[] GetCellArray1D()
        {
            return _cellArray1D;
        }



        /// <summary>
        /// Returns the data of a grid cell
        /// </summary>
        /// <param name="x">x index</param>
        /// <param name="y">y index</param>
        public TType GetCellData(int x, int y)
        {
            if (IsInside(x, y))
                return _cellArray2D[x, y].Data;
            return null;
        }

        /// <summary>
        /// Returns the data of a grid cell
        /// </summary>
        /// <param name="index">Cell index</param>
        public TType GetCellData(Vector2Int index)
        {
            return GetCellData(index.x, index.y);
        }

        /// <summary>
        /// Returns the data of a grid cell
        /// </summary>
        /// <param name="worldPosition">World position of the cell</param>
        public TType GetCellData(Vector3 worldPosition)
        {
            ConvertToXY(worldPosition, out int x, out int y);
            return GetCellData(x, y);
        }


        /// <summary>
        /// Returns the cell validity
        /// </summary>
        /// <param name="x">x index</param>
        /// <param name="y">y index</param>
        public bool GetCellValidity(int x, int y)
        {
            if (IsInside(x, y))
                return _cellArray2D[x, y].IsValid;
            return false;
        }

        /// <summary>
        /// Returns the cell validity
        /// </summary>
        /// <param name="index">Cell index</param>
        public bool GetCellValidity(Vector2Int index)
        {
            return GetCellValidity(index.x, index.y);
        }

        /// <summary>
        /// Returns the cell validity
        /// </summary>
        /// <param name="worldPosition">World position of this cell</param>
        public bool GetCellValidity(Vector3 worldPosition)
        {
            if (ConvertToXY(worldPosition, out int x, out int y))
                return _cellArray2D[x, y].IsValid;
            return false;
        }



        /// <summary>
        /// Returns true if x and y is inside the grid
        /// </summary>
        /// <param name="x">X index</param>
        /// <param name="y">Y index</param>
        public bool IsInside(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < GridDimension.x && y < GridDimension.y)
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if the point is inside the grid
        /// </summary>
        /// <param name="worldPosition">Target world position</param>
        public bool IsInside(Vector3 worldPosition)
        {
            ConvertToXY(worldPosition, out int x, out int y);
            return IsInside(x, y);
        }

        /// <summary>
        /// Returns true if the index is inside the grid
        /// </summary>
        /// <param name="index">Target index</param>
        public bool IsInside(Vector2Int index)
        {
            return IsInside(index.x, index.y);
        }



        /// <summary>
        /// Converts world position into grid sections
        /// </summary>
        /// <param name="worldPosition">Target position</param>
        /// <param name="x">Out parameter for x</param>
        /// <param name="y">Out parameter for y</param>
        /// <returns>Returns true if this is a valid point</returns>
        public bool ConvertToXY(Vector3 worldPosition, out int x, out int y)
        {
            bool ret = ConvertToXY(worldPosition, out Vector2Int index);
            x = index.x;
            y = index.y;
            return ret;
        }

        /// <summary>
        /// Converts world position into grid sections
        /// </summary>
        /// <param name="worldPosition">Target position</param>
        /// <param name="isInsde">Will be true if this worldposition is inside the grid</param>
        /// <returns>Returns converted index</returns>
        public Vector2Int ConvertToXY(Vector3 worldPosition, out bool isInsde)
        {
            isInsde = ConvertToXY(worldPosition, out Vector2Int index);
            return index;
        }

        /// <summary>
        /// Converts world position into grid sections
        /// </summary>
        /// <param name="worldPosition">Target position</param>
        /// <param name="x">Out parameter for x</param>
        /// <param name="y">Out parameter for y</param>
        /// <returns>Retusn true if this is a valid point</returns>
        public bool ConvertToXY(Vector3 worldPosition, out Vector2Int index)
        {
            int x = Mathf.FloorToInt((worldPosition - (Vector3)GridOrigin).x / CellDimension.x);
            int y = Mathf.FloorToInt((worldPosition - (Vector3)GridOrigin).y / CellDimension.y);

            index = new Vector2Int(x, y);

            return IsInside(x, y);
        }



        /// <summary>
        /// Converts grid sections to world points
        /// </summary>
        /// <param name="x">Target x</param>
        /// <param name="y">Target y</param>
        public Vector3 ConvertToWorldPosition(int x, int y)
        {
            return new Vector2(x * CellDimension.x, y * CellDimension.y) + GridOrigin;
        }

        /// <summary>
        /// Converts grid sections to world points
        /// </summary>
        /// <param name="x">Target x</param>
        /// <param name="y">Target y</param>
        public Vector3 ConvertToWorldPosition(Vector2Int index)
        {
            return new Vector2(index.x * CellDimension.x, index.y * CellDimension.y) + GridOrigin;
        }



        /// <summary>
        /// Returns the grid bounds
        /// </summary>
        public Rect GetGridBounds()
        {
            return new Rect(ConvertToWorldPosition(0, 0), new Vector2(CellDimension.x * GridDimension.x, CellDimension.y * GridDimension.y));
        }



        /// <summary>
        /// Returns the target cell's bounds
        /// </summary>
        /// <param name="x">X index</param>
        /// <param name="y">Y index</param>
        /// <returns>Returns zero if invalid</returns>
        public Rect GetCellBounds(int x, int y)
        {
            if (IsInside(x, y))
                return new Rect(GetCellCenter(x, y), CellDimension);
            return Rect.zero;
        }

        /// <summary>
        /// Returns the target cell's bounds
        /// </summary>
        /// <param name="index">Index of the cell</param>
        /// <returns>Returns zero if invalid</returns>
        public Rect GetCellBounds(Vector2Int index)
        {
            return GetCellBounds(index.x, index.y);
        }



        /// <summary>
        /// Returns the bounds of all cells in a 2D array
        /// </summary>
        public Rect[,] GetCellBoundsArray2D()
        {
            Rect[,] bounds = new Rect[GridDimension.x, GridDimension.y];
            for (int x = 0; x < GridDimension.x; ++x)
                for (int y = 0; y < GridDimension.y; ++y)
                    bounds[x, y] = new Rect(ConvertToWorldPosition(x, y) + new Vector3(CellDimension.x, CellDimension.y) / 2, CellDimension);
            return bounds;
        }

        /// <summary>
        /// Returns the bounds of all cells in a 1D array
        /// </summary>
        public Rect[] GetCellBoundsArray1D()
        {
            Rect[] bounds = new Rect[TotalCellCount];
            for (int y = 0; y < GridDimension.y; ++y)
                for (int x = 0; x < GridDimension.x; ++x)
                    bounds[x + y * GridDimension.x] = new Rect(ConvertToWorldPosition(x, y) + new Vector3(CellDimension.x, CellDimension.y) / 2, CellDimension);
            return bounds;
        }



        /// <summary>
        /// Returns a 2D array of all cell centers
        /// </summary>
        public Vector3[,] GetCellCenterArray2D()
        {
            Vector3[,] cellCenters = new Vector3[GridDimension.x, GridDimension.y];
            for (int y = 0; y < GridDimension.y; ++y)
                for (int x = 0; x < GridDimension.x; ++x)
                    cellCenters[x, y] = ConvertToWorldPosition(x, y) + new Vector3(CellDimension.x, CellDimension.y) / 2;
            return cellCenters;
        }

        /// <summary>
        /// Returns a 1D array of all cell centers
        /// </summary>
        public Vector3[] GetCellCenterArray1D()
        {
            Vector3[] cellCenters = new Vector3[TotalCellCount];
            for (int y = 0; y < GridDimension.y; ++y)
                for (int x = 0; x < GridDimension.x; ++x)
                    cellCenters[x + y * GridDimension.x] = ConvertToWorldPosition(x, y) + new Vector3(CellDimension.x, CellDimension.y) / 2;
            return cellCenters;
        }



        /// <summary>
        /// Returns the center of a cell
        /// </summary>
        /// <param name="x">X value of a cell</param>
        /// <param name="y">Y value of a cell</param>
        /// <returns>Returns positive infinity if the cell is not valid otherwise actual value</returns>
        public Vector3 GetCellCenter(int x, int y)
        {
            if (IsInside(x, y))
                return (ConvertToWorldPosition(x, y) + new Vector3(CellDimension.x, CellDimension.y) / 2);
            return Vector3.positiveInfinity;
        }

        /// <summary>
        /// Returns the center of a cell
        /// </summary>
        /// <param name="index">Index of the cell</param>
        /// <returns>Returns positive infinity if the cell is not valid otherwise actual value</returns>
        public Vector3 GetCellCenter(Vector2Int index)
        {
            return GetCellCenter(index.x, index.y);
        }

        /// <summary>
        /// Returns the center of a cell
        /// </summary>
        /// <param name="worldPosition">World position of the cell</param>
        /// <returns>Returns positive infinity if the cell is not valid otherwise actual value</returns>
        public Vector3 GetCellCenter(Vector3 worldPosition)
        {
            ConvertToXY(worldPosition, out int x, out int y);
            return GetCellCenter(x, y);
        }



        /// <summary>
        /// Sets the value of the cell at target index
        /// </summary>
        /// <param name="x">x index</param>
        /// <param name="y">y index</param>
        /// <param name="data">Value</param>
        public void SetCellDataAt(int x, int y, TType data)
        {
            if (IsInside(x, y))
            {
                _cellArray2D[x, y].Data = data;
            }
        }

        /// <summary>
        /// Sets the value of the cell at target index
        /// </summary>
        /// <param name="index">Cell index</param>
        /// <param name="data">Value</param>
        public void SetCellDataAt(Vector2Int index, TType data)
        {
            SetCellDataAt(index.x, index.y, data);
        }

        /// <summary>
        /// Sets the value of the cell at target world position
        /// </summary>
        /// <param name="worldPosition">World position</param>
        /// <param name="value">Value</param>
        public void SetCellDataAt(Vector3 worldPosition, TType data)
        {
            if (ConvertToXY(worldPosition, out int x, out int y))
            {
                _cellArray2D[x, y].Data = data;
            }
        }



        /// <summary>
        /// Set the cell validity at index
        /// </summary>
        /// <param name="x">x index</param>
        /// <param name="y">y index</param>
        public void SetCellValidityAt(int x, int y, bool isValid)
        {
            if (IsInside(x, y))
            {
                _cellArray2D[x, y].IsValid = isValid;
            }
        }

        /// <summary>
        /// Set the cell validity at index
        /// </summary>
        /// <param name="index">Cell index</param>
        public void SetCellValidityAt(Vector2Int index, bool isValid)
        {
            SetCellValidityAt(index.x, index.y, isValid);
        }

        /// <summary>
        /// Set the cell validity at world position
        /// </summary>
        /// <param name="worldPosition">World position of the cell</param>
        public void SetCellValidityAt(Vector3 worldPosition, bool isValid)
        {
            if (ConvertToXY(worldPosition, out int x, out int y))
            {
                _cellArray2D[x, y].IsValid = isValid;
            }
        }



        /// <summary>
        /// Returns true if the grid overlaps with another grid. The out parameter will have all the cells this grid overlaps
        /// </summary>
        /// <param name="otherGrid">Other grid</param>
        /// <param name="overlappedCellsIndeces">Cells that are overlapped</param>
        public bool Overlaps(Grid<TType> otherGrid, out Vector2Int[] overlappedCellsIndeces)
        {
            Rect gridABound = this.GetGridBounds();
            Rect gridBBound = otherGrid.GetGridBounds();
            bool bDoesGridOverlap = gridABound.Overlaps(gridBBound);
            List<Vector2Int> overlappedCells = new List<Vector2Int>();

            if (bDoesGridOverlap)
            {
                var cellBoundsArray = this.GetCellBoundsArray2D();
                var gridDimension = this.GridDimension;

                for (int y = 0; y < gridDimension.y; ++y)
                    for (int x = 0; x < gridDimension.x; ++x)
                        if (this.GetCellValidity(x, y) && cellBoundsArray[x, y].Overlaps(gridBBound))
                            overlappedCells.Add(new Vector2Int(x, y));

                overlappedCellsIndeces = overlappedCells.ToArray();
            }
            else
                overlappedCellsIndeces = null;

            return bDoesGridOverlap;
        }



        /// <summary>
        /// Overrided
        /// </summary>
        public override string ToString()
        {
            string retValue = "";

            for (int v = 0; v < GridDimension.y; ++v)
            {
                for (int h = 0; h < GridDimension.x; ++h)
                    retValue += _cellArray2D[h, v].ToString() + " ";
                retValue += "\n";
            }
            return retValue;
        }


        // IEnumerable interface implemetation

        public IEnumerator<Cell<TType>> GetEnumerator()
        {
            foreach (var cell in _cellArray1D)
            {
                yield return cell;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}