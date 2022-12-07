using UnityEngine;


namespace Lacobus.Grid
{
    public class Cell<TType> where TType : class
    {
        // Fields

        private bool _isValid;
        private TType _data;
        private Vector2Int _index;


        // Properties

        /// <summary>
        /// Returns true if this cell is usable/ is valid
        /// </summary>
        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
            }
        }

        /// <summary>
        /// Returns the data stored in this cell
        /// </summary>
        public TType Data
        {
            get
            {
                return _data;
            }
            set
            {
                if (_isValid)
                    _data = value;
            }
        }

        /// <summary>
        /// Returns the index of this cell
        /// </summary>
        public Vector2Int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }


        // Public methods

        /// <summary>
        /// Override method to format the values inside the cell
        /// </summary>
        public override string ToString() => (Data == null ? "-" : Data.ToString()) + ":" + IsValid;

        /// <summary>
        /// Returns true if the cell data and index are the same
        /// </summary>
        /// <param name="anotherCell">Cell you want to compare against</param>
        public bool IsSame(Cell<TType> anotherCell)
        {
            return Data.Equals(anotherCell.Data) && Index == anotherCell.Index;
        }
    }
}