using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool placed;
    public int createPrice = 10;
    public int startEnergyAmount = 100;
    public Text energyCountText;
    public int energyConsumptionPerTick = 1;
    public Dictionary<Cell, LinkObject> nextCells = new Dictionary<Cell, LinkObject>();

    public System.Action onCellPlaced;

    private float lastConsumptionTime;
    private BasicCell basicCell;
    private Animator cellAnimator;
    private RectTransform rectTransform;

    public RectTransform CellRectTransform
    {
        get
        {
            return rectTransform;
        }
    }

    protected virtual void Awake()
    {
        if (!cellAnimator) cellAnimator = GetComponent<Animator>();
        if (!rectTransform) rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void OnDestroy()
    {
        GameManager.Instance.UnregisterCell(this);
    }

    protected virtual void Start()
    {
        GameManager.Instance.RegisterCell(this);

        StartCoroutine(LifeTicker_Routine());
    }

    #region Energy Storage

    [Header("Energy Storage")]
    public int capacity = 100;
    public List<Energy> currentEnergy = new List<Energy>();

    public bool AddEnergy(List<Energy> energy)
    {
        if (currentEnergy.Count + energy.Count <= capacity)
        {
            currentEnergy.AddRange(energy);
            UpdateEnergyCountText();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool WithdrawEnergy(int amount, out List<Energy> energy)
    {
        energy = new List<Energy>();

        if (currentEnergy.Count >= amount)
        {
            energy.AddRange(currentEnergy.GetRange(0, amount));
            currentEnergy.RemoveRange(0, amount);
            UpdateEnergyCountText();

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ConsumeEnergy(int amount)
    {
        if (currentEnergy.Count >= amount)
        {
            for (int i = 0; i < amount; i++)
            {
                currentEnergy[i].Consume();
            }

            currentEnergy.RemoveRange(0, amount);
            UpdateEnergyCountText();

            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Linking Cells

    public void AddLinkCell(Cell cell, LinkObject linkObject)
    {
        if (!nextCells.ContainsKey(cell))
        {
            nextCells.Add(cell, linkObject);
        }
    }

    public void UnlinkCell(Cell cell)
    {
        if (nextCells.ContainsKey(cell))
        {
            nextCells[cell].DestroyLink();
            nextCells.Remove(cell);
        }
    }

    public void LinkCells(Cell cellA, Cell cellB)
    {
        LinkObject linkObject = GameManager.Instance.CreateLinkQuad(cellA, cellB);

        cellA.AddLinkCell(cellB, linkObject);
        cellB.AddLinkCell(cellA, linkObject);
    }

    public void UnlinkCells(Cell cellA, Cell cellB)
    {
        cellA.UnlinkCell(cellB);
        cellB.UnlinkCell(cellA);
    }

    public void UnlinkAllCells()
    {
        var nextCellsKeys = nextCells.Keys.ToList();

        foreach (var key in nextCellsKeys)
        {
            UnlinkCells(this, key);
        }
    }

    #endregion

    IEnumerator LifeTicker_Routine()
    {
        float delay = 0;

        while (true)
        {
            if (placed)
            {
                LifeTick();

                delay = Settings.Instance.lifetickTime;
                while (delay > 0)
                {
                    delay -= Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    protected virtual void UpdateEnergyCountText()
    {
        if (energyCountText) energyCountText.text = currentEnergy.Count.ToString();
    }

    protected virtual void LifeTick()
    {
        if (!ConsumeEnergy(energyConsumptionPerTick))
        {
            Die();
        }

        UpdateEnergyCountText();
    }

    protected virtual void Die()
    {
        UnlinkAllCells();
        Destroy(gameObject);
    }

    public virtual void Place()
    {
        if (!placed)
        {
            if (onCellPlaced != null) onCellPlaced.Invoke();

            placed = true;

            OpenCellChooser();
        }
    }

    void OpenCellChooser()
    {
        if (this is BasicCell)
        {
            CellChooserMenu.Instance.AttachToCell(this);
        }
    }

    bool GetMousePositionOnContainer(out Vector2 point)
    {
        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.Instance.cellContainerRectTransform,
                Input.mousePosition, GameManager.Instance.mainCamera, out point);

        Debug.LogFormat("Success: {0} MousePos: {1} point: {2}", success, Input.mousePosition, point);

        return success;
    }

    #region Pointer Events

    Vector2 initialTouchPos;

    protected virtual void CreateBasicCell()
    {
        List<Energy> energy = new List<Energy>();
        if (WithdrawEnergy(Settings.Instance.basicCellEnergyCost, out energy))
        {
            basicCell = GameManager.Instance.CreateBasicCell();
            basicCell.rectTransform.anchoredPosition = rectTransform.anchoredPosition;
            basicCell.AddEnergy(energy);
            LinkCells(this, basicCell);
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (placed && !basicCell)
        {
            CreateBasicCell();
        }
    }

    public System.Action onCellClicked;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!placed)
        {
            if (onCellClicked != null) onCellClicked.Invoke();
        }
        else
        {
            OpenCellChooser();
        }
    }

    private const int NON_EXISTING_TOUCH = -98456;
    private int pointerId = NON_EXISTING_TOUCH;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!placed)
        {
            if (pointerId != NON_EXISTING_TOUCH)
            {
                eventData.pointerDrag = null;
                return;
            }

            pointerId = eventData.pointerId;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out initialTouchPos);

            Debug.Log(name + " OnBeginDrag", gameObject);
        }

        if (basicCell)
        {
            basicCell.OnBeginDrag(eventData);
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!placed)
        {
            if (eventData.pointerId != pointerId)
                return;

            Vector2 touchPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out touchPos);

            rectTransform.anchoredPosition += touchPos - initialTouchPos;
        }

        if (basicCell)
        {
            basicCell.OnDrag(eventData);
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!placed)
        {
            if (eventData.pointerId != pointerId)
                return;

            pointerId = NON_EXISTING_TOUCH;

            Place();
        }

        if (basicCell)
        {
            basicCell.OnEndDrag(eventData);
            basicCell = null;
        }
    }

    #endregion    

    public void OnCellPlaced(Cell cell, int cellChooserPosition)
    {
        GameManager.Instance.activeCells.Add(cell);
    }

    #region Evolving

    Cell evolveTarget;

    public void SetEvolveTarget(Cell targetCell)
    {
        evolveTarget = targetCell;
    }

    #endregion
}
