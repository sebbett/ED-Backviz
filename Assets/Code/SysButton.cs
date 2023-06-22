using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysButton : MonoBehaviour
{
    string id;
    SystemDraw sd;

    public void Init(string id, SystemDraw sd)
    {
        this.id = id;
        this.sd = sd;
    }

    private void OnMouseOver()
    {
        Debug.Log(id);
    }

    private void OnMouseUpAsButton()
    {
        sd.onSysButtonClicked(id);
    }
}
