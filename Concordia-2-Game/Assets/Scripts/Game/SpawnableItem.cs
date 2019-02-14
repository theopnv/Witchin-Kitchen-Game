using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    [Serializable]
    public class SpawnableItem
    {
        [Tooltip("[TIMER MODE] Used to retrieve the item in the list")]
        public string Name;

        public GameObject Prefab;

        [Tooltip("[TIMER MODE]Delay between two respawns of this item")]
        public float SpawnDelay;

        /// <summary>
        /// [TRIGGER MODE] This event will trigger the instantiation of the item upon invocation.
        /// </summary>
        [HideInInspector]
        public Action AskToInstantiate;

        [HideInInspector]
        public float TimeSinceSpawn;
    }

}
