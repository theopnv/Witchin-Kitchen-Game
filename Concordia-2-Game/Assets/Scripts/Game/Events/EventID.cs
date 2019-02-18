using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public static class Events
    {

        public enum EventID
        {
            freezing_rain = 0,
        }

        public static Dictionary<EventID, string> EventList = new Dictionary<EventID, string>
        {
            { EventID.freezing_rain, "Freezing Rain"},
        };

    }

}
