using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    /// <summary>
    /// Static class to pass player data from lobby scene to game scene
    /// </summary>
    public static class PlayersInfo
    {
        public static Color[] Color = new Color[2];
        public static string[] Name = new string[2];
    }
}
