using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using con2.messages;
using UnityEngine;
using Random = UnityEngine.Random;

namespace con2.game
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField]
        public float NewtSpawnHeight = 18.0f;

        [SerializeField] [Tooltip("[RANDOM POS. MODE] Boundaries of the map")]
        private float _MinX, _MaxX, _MinZ, _MaxZ;

        [Tooltip("A list of areas to avoid for items spawn")]
        [SerializeField]
        private List<SpawnArea> _ForbiddenAreas;

        private List<Tuple<float, float>> _ForbiddenRangesX;
        private List<Tuple<float, float>> _ForbiddenRangesZ;

        /// <summary>
        /// This list is used to prepare the items in the editor.
        /// Also if UseTimerMode is true.
        /// We can't access it from outside (no way to retrieve an item)
        /// </summary>
        [SerializeField]
        [Tooltip("List of spawnable items")]
        private List<SpawnableItem> SpawnableItemsList = new List<SpawnableItem>();

        public Dictionary<Ingredient, int> SpawnedItemsCount;

        private bool _AllowSpawn = false;
        public bool SpawnFromStart = true;

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
            ComputeForbiddenRanges();
            _AllowSpawn = SpawnFromStart;

            SpawnableItems = new Dictionary<Ingredient, SpawnableItem>();
            SpawnedItemsCount = new Dictionary<Ingredient, int>();
            foreach (var item in SpawnableItemsList)
            {
                SpawnedItemsCount[item.Type] = 0;
                SpawnableItems.Add(item.Type, item);
                item.TimeSinceSpawn = -item.FirstSpawnDelay;
                item.AskToInstantiate += () =>
                {
                    _AllowSpawn = true;
                    InstantiateOnMap(item);
                };
                if (SpawnFromStart)
                {
                    InstantiateOnMap(item);
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

        private void ComputeForbiddenRanges()
        {
            _ForbiddenRangesX = new List<Tuple<float, float>>();
            _ForbiddenRangesZ = new List<Tuple<float, float>>();

            foreach (var area in _ForbiddenAreas)
            {
                var tupleX = new Tuple<float, float>(area.transform.position.x - area.Radius, area.transform.position.x + area.Radius);
                var tupleZ = new Tuple<float, float>(area.transform.position.z - area.Radius, area.transform.position.z + area.Radius);
                _ForbiddenRangesX.Add(tupleX);
                _ForbiddenRangesZ.Add(tupleZ);
            }

        }

        private void UpdateForTimerMode()
        {
            foreach (var item in SpawnableItemsList)
            {
                item.TimeSinceSpawn += Time.deltaTime;
                if (item.UseTimerMode 
                    && item.TimeSinceSpawn >= item.SpawnDelay
                    && SpawnedItemsCount[item.Type] < item.MaxNbOfInstances
                    && _AllowSpawn)
                {
                    InstantiateOnMap(item);
                    item.TimeSinceSpawn = 0f;
                }
            }
        }

        private bool IsValidPosition(float pos)
        {
            foreach (var (item1, item2) in _ForbiddenRangesX)
            {
                if (item1 < pos && pos < item2)
                {
                    return false;
                }
            }

            foreach (var (item1, item2) in _ForbiddenRangesZ)
            {
                if (item1 < pos && pos < item2)
                {
                    return false;
                }
            }
            return true;
        }

        private float FindValidPoint(bool randomPosModeActivated, float min, float max, float areaPoint, float areaRadius, Ingredient itemType)
        {
            var point = float.NaN;
            var trials = 0;
            do
            {
                ++trials;
                var tmp = randomPosModeActivated
                    ? Random.Range(min, max)
                    : Random.Range(
                        areaPoint - areaRadius,
                        areaPoint + areaRadius);
                if (IsValidPosition(tmp))
                {
                    point = tmp;
                }

                if (trials == 10)
                {
                    Debug.LogWarning("A Forbidden zone is overlapping too much with the " + itemType + " spawn zone. Please make the zones smaller or change their position.");
                    point = tmp;
                }
            } while (float.IsNaN(point));

            return point;
        }

        /// <summary>
        /// Actual instantiation on the map
        /// </summary>
        /// <param name="prefab"></param>
        private void InstantiateOnMap(SpawnableItem item)
        {
            if (SpawnedItemsCount[item.Type] >= SpawnableItems[item.Type].MaxNbOfInstances)
            {
                return;
            }

            var position = new Vector3
            {
                x = FindValidPoint(
                    item.ActivateRandomPosMode,
                    _MinX,
                    _MaxX,
                    item.ActivateRandomPosMode ? 0 : item.Area.transform.position.x,
                    item.ActivateRandomPosMode ? 0 : item.Area.Radius,
                    item.Type),
                y = 0.33f,
                z = FindValidPoint(
                    item.ActivateRandomPosMode,
                    _MinZ,
                    _MaxZ,
                    item.ActivateRandomPosMode ? 0 : item.Area.transform.position.z,
                    item.ActivateRandomPosMode ? 0 : item.Area.Radius,
                    item.Type)
            };

            // Drop newt from above and register 2 eyes instead of just 1
            if (item.Type == Ingredient.NEWT_EYE)
            {
                ++SpawnedItemsCount[item.Type];
                position.y = NewtSpawnHeight;
            }
            
            Instantiate(item.Prefab, position, Quaternion.identity);
            ++SpawnedItemsCount[item.Type];
        }

        #endregion

    }

}
