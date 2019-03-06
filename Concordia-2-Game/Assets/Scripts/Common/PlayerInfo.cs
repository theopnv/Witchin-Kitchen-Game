using System.Collections.Generic;
using con2.messages;
using UnityEngine;

namespace con2
{
    public static class GameInfo
    {
        public static string RoomId = "0000";
        public static List<Viewer> Viewers = new List<Viewer>();
    }

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
