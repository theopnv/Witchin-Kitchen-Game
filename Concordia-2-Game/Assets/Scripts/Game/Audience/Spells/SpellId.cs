using System;
using System.Collections.Generic;

namespace con2.game
{

    public class Spells
    {

        public enum SpellID
        {
            disco_mania = 0,

            max_id,
        }

        public static Dictionary<SpellID, string> EventList = new Dictionary<SpellID, string>
        {
            { SpellID.disco_mania, "Disco Mania" },
        };

    }

}
