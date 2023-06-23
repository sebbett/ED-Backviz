using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using eds;

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
    }
}
