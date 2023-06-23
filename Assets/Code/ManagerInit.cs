using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerInit : MonoBehaviour
{
    public GameObject systemButtonPrefab;
    public Color[] factionColors;

    private void Awake()
    {
        Game.Manager.Init(systemButtonPrefab, factionColors);
    }
}
