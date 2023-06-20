using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickSetTarget : MonoBehaviour
{
    MouseOrbit mo;

    private void Start()
    {
        mo = Camera.main.GetComponent<MouseOrbit>();
    }

    private void OnMouseUpAsButton()
    {
        mo.SetTarget(transform.position);
    }
}
