using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using eds;
using Object = UnityEngine.Object;
public static class Game
{
    public static class Events
    {
        public delegate void GenericEvent();
        public static GenericEvent disableMovement;
        public static GenericEvent enableMovement;

        public delegate void UpdateSystems (eds.System[] systems);
        public static UpdateSystems updateSystems;

        public delegate void UpdateFactions(eds.Faction[] factions);
        public static UpdateFactions updateFactions;

        public delegate void SysButtonClicked(eds.System system);
        public static SysButtonClicked sysButtonClicked;

        public delegate void UpdateGameStatus(string status);
        public static UpdateGameStatus updateGameStatus;
    }

    public static class Manager
    {
        static Dictionary<string, FactionColor> factionColors = new Dictionary<string, FactionColor>();
        static Dictionary<string, GameObject> spawnedSystems = new Dictionary<string, GameObject>();
        static GameObject systemPrefab;
        static Color[] colors;
        static int currentColor = 0;

        internal static void Init(GameObject systemButtonPrefab, Color[] factionColors)
        {
            Debug.Log("Init");
            systemPrefab = systemButtonPrefab;
            colors = factionColors;
        }

        public static void AddFactions(Faction[] factions, bool updateSystems)
        {
            foreach(Faction f in factions)
            {
                if(!factionColors.ContainsKey(f._id) && factionColors.Count < 20)
                {
                    factionColors.Add(f._id, new FactionColor(f, colors[currentColor]));
                    currentColor++;
                }
            }

            Events.updateGameStatus("Factions found, getting presence..");

            if (updateSystems)
            {
                List<string> systemRequestIds = new List<string>();
                foreach(KeyValuePair<string, FactionColor> kvp in factionColors)
                {
                    foreach(eds.Faction.FactionPresence fp in kvp.Value.faction.faction_presence)
                    {
                        if (!systemRequestIds.Contains(fp.system_id)) systemRequestIds.Add(fp.system_id);
                    }
                }

                if (systemRequestIds.Count > 0)
                    _ = Requests.GetSystemByID(systemRequestIds.ToArray(), (systems) => RedrawSystems(systems));
            }
        }

        private static void RedrawSystems(eds.System[] systems)
        {
            foreach(KeyValuePair<string, GameObject> kvp in spawnedSystems)
            {
                Object.Destroy(kvp.Value.gameObject);
            }
            
            spawnedSystems = new Dictionary<string, GameObject>();
            Events.sysButtonClicked(systems[0]);
            foreach(eds.System newSystemData in systems)
            {
                GameObject newSystem = Object.Instantiate(systemPrefab, newSystemData.position, Quaternion.identity);
                newSystem.GetComponent<SysButton>().Init(newSystemData);
                string faction = newSystemData.controlling_minor_faction_id;
                foreach(KeyValuePair<string, FactionColor> kvp in factionColors)
                {
                    if(kvp.Key == faction)
                    {
                        Debug.Log("This other thing");
                        newSystem.GetComponent<MeshRenderer>().material.color = factionColors[faction].color;
                    }
                }

                spawnedSystems.Add(newSystemData.id, newSystem);
            }

            Events.updateGameStatus("Done.");
        }
    }
}
