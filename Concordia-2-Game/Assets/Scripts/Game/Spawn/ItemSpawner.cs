using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class ItemSpawner : MonoBehaviour
    {

        [SerializeField] [Tooltip("[RANDOM POS. MODE] Boundaries of the map")]
        private float _MinX, _MaxX, _MinZ, _MaxZ;

        /// <summary>
        /// This list is used to prepare the items in the editor.
        /// Also if UseTimerMode is true.
        /// We can't access it from outside (no way to retrieve an item)
        /// </summary>
        [SerializeField]
        [Tooltip("List of spawnable items")]
        private List<SpawnableItem> SpawnableItemsList = new List<SpawnableItem>();

        /// <summary>
        /// This dictionary is used if UseTimerMode is false (Trigger Mode)
        /// It's built in Start(), from SpawnableItemsList
        /// From outside we can access any item (with its name) and ask to instantiate it.
        /// </summary>
        [HideInInspector]
        public Dictionary<Ingredient, SpawnableItem> SpawnableItems;

        #region Unity API

        // Start is called before the first frame update
        void Start()
        {
            SpawnableItems = new Dictionary<Ingredient, SpawnableItem>();
            foreach (var item in SpawnableItemsList)
            {
                InstantiateOnMap(item);
                if (item.ActivateTimerMode)
                {
                    item.TimeSinceSpawn = -item.FirstSpawnDelay;
                    item.AskToInstantiate += () => { }; // Empty callback
                    SpawnableItems.Add(item.Type, item);
                }
                else
                {
                    item.AskToInstantiate += () => InstantiateOnMap(item);
                    SpawnableItems.Add(item.Type, item);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateForTimerMode();
        }

        #endregion

        #region Custom Methods

        private void UpdateForTimerMode()
        {
            foreach (var item in SpawnableItemsList)
            {
                if (item.ActivateTimerMode)
                {
                    item.TimeSinceSpawn += Time.deltaTime;
                    if (item.TimeSinceSpawn >= item.SpawnDelay)
                    {
                        InstantiateOnMap(item);
                        item.TimeSinceSpawn = 0f;
                    }
                }
            }
        }

        /// <summary>
        /// Actual instantiation on the map
        /// (later on there may be more computation to find a valid position on the map)
        /// </summary>
        /// <param name="prefab"></param>
        private void InstantiateOnMap(SpawnableItem item)
        {
            var position = new Vector3();

            var rangeX = item.ActivateRandomPosMode
                ? Random.Range(_MinX, _MaxX)
                : Random.Range(
                    item.Area.transform.position.x - item.Area.Radius, 
                    item.Area.transform.position.x + item.Area.Radius);
            var rangeZ = item.ActivateRandomPosMode
                ? Random.Range(_MinZ, _MaxZ)
                : Random.Range(
                    item.Area.transform.position.z - item.Area.Radius,
                    item.Area.transform.position.z + item.Area.Radius);
            position.x = rangeX;
            position.y = 1f;
            position.z = rangeZ;

            Instantiate(item.Prefab, position, Quaternion.identity);
        }

        #endregion

    }

}
