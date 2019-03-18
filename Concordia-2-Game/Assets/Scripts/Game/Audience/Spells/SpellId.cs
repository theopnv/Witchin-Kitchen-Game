using System;
using System.Collections.Generic;

namespace con2.game
{

    public class Spells
    {

        public enum SpellID
        {
            disco_mania = 0,
            mega_mage_punch = 1,
            fireball_for_all = 2,
            rocket_speed = 3,
            gift_item = 4,
            gift_bomb = 5,
            invisibility = 6,

            max_id,
        }

        public static Dictionary<SpellID, string> EventList = new Dictionary<SpellID, string>
        {
            { SpellID.disco_mania, "Disco Mania" },
            { SpellID.mega_mage_punch, "Mega Mage Punch" },
            { SpellID.fireball_for_all, "Fireball For All" },
            { SpellID.rocket_speed, "Rocket Speed" },
            { SpellID.gift_item, "Gift" },
            { SpellID.gift_bomb, "Gift" },
            { SpellID.invisibility, "Invisibility" },
        };

    }

}
