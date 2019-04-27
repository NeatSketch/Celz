using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public Energy energyPrefab;
    public List<Cell> activeCells = new List<Cell>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    
}
