using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    [Serializable]
    public class SpawnableItem
    {
        #region Editor Properties

        public Ingredient Type;

        public GameObject Prefab;

        [Header("Frequency")]
        [Tooltip("Should the item us Timer Mode ? If not checked, will use Trigger Mode")]
        public bool UseTimerMode;

        [Tooltip("[TIMER MODE]Delay between two respawns of this item")]
        public float SpawnDelay;

        [Tooltip("Delay before first spawn so that all items don't spawn on the same second each time.")]
        public float FirstSpawnDelay;

        [Tooltip("Maximum instances of this item on the map")]
        public int MaxNbOfInstances = 1; 

        [Header("Position")]
        [Tooltip("[RANDOM POS. MODE] Will spawn the item randomly in the map")]
        public bool ActivateRandomPosMode;

        [Tooltip("[LOCAL AREA MODE] Will spawn the item around this transform")]
        public SpawnArea Area;

        #endregion

        /// <summary>
        /// [TRIGGER MODE] This event will trigger the instantiation of the item upon invocation.
        /// </summary>
        [HideInInspector]
        public Action AskToInstantiate;

        [HideInInspector]
        public float TimeSinceSpawn;
    }

}
