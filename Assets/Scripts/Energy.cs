using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{
    public void Consume()
    {
        Destroy(gameObject);
    }
}
