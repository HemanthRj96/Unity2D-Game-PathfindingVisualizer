using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

public class Cell : MonoBehaviour
{
    // Fields

    [Header("Cell details")]
    public Vector2Int index;
    public Vector2 cellOrigin;
    public bool canWalk;

    [Header("Node details")]
    public float gCost;
    public float hCost;
    public float costMultiplier;
    public Cell fromNode;
    public CellType cellType;
    [SerializeField] private float _defaultWeightAmount;

    [Header("Render settings")]
    public float cellSize;
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Sprite _defaultCell;
    [SerializeField] private Sprite _modifiedCell;
    [SerializeField] private Sprite _selectedCell;
    [SerializeField] private Sprite _blockedCell;
    [SerializeField] private Sprite _weightedCell;
    [SerializeField] private Color _endColor;
    [SerializeField] private Color _startColor;

    int _savedCycle = -1;


    // Properties

    public float fCost { get { return gCost + hCost; } }


    // Public methods

    public void SlowReset()
    {
        cellType = CellType.defaultNull;
        _savedCycle = -1;
        GridManager.manager.onCycleUpdate -= updateColor;

        gCost = int.MaxValue;
        hCost = int.MaxValue;
        costMultiplier = 1;
        fromNode = null;
        canWalk = true;

        transform.DOScale(cellSize * 0.5f, 0.1f).OnComplete(() =>
        {
            _sr.sprite = _defaultCell;
            _sr.color = Color.white;

            transform.DOScale(cellSize, 0.25f).SetEase(Ease.OutBounce);
        });
    }

    public void QuickReset()
    {
        cellType = CellType.defaultNull;
        _savedCycle = -1;
        GridManager.manager.onCycleUpdate -= updateColor;

        gCost = int.MaxValue;
        hCost = int.MaxValue;
        costMultiplier = 1;
        fromNode = null;
        canWalk = true;

        _sr.sprite = _defaultCell;
        _sr.color = Color.white;
    }

    public void SelectCell(Color color)
    {
        if (cellType == CellType.selected)
            return;

        cellType = CellType.selected;
        _sr.sprite = _selectedCell;
        _sr.color = color;

        transform.DOScale(cellSize * 0.5f, 0.1f).OnComplete(() =>
        {
            transform.DOScale(cellSize, 0.25f).SetEase(Ease.OutBounce);
        });
    }


    public void ModifyCell(Color color)
    {
        GridManager.manager.onCycleUpdate += updateColor;
        cellType = CellType.modified;
        _sr.color = color;
        _sr.sprite = _modifiedCell;

        transform.DOScale(cellSize * 0.5f, 0.1f).OnComplete(() =>
        {
            transform.DOScale(cellSize, 0.25f).SetEase(Ease.OutBounce);
        });
    }

    public void BlockCell()
    {
        if (cellType == CellType.wall)
            return;

        cellType = CellType.wall;
        canWalk = false;
        _sr.sprite = _blockedCell;

        transform.DOScale(cellSize * 0.5f, 0.1f).OnComplete(() =>
        {
            transform.DOScale(cellSize, 0.25f).SetEase(Ease.OutBounce);
        });
    }

    public void WeightCell()
    {
        if (cellType == CellType.weight)
            return;

        cellType = CellType.weight;
        costMultiplier = _defaultWeightAmount;
        _sr.sprite = _weightedCell;

        transform.DOScale(cellSize * 0.5f, 0.1f).OnComplete(() =>
        {
            transform.DOScale(cellSize, 0.25f).SetEase(Ease.OutBounce);
        });
    }


    // Private methods

    private void updateColor(int currentCycle)
    {
        if (_savedCycle == -1)
            _savedCycle = currentCycle;

        float t = (float)_savedCycle / currentCycle;

        _sr.color = Color.Lerp(_startColor, _endColor, t);
    }


   // Nested types

    public enum CellType
    {
        defaultNull,
        selected,
        modified,
        wall,
        weight
    }
}