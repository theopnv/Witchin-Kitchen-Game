using System;
using System.Collections.Generic;
using con2.messages;
using UnityEngine;

namespace con2
{
    public static class GameInfo
    {
        public static string RoomId = "0000";
        public static List<Viewer> Viewers = new List<Viewer>();
        public static int PlayerNumber = 0;
    }

    public class Player
    {
        public string Name;
    }

    public static class Players
    {
        public static Dictionary<int, Player> Info = new Dictionary<int, Player>()
        {
            { 0, new Player() { Name = "Red"} },
            { 1, new Player() { Name = "Blue"} },
            { 2, new Player() { Name = "Green"} },
            { 3, new Player() { Name = "Yellow"} },
        };

    }

}
