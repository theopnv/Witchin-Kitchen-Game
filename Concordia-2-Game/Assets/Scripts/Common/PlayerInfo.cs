using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    /// <summary>
    /// Static class to pass player data from lobby scene to game scene
    /// Unfortunately we can't use an array of static PlayerInfo[] because arrays
    /// can't be made of static types. We therefore have to use arrays for each attribute.
    /// </summary>
    public static class PlayersInfo
    {
        public static int PlayerNumber = 2; // Default value
        public static Color[] Color = new Color[PlayerNumber];
        public static string[] Name = new string[PlayerNumber];
    }
}
