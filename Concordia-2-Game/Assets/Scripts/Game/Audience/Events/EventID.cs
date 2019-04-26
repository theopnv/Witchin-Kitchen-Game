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
            network_ads = 1,
            meteorites = 2,
            ingredient_morph = 3,
            kitchen_spin = 4,
            ingredient_dance = 5,
            grass_growth = 6,

            max_id,
        }

        public static Dictionary<EventID, string> EventList = new Dictionary<EventID, string>
        {
            { EventID.freezing_rain, "Freezing Rain"},
            { EventID.network_ads, "Network Ads"},
            { EventID.meteorites, "Meteorites"},
            { EventID.ingredient_morph, "Ingredient Morph"},
            { EventID.kitchen_spin, "Kitchen Spin"},
            { EventID.ingredient_dance, "Ingredient Dance"},
            { EventID.grass_growth, "Grass Growth"},
        };

    }

}
