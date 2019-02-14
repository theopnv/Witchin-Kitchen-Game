using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class ItemSpawner : MonoBehaviour
    {

        [Tooltip("Radius of the spawning zone (circle)")]
        public float Radius;

        [Tooltip("List of spwanable items")]
        public List<SpawnableItem> SpawnableItems = new List<SpawnableItem>();

        [Tooltip("If not checked, will use trigger-based mode")]
        public bool UseTimerMode;

        #region Unity API

        // Start is called before the first frame update
        void Start()
        {
            if (UseTimerMode)
            {
                foreach (var item in SpawnableItems)
                {
                    InstantiateOnMap(item.Prefab);
                    item.TimeSinceSpawn = 0f;
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
            foreach (var item in SpawnableItems)
            {
                item.TimeSinceSpawn += Time.deltaTime;
                if (item.TimeSinceSpawn >= item.SpawnDelay)
                {
                    InstantiateOnMap(item.Prefab);
                    item.TimeSinceSpawn = 0f;
                }
            }
        }

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
