using System.Collections;
using UnityEngine;


public class InputHandler : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private AStarPathfinder _aStarPathfinder;

    Vector2 _start, _end;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _gridManager.SetStartingPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _gridManager.SetEndingPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            _aStarPathfinder.BeginPathfinding();
        }
        if (Input.GetKeyDown(KeyCode.F))
            _gridManager.ResetGrid();
    }
}
