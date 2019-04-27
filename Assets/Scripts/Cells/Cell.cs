using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [System.Serializable]
    public class Storage
    {
        public int capacity = 100;
        public List<Energy> currentEnergy = new List<Energy>();

        public bool AddEnergy(List<Energy> energy)
        {
            if(currentEnergy.Count + energy.Count <= capacity)
            {
                currentEnergy.AddRange(energy);
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

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ConsumeEnergy(int amount)
        {
            if(currentEnergy.Count >= amount)
            {
                for (int i = 0; i < amount; i++)
                {
                    currentEnergy[i].Consume();
                }

                currentEnergy.RemoveRange(0, amount);

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool placed;
    public int startEnergyAmount = 100;
    public Animator cellAnimator;
    public Storage storage;
    public Text energyCountText;
    public int energyConsumptionPerTick = 1;

    public System.Action onCellPlaced;

    private float lastConsumptionTime;

    protected virtual void Start()
    {
        StartCoroutine(LifeTicker_Routine());
    }

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
        if (energyCountText) energyCountText.text = storage.currentEnergy.Count.ToString();
    }

    protected virtual void LifeTick()
    {
        if(!storage.ConsumeEnergy(energyConsumptionPerTick))
        {
            Die();
        }

        UpdateEnergyCountText();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public virtual void Place()
    {
        if(!placed)
        {
            if (onCellPlaced != null) onCellPlaced.Invoke();
        }
    }

    Vector2 initialMousePos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!placed)
        {
            initialMousePos = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!placed)
        {
            initialMousePos = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!placed)
        {
            //Place();
        }
    }
}
