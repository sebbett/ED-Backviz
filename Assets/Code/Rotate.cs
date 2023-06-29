using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Rotate : MonoBehaviour
{
    public Vector3 speed;

    private void FixedUpdate()
    {
        transform.Rotate(speed);
    }
}
