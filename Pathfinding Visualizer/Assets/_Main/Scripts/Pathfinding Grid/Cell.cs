using UnityEngine;


public class Cell : MonoBehaviour
{
    // Fields

    [Header("Cell details")]
    public Vector2Int index;
    public Vector2 cellOrigin;
    public bool isValid;

    [Header("Node details")]
    public float gCost;
    public float hCost;
    public float costMultiplier;
    public Cell fromNode;

    [Header("Render settings")]
    public SpriteRenderer sr;


    // Properties

    public float fCost { get { return gCost + hCost; } }


    // Public methods

    public void OnGridResetCallback()
    {
        // callback to rest all cells that has been modified
        gCost = int.MaxValue;
        hCost = int.MaxValue;
        costMultiplier = 1;
        fromNode = null;

        sr.color = Color.white;
    }

    public void OnColorUpdate()
    {
        // callback to control the color of the cells
    }


    // Lifecycle methods
}