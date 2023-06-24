using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysButton : MonoBehaviour
{
    public GameObject ps_war;
    public GameObject ps_civilwar;
    public GameObject ps_election;
    private eds.System data;

    public void Init(eds.System newData)
    {
        data = newData;
        if (data.conflicts.Count > 0)
        {
            ps_war.SetActive(data.conflicts[0].type == "war");
            ps_election.SetActive(data.conflicts[0].type == "election");
            ps_civilwar.SetActive(data.conflicts[0].type == "civilwar");
        }
    }

    private void OnMouseUpAsButton()
    {
        Debug.Log("SysButton.OnMouseUpAsButton()");
        Game.Events.sysButtonClicked(data);
    }
}
