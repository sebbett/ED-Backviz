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
        //TODO: Move the bulk of what's being done in SystemDraw.cs here, we don't need
        //a monob handling the kinds of things is done. Have SystemDraw init this class
        //With the values it needs, like the button prefabs, colors, map canvas, and
        //camera, but the functionality of SystemDraw can be done here.

        //Also need to figure out why spawned buttons don't seem to receive raycasts, but
        //buttons placed manually do?
    }
}
