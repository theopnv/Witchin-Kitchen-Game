using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    [System.Serializable]
    public class SpawnableItem
    {
        public GameObject Prefab;

        [Tooltip("[TIMER MODE]Delay between two respawns of this item")]
        public float SpawnDelay;

        [Tooltip("[TRIGGER MODE] If true, will trigger the item's instantiation")]
        public bool InstantiationTrigger;

        [HideInInspector]
        public float TimeSinceSpawn;
    }

}
