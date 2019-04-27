using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public Camera mainCamera;
    public RectTransform cellContainerRectTransform;
    public Energy energyPrefab;
    public LinkObject cellLinkPrefab;
    public BasicCell basicCellPrefab;
    public List<Cell> availableCells = new List<Cell>();
    public List<Cell> activeCells = new List<Cell>();



    private void Awake()
    {
        instance = this;
        mainCamera = Camera.main;
    }

    void Start()
    {
        
    }

    public Energy CreateEnergy()
    {
        Energy energy = Instantiate(energyPrefab, transform);

        return energy;
    }

    public List<Energy> CreateEnergy(int amount)
    {
        List<Energy> energy = new List<Energy>(amount);

        for (int i = 0; i < amount; i++)
        {
            energy.Add(CreateEnergy());
        }

        return energy;
    }

    public BasicCell CreateBasicCell()
    {
        return Instantiate(basicCellPrefab, cellContainerRectTransform).GetComponent<BasicCell>();
    }

    public LinkObject CreateLinkQuad(Cell cellA, Cell cellB)
    {
        var linkObject = Instantiate(cellLinkPrefab, cellContainerRectTransform).GetComponent<LinkObject>();
        linkObject.Create(cellA, cellB);
        linkObject.transform.SetAsFirstSibling();

        return linkObject;
    }
}
