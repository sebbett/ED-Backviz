using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysButton : MonoBehaviour
{
    public Transform lookTarget;

    public void Init(Transform lookTarget)
    {
        Debug.Log("Init()");
        this.lookTarget = lookTarget;
    }

    private void Update()
    {
        if(lookTarget != null)
        {
            transform.rotation = Quaternion.LookRotation(lookTarget.position - transform.position);
        }
    }
}
