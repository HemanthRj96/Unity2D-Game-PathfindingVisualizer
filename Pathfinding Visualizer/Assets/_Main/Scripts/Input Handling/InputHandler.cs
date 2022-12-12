using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Info = UnityEngine.InputSystem.InputAction.CallbackContext;


public class InputHandler : MonoBehaviour
{
    // Fields

    [SerializeField] private GridManager _gridManager;
    [SerializeField] private AStarPathfinder _aStarPathfinder;

    PV_Actions _input = null;
    bool _canSetPoint;
    bool _canResetPoint;
    float _speedModifier;
    CursorState _currentCursor;


    // Propertites

    public static InputHandler Instance { get; private set; }


    // Properties

    Vector2 mousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);


    // Lifecycle methods

    private void Awake()
    {
        _input = new PV_Actions();

        _input.Interaction.SetPoint.performed += (Info info) => _canSetPoint = true;
        _input.Interaction.SetPoint.canceled += (Info info) => _canSetPoint = false;

        _input.Interaction.ResetPoint.performed += (Info info) => _canResetPoint = true;
        _input.Interaction.ResetPoint.canceled += (Info info) => _canResetPoint = false;

        _input.Interaction.SSRPathfinding.performed += (Info info) => _gridManager.Simulate();

        _input.Interaction.ScrollWheel.performed += (Info info) => _speedModifier = info.ReadValue<float>();
        _input.Interaction.ScrollWheel.canceled += (Info info) => _speedModifier = 0;

        _input.Interaction.Start.performed += (Info info) => _currentCursor = CursorState.setStart;
        _input.Interaction.End.performed += (Info info) => _currentCursor = CursorState.setEnd;
        _input.Interaction.Weight.performed += (Info info) => _currentCursor = CursorState.setWeight;
        _input.Interaction.Wall.performed += (Info info) => _currentCursor = CursorState.setWall;

        _input.Interaction.ClearPath.performed += (Info info) => _gridManager.ClearPath();

        _input.Interaction.ClearWall.performed += (Info info) => _gridManager.ClearWalls();

        _input.Interaction.ClearWeight.performed += (Info info) => _gridManager.ClearWeights();

        _input.Interaction.ClearGrid.performed += (Info info) => _gridManager.ClearGrid();


        _input.Enable();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        var mp = mousePosition;

        if (_canSetPoint)
        {
            if (_currentCursor == CursorState.setStart)
                _gridManager.SetStart(mp);
            else if (_currentCursor == CursorState.setEnd)
                _gridManager.SetEnd(mp);
            else if (_currentCursor == CursorState.setWeight)
                _gridManager.SetWeights(mp);
            else if (_currentCursor == CursorState.setWall)
                _gridManager.SetWalls(mp);
        }
        if (_canResetPoint)
            _gridManager.ResetCell(mp);

        _gridManager.updateDelay = Mathf.Clamp(_gridManager.updateDelay + _speedModifier * Time.deltaTime, 0.001f, 0.2f);
    }

    public enum CursorState
    {
        setStart,
        setEnd,
        setWeight,
        setWall
    }
}
