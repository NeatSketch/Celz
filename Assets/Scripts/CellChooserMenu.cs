using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellChooserMenu : MonoBehaviour
{
    static CellChooserMenu instance;
    public static CellChooserMenu Instance { get { return instance; } }

    public Transform menuContainer;

    Animator animator;
    Cell attachedCell;

    public bool MenuVisible
    {
        get
        {
            return animator.GetBool("Open");
        }

        set
        {
            animator.SetBool("Open", value);
        }
    }

    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        PopulateCellChooser();
    }

    private void Update()
    {
        
    }

    public void AttachToCell(Cell cell)
    {
        transform.localPosition = cell.transform.localPosition;

        MenuVisible = true;

        attachedCell = cell;

        transform.SetAsLastSibling();
    }

    void CreateCellInCellChooser(Cell cellToCreate)
    {
        Cell newCell = Instantiate(cellToCreate, menuContainer);
        newCell.onCellClicked += () => { CellPicked(newCell); };
    }

    void CellPicked(Cell cell)
    {
        attachedCell.SetEvolveTarget(cell);

        CloseCellsChooser();
    }

    void PopulateCellChooser()
    {
        for (int i = 0; i < GameManager.Instance.availableCells.Count; i++)
        {
            CreateCellInCellChooser(GameManager.Instance.availableCells[i]);
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
}
