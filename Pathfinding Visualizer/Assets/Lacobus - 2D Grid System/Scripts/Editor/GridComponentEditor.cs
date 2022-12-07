using Lacobus.Grid;
using UnityEditor;
using UnityEngine;


namespace Lacobus_Editors.Grid
{
    [CustomEditor(typeof(GridComponent))]
    public class GridComponentEditor : EditorUtils<GridComponent>
    {
        private static bool _enableDebugSettings = false;

        SerializedProperty _gcData;
        SerializedProperty _gridDimesion;
        SerializedProperty _cellDimesion;
        SerializedProperty _gridOffset;

        SerializedProperty _offsetType;
        SerializedProperty _presetType;
        SerializedProperty _shouldDrawGizmos;
        SerializedProperty _gridLineColor;
        SerializedProperty _crossLineColor;

        SerializedProperty _useSimpleSpriteRendering;
        SerializedProperty _defaultSimpleSprite;



        public override void CustomOnGUI()
        {
            bool shouldDisable = EditorApplication.isPlaying || EditorApplication.isPaused;

            if (shouldDisable)
                Info("Exit playmode to edit fields", MessageType.Warning);

            EditorGUI.BeginDisabledGroup(shouldDisable);

            _gcData = GetProperty("_gcData");
            _useSimpleSpriteRendering = GetProperty("_useSimpleSpriteRendering");
            _defaultSimpleSprite = GetProperty("_defaultSimpleSprite");

            _gridDimesion = _gcData.FindPropertyRelative("gridDimension");
            _cellDimesion = _gcData.FindPropertyRelative("cellDimension");
            _gridOffset = _gcData.FindPropertyRelative("gridOffset");
            _offsetType = _gcData.FindPropertyRelative("offsetType");
            _presetType = _gcData.FindPropertyRelative("presetType");
            _shouldDrawGizmos = _gcData.FindPropertyRelative("shouldDrawGizmos");
            _gridLineColor = _gcData.FindPropertyRelative("gridLineColor");
            _crossLineColor = _gcData.FindPropertyRelative("crossLineColor");

            Heading("Grid Settings");
            Space(10);

            _enableDebugSettings = EditorGUILayout.Toggle("Show debug settings : ", _enableDebugSettings);

            if (_shouldDrawGizmos.boolValue == false)
            {
                if (Button("Enable Grid View"))
                    _shouldDrawGizmos.boolValue = true;
            }
            else
            {
                if (Button("Disable Grid View"))
                    _shouldDrawGizmos.boolValue = false;
            }

            Space(10);

            PropertyField(_useSimpleSpriteRendering, "Create sprite grid on Awake :", "Set this as true if you need some kind of representation of cells");
            if (_useSimpleSpriteRendering.boolValue)
                PropertyField(_defaultSimpleSprite, "Default sprite :", "This will be the default sprite for all the cells");

            Space(20);

            PropertyField(_gridDimesion, "Grid Dimensions : ", "Width and height of the grid");
            PropertyField(_cellDimesion, "Cell Dimensions : ", "The size of a single cell");
            PropertyField(_offsetType, "Pivot Type : ", "");


            int h = _gridDimesion.vector2IntValue.x;
            int v = _gridDimesion.vector2IntValue.y;
            Vector2 cd = _cellDimesion.vector2Value;

            // Grid origin
            switch (_offsetType.enumValueIndex)
            {
                case 0:
                    PropertyField(_presetType, "Select Preset Pivot : ", "");
                    switch (_presetType.enumValueIndex)
                    {
                        case 0:
                            _gridOffset.vector2Value = new Vector2(-h * cd.x, -v * cd.y);
                            break;
                        case 1:
                            _gridOffset.vector2Value = new Vector2(-h * cd.x / 2, -v * cd.y);
                            break;
                        case 2:
                            _gridOffset.vector2Value = new Vector2(0, -v * cd.y);
                            break;
                        case 3:
                            _gridOffset.vector2Value = new Vector2(-h * cd.x, -v * cd.y / 2);
                            break;
                        case 4:
                            _gridOffset.vector2Value = new Vector2(-h * cd.x / 2, -v * cd.y / 2);
                            break;
                        case 5:
                            _gridOffset.vector2Value = new Vector2(0, -v * cd.y / 2);
                            break;
                        case 6:
                            _gridOffset.vector2Value = new Vector2(-h * cd.x, 0);
                            break;
                        case 7:
                            _gridOffset.vector2Value = new Vector2(-h * cd.x / 2, 0);
                            break;
                        case 8:
                            _gridOffset.vector2Value = new Vector2(0, 0);
                            break;
                    }
                    break;
                case 1:
                    PropertyField(_gridOffset, "Pivot Point : ", "");
                    break;
            }

            // Debug settings
            if (_enableDebugSettings)
            {
                Space(15);
                Heading("Debug Settings");

                Space(10);
                _gridLineColor.colorValue = EditorGUILayout.ColorField("Grid line color : ", _gridLineColor.colorValue);
                _crossLineColor.colorValue = EditorGUILayout.ColorField("Cross line color : ", _crossLineColor.colorValue);
            }

            EditorGUI.EndDisabledGroup();
        }
    }

    public class EditorUtils<TType> : Editor where TType : Object
    {
        public TType Root => (TType)target;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            CustomOnGUI();
            serializedObject.ApplyModifiedProperties();
        }

        public virtual void CustomOnGUI() { }

        public SerializedProperty GetProperty(string propertyName)
            => serializedObject.FindProperty(propertyName);

        public void PropertyField(SerializedProperty property)
            => PropertyField(property, "", "");

        public void PropertyField(SerializedProperty property, string propertyName, string tooltip)
            => EditorGUILayout.PropertyField(property, new GUIContent(propertyName, tooltip));

        public void Info(string info, MessageType type = MessageType.Info)
            => EditorGUILayout.HelpBox(info, type);

        public void PropertySlider(SerializedProperty property, float min, float max, string label)
            => EditorGUILayout.Slider(property, min, max, label);

        public void Space(float val)
            => GUILayout.Space(val);

        public void Heading(string label)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            EditorGUILayout.LabelField(label, style, GUILayout.ExpandWidth(true));
        }
        public bool Button(string content)
            => GUILayout.Button(content);

        public bool Button(string content, float height)
            => GUILayout.Button(content, GUILayout.Height(height));

        public bool Button(string content, float height, float width)
            => GUILayout.Button(content, GUILayout.Height(height), GUILayout.Width(width));

        public int DropdownList(string label, int index, string[] choices)
            => EditorGUILayout.Popup(label, index, choices);

        public void BeginVertical()
            => EditorGUILayout.BeginVertical();

        public void EndVertical()
            => EditorGUILayout.EndVertical();

        public void BeginHorizontal()
            => EditorGUILayout.BeginHorizontal();

        public void EndHorizontal()
            => EditorGUILayout.EndHorizontal();

        public void Label(string labelContent)
            => EditorGUILayout.LabelField(labelContent);
    }
}