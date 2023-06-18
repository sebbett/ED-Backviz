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
    }
}
