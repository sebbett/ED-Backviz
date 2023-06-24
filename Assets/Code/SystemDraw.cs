using UnityEngine;
using eds;
using eds.api;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class SystemDraw : MonoBehaviour
{
    public List<eds.Faction> registeredFactions = new List<Faction>();
    public List<eds.System> registeredSystems = new List<eds.System>();
    public List<GameObject> systemButtons = new List<GameObject>();
    public Transform map;
    public GameObject systemButtonPrefab;
    public Color[] factionColors;
    public int newColor = -1;

    private void updateFactions(Faction[] factions)
    {
        if (factions.Length > 0)
        {
            newColor++;
            foreach (Faction f in factions)
            {
                bool found = false;
                foreach (Faction rf in registeredFactions)
                {
                    if (rf._id == f._id) found = true;
                }

                if (!found)
                {
                    registeredFactions.Add(f);
                    Debug.Log($"{f.name} : {f._id}");
                    List<string> systemRequests = new List<string>();
                    foreach (Faction.FactionPresence p in f.faction_presence)
                    {
                        systemRequests.Add(p.system_id);
                    }

                    if (systemRequests.Count > 0)
                    {
                        _ = Requests.GetSystemByID(systemRequests.ToArray());
                    }
                }
                else
                {
                    Debug.Log("Faction already shown, consider a refresh instead");
                }
            }
            Game.Events.updateGameStatus("Factions found, getting presence..");
        }
    }
    private void updateSystems(eds.System[] systems)
    {
        Game.Events.updateGameStatus("Systems found, spawning..");
        foreach (eds.System s in systems)
        {
            bool found = false;
            foreach (eds.System rs in registeredSystems) if(rs.id == s.id) found = true;

            if (!found)
            {
                registeredSystems.Add(s);

                Debug.Log($"Spawning: {s.name}");
                GameObject newSystem = Instantiate(systemButtonPrefab, s.position, Quaternion.identity);
                //newSystem.GetComponent<SysButton>().Init(s.id, this);
                newSystem.GetComponent<MeshRenderer>().material.color = factionColors[newColor];
                newSystem.transform.SetParent(map);
            }
        }
        Game.Events.sysButtonClicked(systems[0]);
        Game.Events.updateGameStatus("Done.");
    }
}
