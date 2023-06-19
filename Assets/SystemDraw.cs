using UnityEngine;
using eds;
using eds.api;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;

public class SystemDraw : MonoBehaviour
{
    public float scrollSensitivity = 0.1f;
    public string json;
    public string[] requests;
    public GameObject prefab;

    private void Awake()
    {
        GameManager.Events.updateSystems += updateSystems;
        GameManager.Events.updateFactions += updateFactions;
    }

    private void updateFactions(Faction[] factions)
    {
        foreach(Faction f in factions)
        {
            Debug.Log($"{f.name} : {f._id}");
            List<string> systemRequests = new List<string>();
            foreach(Faction.FactionPresence p in f.faction_presence)
            {
                systemRequests.Add(p.system_id);
            }

            if(systemRequests.Count > 0)
            {
                _ = Requests.GetSystemByID(systemRequests.ToArray());
            }
        }
    }

    private void updateSystems(eds.System[] systems)
    {
        foreach(eds.System sys in systems)
        {
            Instantiate(prefab, sys.position, Quaternion.identity);
        }
    }

    private void Start()
    {
        MakeRequest();
    }

    private void MakeRequest()
    {
        Debug.Log($"MakeRequest(): {requests[0]}");
        _ = Requests.GetFactionByName(requests);
    }
}
