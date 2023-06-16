using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeQueries : MonoBehaviour
{
    void Start()
    {
        Debug.Log(edbgs.GetFactionByName("Aegis Core"));
    }
}
