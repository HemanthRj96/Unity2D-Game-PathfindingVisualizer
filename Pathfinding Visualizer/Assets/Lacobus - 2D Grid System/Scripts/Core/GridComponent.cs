using UnityEngine;


namespace Lacobus.Grid
{
    public sealed class GridComponent : MonoBehaviour
    {
        // Fields 

        [SerializeField] private GridComponentDataContainer _gcData;
        [SerializeField] private bool _useSimpleSpriteRendering = false;
        [SerializeField] private Sprite _defaultSimpleSprite = null;

        private Grid<DefaultCell> _grid = null;
        private Transform _t;


        // Properties

        private Transform t
        {
            get
            {
                if (_t)
                    return _t;
                else
                {
                    _t = transform;
                    return _t;
                }
            }
        }
        private Vector2 gridOrigin
        {
            get
            {
                return _gcData.gridOffset + (Vector2)t.position;
            }
        }
        public Grid<DefaultCell> Grid { get { return _grid; } }


        // Public methods

        /// <summary>
        /// Call this method to change sprite at a specific index
        /// </summary>
        /// <param name="index">Target index</param>
        /// <param name="targetSprite">Target color</param>
        public void SetSpriteAt(Vector2Int index, Sprite targetSprite)
        {
            if (_grid.IsInside(index))
                _grid.GetCellData(index).ChangeSprite(targetSprite);
        }

        /// <summary>
        /// Call this method to change sprite at a specific index
        /// </summary>
        /// <param name="x">x index</param>
        /// <param name="y">y index</param>
        /// <param name="targetSprite">Target color</param>
        public void SetSpriteAt(int x, int y, Sprite targetSprite)
        {
            if (_grid.IsInside(x, y))
                _grid.GetCellData(x, y).ChangeSprite(targetSprite);
        }

        /// <summary>
        /// Call this method to change sprite at a specific world position
        /// </summary>
        /// <param name="worldPosition">Target world position</param>
        /// <param name="targetSprite">Target color</param>
        public void SetSpriteAt(Vector3 worldPosition, Sprite targetSprite)
        {
            if (_grid.IsInside(worldPosition))
                _grid.GetCellData(worldPosition).ChangeSprite(targetSprite);
        }



        /// <summary>
        /// Call this method to change sprite color at a specific index
        /// </summary>
        /// <param name="index">Target index</param>
        /// <param name="targetColor">Target color</param>
        public void SetSpriteColorAt(Vector2Int index, Color targetColor)
        {
            if (_grid.IsInside(index))
                _grid.GetCellData(index).ChangeColor(targetColor);
        }

        /// <summary>
        /// Call this method to change sprite color at a specific index
        /// </summary>
        /// <param name="x">x index</param>
        /// <param name="y">y index</param>
        /// <param name="targetColor">Target color</param>
        public void SetSpriteColorAt(int x, int y, Color targetColor)
        {
            if (_grid.IsInside(x, y))
                _grid.GetCellData(x, y).ChangeColor(targetColor);
        }

        /// <summary>
        /// Call this method to change sprite color at a specific world position
        /// </summary>
        /// <param name="worldPosition">Target world position</param>
        /// <param name="targetColor">Target color</param>
        public void SetSpriteColorAt(Vector3 worldPosition, Color targetColor)
        {
            if (_grid.IsInside(worldPosition))
                _grid.GetCellData(worldPosition).ChangeColor(targetColor);
        }



        /// <summary>
        /// Call this method to change sprite color at a specific index
        /// </summary>
        /// <param name="index">Target index</param>
        /// <param name="size">Target size</param>
        public void SetSpriteSizeAt(Vector2Int index, Vector2 size)
        {
            if (_grid.IsInside(index))
                _grid.GetCellData(index).ChangeSpriteSize(size);
        }

        /// <summary>
        /// Call this method to change sprite color at a specific index
        /// </summary>
        /// <param name="x">x index</param>
        /// <param name="y">y index</param>
        /// <param name="size">Target size</param>
        public void SetSpriteSizeAt(int x, int y, Vector2 size)
        {
            if (_grid.IsInside(x, y))
                _grid.GetCellData(x, y).ChangeSpriteSize(size);
        }

        /// <summary>
        /// Call this method to change sprite color at a specific world position
        /// </summary>
        /// <param name="worldPosition">Target world position</param>
        /// <param name="size">Target size</param>
        public void SetSpriteSizeAt(Vector2 worldPosition, Vector2 size)
        {
            if (_grid.IsInside(worldPosition))
                _grid.GetCellData(worldPosition).ChangeSpriteSize(size);
        }


        // Lifecycle methods

        private void Awake()
        {
            _t = transform;

            // Create grid here
            _grid = new Grid<DefaultCell>(gridOrigin, _gcData.gridDimension, _gcData.cellDimension);
            _grid.PrepareGrid();

            if (_useSimpleSpriteRendering)
                setupSimpleSpriteRendering();
        }

        private void Update()
        {
            _grid.GridOrigin = _gcData.gridOffset + (Vector2)_t.position;
        }

        private void OnValidate()
        {
            _grid = new Grid<DefaultCell>(gridOrigin, _gcData.gridDimension, _gcData.cellDimension);
        }

        private void OnDrawGizmos()
        {
            if (_gcData.shouldDrawGizmos == false)
                return;

            _grid.GridOrigin = _gcData.gridOffset + (Vector2)transform.position;
            _grid.DrawGridLines(_gcData.gridLineColor, _gcData.crossLineColor);
        }

        private void setupSimpleSpriteRendering()
        {
            foreach (var c in _grid)
            {
                GameObject go = new GameObject($"{c.Index}", typeof(SpriteRenderer));

                c.Data.sr = go.GetComponent<SpriteRenderer>();
                c.Data.sr.sprite = _defaultSimpleSprite;

                go.transform.parent = _t;
                go.transform.position = _grid.GetCellCenter(c.Index);
                go.transform.localScale = _grid.CellDimension;
            }
        }


        // Nested types

        private enum OffsetType
        {
            Preset,
            Custom
        }

        private enum PresetTypes
        {
            TopRight,
            TopCenter,
            TopLeft,
            MiddleRight,
            MiddleCenter,
            MiddleLeft,
            BottomRight,
            BottomCenter,
            BottomLeft
        }

        [System.Serializable]
        private class GridComponentDataContainer
        {
            // Grid things
            [SerializeField]
            public Vector2Int gridDimension = new Vector2Int();
            [SerializeField]
            public Vector2 cellDimension = new Vector2();
            [SerializeField]
            public Vector2 gridOffset = new Vector2();

            // Gizmos and editor things
            [SerializeField]
            public OffsetType offsetType = OffsetType.Preset;
            [SerializeField]
            public PresetTypes presetType = PresetTypes.BottomLeft;
            [SerializeField]
            public bool shouldDrawGizmos = false;
            [SerializeField]
            public Color gridLineColor;
            [SerializeField]
            public Color crossLineColor;
        }

        public class DefaultCell
        {
            // Fields

            public SpriteRenderer sr;


            // Public methods 

            public void ChangeSprite(Sprite sprite)
            {
                sr.sprite = sprite;
            }

            public void ChangeColor(Color color)
            {
                sr.color = color;
            }

            public void ChangeSpriteSize(Vector2 size)
            {
                sr.transform.localScale = size;
            }
        }
    }
}