using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class ItemSpawner : MonoBehaviour
    {

        [Tooltip("Radius of the spawning zone (circle)")]
        public float Radius;

        [Tooltip("If not checked, will use trigger-based mode")]
        public bool UseTimerMode;

        /// <summary>
        /// This list is used to prepare the items in the editor.
        /// Also if UseTimerMode is true.
        /// We can't access it from outside (no way to retrieve an item)
        /// </summary>
        [SerializeField]
        [Tooltip("List of spwanable items")]
        private List<SpawnableItem> SpawnableItemsList = new List<SpawnableItem>();

        /// <summary>
        /// This dictionary is used if UseTimerMode is false (Trigger Mode)
        /// It's built in Start(), from SpawnableItemsList
        /// From outside we can access any item (with its name) and ask to instantiate it.
        /// </summary>
        [HideInInspector]
        public Dictionary<string, SpawnableItem> SpawnableItems = new Dictionary<string, SpawnableItem>();

        #region Unity API

        // Start is called before the first frame update
        void Start()
        {
            foreach (var item in SpawnableItemsList)
            {
                if (UseTimerMode)
                {
                    InstantiateOnMap(item.Prefab);
                    item.TimeSinceSpawn = 0f;
                }
                else
                {
                    item.AskToInstantiate += () => OnInstantiationAsked(item.Prefab);
                    SpawnableItems.Add(item.Name, item);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (UseTimerMode)
            {
                UpdateForTimerMode();
            }
        }

        #endregion

        #region Custom Methods

        private void UpdateForTimerMode()
        {
            foreach (var item in SpawnableItemsList)
            {
                item.TimeSinceSpawn += Time.deltaTime;
                if (item.TimeSinceSpawn >= item.SpawnDelay)
                {
                    InstantiateOnMap(item.Prefab);
                    item.TimeSinceSpawn = 0f;
                }
            }
        }

        private void OnInstantiationAsked(GameObject prefab)
        {
            InstantiateOnMap(prefab);
        }

        /// <summary>
        /// Actual instantiation on the map
        /// (later on there may be more computation to find a valid position on the map)
        /// </summary>
        /// <param name="prefab"></param>
        private void InstantiateOnMap(GameObject prefab)
        {
            var position = new Vector3();

            position.x = Random.Range(-Radius, Radius);
            position.y = 1f;
            position.z = Random.Range(-Radius, Radius);

            Instantiate(prefab, position, Quaternion.identity);
        }

        #endregion

    }

}
