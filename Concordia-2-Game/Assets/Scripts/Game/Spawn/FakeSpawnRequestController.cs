using System.Collections;
using System.Collections.Generic;
using con2.game;
using UnityEngine;

namespace con2.game
{

    /// <summary>
    /// This is only for testing purposes (Item spawner)
    /// Remove it as soon as it's not needed anymore
    /// </summary>
    public class FakeSpawnRequestController : MonoBehaviour
    {
        public ItemSpawner ItemSpawner;

        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating("FakeRequestToSpawnAnItem", 0, 8);
        }

        private void FakeRequestToSpawnAnItem()
        {
            ItemSpawner.SpawnableItems["SomeSpawnableItem"].AskToInstantiate();
            ItemSpawner.SpawnableItems["AnotherSpawnableItem"].AskToInstantiate();
        }
    }

}
