using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysButton : MonoBehaviour
{
    public GameObject ps_conflict;
    public GameObject ps_retreat;
    public GameObject ps_expansion;
    public GameObject expansion_cubes;
    private eds.System data;

    public void Init(eds.System newData)
    {
        data = newData;
        if (data.conflicts.Count > 0)
        {
            ps_conflict.SetActive(data.conflicts[0].type == "war" || data.conflicts[0].type == "election" || data.conflicts[0].type == "civilwar");
            ps_expansion.SetActive(data.conflicts[0].type == "retreat");
            ps_retreat.SetActive(data.conflicts[0].type == "expansion");
        }
    }

    private void LateUpdate()
    {
        expansion_cubes.SetActive(data.state == "expansion" && Game.Settings.Visibility.expansionCubesVisible);
    }

    private void OnMouseUpAsButton()
    {
        Game.Events.sysButtonClicked(data);
    }
}
