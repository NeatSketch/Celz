using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    static Settings instance;
    public static Settings Instance { get { return instance; } }

    public float lifetickTime = 1f;

    private void Awake()
    {
        instance = this;
    }
    
}
