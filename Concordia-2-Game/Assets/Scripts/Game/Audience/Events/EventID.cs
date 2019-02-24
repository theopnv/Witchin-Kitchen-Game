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
            network_ads,

            max_id,
        }

        public static Dictionary<EventID, string> EventList = new Dictionary<EventID, string>
        {
            { EventID.freezing_rain, "Freezing Rain"},
            { EventID.network_ads, "Network Ads"},
        };

    }

}
