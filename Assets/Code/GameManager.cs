using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using eds;
using Object = UnityEngine.Object;
using Newtonsoft.Json;
using System.IO;

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
        public static Dictionary<string, FactionColor> factionColors { get; private set; }
        static Dictionary<string, GameObject> spawnedSystems = new Dictionary<string, GameObject>();
        static GameObject systemPrefab;
        static Color[] colors;
        static int currentColor = 0;

        public static void Init(GameObject systemButtonPrefab, Color[] factionPallete)
        {
            factionColors = new Dictionary<string, FactionColor>();
            Debug.Log("Game.Manager.Init()");
            systemPrefab = systemButtonPrefab;
            colors = factionPallete;

            //ReadFactions();
            //RedrawSystems(new eds.System[0]);
        }

        public static void AddFactions(Faction[] factions)
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
            Debug.Log("Game.Manager.AddFactions(): Redrawing..");

            List<string> systemRequestIds = new List<string>();
            foreach (KeyValuePair<string, FactionColor> kvp in factionColors)
            {
                foreach (eds.Faction.FactionPresence fp in kvp.Value.faction.faction_presence)
                {
                    if (!systemRequestIds.Contains(fp.system_id)) systemRequestIds.Add(fp.system_id);
                }
            }

            if (systemRequestIds.Count > 0)
                _ = Requests.GetSystemByID(systemRequestIds.ToArray(), (systems) => RedrawSystems(systems));

            //WriteFactions();
        }

        public static void AddFaction(Faction faction)
        {
            if (!factionColors.ContainsKey(faction._id) && factionColors.Count < 20)
            {
                factionColors.Add(faction._id, new FactionColor(faction, colors[currentColor]));
                currentColor++;
            }

            Events.updateGameStatus("Factions found, getting presence..");

            Debug.Log("Game.Manager.AddFaction(): Redrawing..");
            List<string> systemRequestIds = new List<string>();
            foreach (KeyValuePair<string, FactionColor> kvp in factionColors)
            {
                foreach (eds.Faction.FactionPresence fp in kvp.Value.faction.faction_presence)
                {
                    if (!systemRequestIds.Contains(fp.system_id)) systemRequestIds.Add(fp.system_id);
                }
            }

            if (systemRequestIds.Count > 0)
                _ = Requests.GetSystemByID(systemRequestIds.ToArray(), (systems) => RedrawSystems(systems));

            //WriteFactions();
        }

        private static void RedrawSystems(eds.System[] systems)
        {
            foreach(KeyValuePair<string, GameObject> kvp in spawnedSystems)
            {
                Object.Destroy(kvp.Value.gameObject);
            }
            
            spawnedSystems = new Dictionary<string, GameObject>();
            if(systems.Length > 0) Events.sysButtonClicked(systems[0]);
            foreach(eds.System newSystemData in systems)
            {
                GameObject newSystem = Object.Instantiate(systemPrefab, newSystemData.position, Quaternion.identity);
                newSystem.GetComponent<SysButton>().Init(newSystemData);
                string faction = newSystemData.controlling_minor_faction_id;
                foreach(KeyValuePair<string, FactionColor> kvp in factionColors)
                {
                    if(kvp.Key == faction)
                    {
                        newSystem.GetComponent<MeshRenderer>().material.color = factionColors[faction].color;
                    }
                }

                spawnedSystems.Add(newSystemData.id, newSystem);
            }

            Events.updateGameStatus("Done.");
        }

        public static bool FactionIsTracked(string name)
        {
            bool value = false;
            foreach (KeyValuePair<string, FactionColor> kvp in factionColors)
            {
                if (kvp.Value.faction.name == name) value = true;
            }

            return value;
        }

        private static void ReadFactions()
        {
            if (File.Exists("factions.json"))
            {
                StreamReader sr = new StreamReader("factions.json");
                string json = sr.ReadToEnd();
                factionColors = JsonConvert.DeserializeObject<Dictionary<string, FactionColor>>(json);
            }
        }

        private static void WriteFactions()
        {
            string json = JsonConvert.SerializeObject(factionColors);
            StreamWriter sw = new StreamWriter("factions.json");
            sw.Write(json, Formatting.Indented);
            sw.Close();
        }
    }
}
