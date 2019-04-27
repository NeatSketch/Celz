using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainCell : Cell
{
    public Animator cellsChooserAnimator;
    public Transform cellChooser;
    public List<Cell> availableCells = new List<Cell>();

    private List<Cell> cellsInCellChooser = new List<Cell>();

    protected override void Start()
    {
        base.Start();

    }
    
    public bool MenuVisible
    {
        get
        {
            return cellsChooserAnimator.GetBool("Open");
        }

        set
        {
            cellsChooserAnimator.SetBool("Open", value);
        }
    }

    public void OnCellPlaced(Cell cell, int cellChooserPosition)
    {
        GameManager.Instance.activeCells.Add(cell);
        CreateCellInCellChooser(cell, cellChooserPosition);
    }

    void CreateCellInCellChooser(Cell cellToCreate, int cellChooserPosition)
    {
        Cell newCell = Instantiate(cellToCreate, cellChooser);
        newCell.transform.SetSiblingIndex(cellChooserPosition);
        newCell.onCellPlaced += () => { OnCellPlaced(cellToCreate, cellChooserPosition); };
        cellsInCellChooser[cellChooserPosition] = newCell;
    }

    void PopulateCellChooser()
    {
        for (int i = 0; i < availableCells.Count; i++)
        {
            CreateCellInCellChooser(availableCells[i], i);
        }
    }

    public void OpenCellsChooser()
    {
        MenuVisible = true;
    }

    public void CloseCellsChooser()
    {
        MenuVisible = false;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (!MenuVisible)
        {
            OpenCellsChooser();
        }
        else
        {
            CloseCellsChooser();
        }
    }
}
