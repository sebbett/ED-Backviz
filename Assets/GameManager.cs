using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using eds;

public static class GameManager
{
    public static class Events
    {
        public delegate void UpdateSystems (eds.System[] systems);
        public static UpdateSystems updateSystems;

        public delegate void UpdateFactions(eds.Faction[] factions);
        public static UpdateFactions updateFactions;
    }

    public static class Data
    {
        public static eds.Faction[] registeredFactions;
        public static eds.System[] registeredSystems;

        public static void Init()
        {
            Events.updateFactions += updateFactions;
            Events.updateSystems += updateSystems;
        }

        private static void updateFactions(Faction[] factions)
        {
            foreach(Faction f in factions)
            {
                bool found = false;
                foreach(Faction ff in registeredFactions)
                {

                }
            }
        }

        private static void updateSystems(eds.System[] systems)
        {
            throw new NotImplementedException();
        }
    }
}
