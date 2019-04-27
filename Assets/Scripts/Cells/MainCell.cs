using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainCell : Cell
{
    protected override void Start()
    {
        AddEnergy(GameManager.Instance.CreateEnergy(startEnergyAmount));
        base.Start();
    }
}
