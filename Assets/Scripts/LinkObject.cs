
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkObject : MonoBehaviour
{
    public Cell cellA;
    public Cell cellB;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = (RectTransform)transform;
    }

    public void Create(Cell _cellA, Cell _cellB)
    {
        cellA = _cellA;
        cellB = _cellB;
        SetPositions();
    }

    private void Update()
    {
        SetPositions();
    }

    void SetPositions()
    {
        if(cellA && cellB)
        {
            Vector2 posA = cellA.CellRectTransform.localPosition;
            Vector2 posB = cellB.CellRectTransform.localPosition;
            Vector2 center = Vector2.Lerp(posA, posB, 0.5f);
            rectTransform.localPosition = center;
            Vector2 direction = posB - posA;
            
            Quaternion rotation = Quaternion.LookRotation(direction, rectTransform.up);
            rectTransform.rotation = rotation;
            rectTransform.Rotate(0, 90, 0, Space.Self);

            float distance = Vector3.Distance(posA, posB);
            rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);
        }
    }

    bool destroying;

    public void DestroyLink()
    {
        if(!destroying)
        {
            destroying = true;
            Destroy(gameObject);
        }
    }
}
