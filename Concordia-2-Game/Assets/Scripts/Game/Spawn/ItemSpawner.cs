using System;
using System.Collections.Generic;
using con2.messages;
using UnityEngine;
using Random = UnityEngine.Random;

namespace con2.game
{
    public class ItemSpawner : MonoBehaviour
    {
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

        private Dictionary<Ingredient, List<GameObject>> _SpawnedItems;

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

            SpawnableItems = new Dictionary<Ingredient, SpawnableItem>();
            _SpawnedItems = new Dictionary<Ingredient, List<GameObject>>();
            foreach (var item in SpawnableItemsList)
            {
                _SpawnedItems.Add(item.Type, new List<GameObject>());

                InstantiateOnMap(item);
                item.TimeSinceSpawn = -item.FirstSpawnDelay;
                item.AskToInstantiate += () => InstantiateOnMap(item);
                SpawnableItems.Add(item.Type, item);
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
                if (item.TimeSinceSpawn >= item.SpawnDelay)
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
                    Debug.LogError("A Forbidden zone is overlapping too much with the " + itemType + " spawn zone. Please make the zones smaller or change their position.");
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
            if (_SpawnedItems[item.Type].Count >= item.MaxNbOfInstances)
            {
                var toRemove = _SpawnedItems[item.Type][0];
                var pickManager = toRemove.gameObject.GetComponent<PickableObject>();
                if (!pickManager)
                {
                    Debug.LogError("Could not find PickableObject component on the ingredient");
                }

                if (pickManager.IsHeld())
                {
                    return;
                }

                _SpawnedItems[item.Type].RemoveAt(0);
                Destroy(toRemove.gameObject);
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
                y = 1f,
                z = FindValidPoint(
                    item.ActivateRandomPosMode,
                    _MinZ,
                    _MaxZ,
                    item.ActivateRandomPosMode ? 0 : item.Area.transform.position.z,
                    item.ActivateRandomPosMode ? 0 : item.Area.Radius,
                    item.Type)
            };

            var instance = Instantiate(item.Prefab, position, Quaternion.identity);
            _SpawnedItems[item.Type].Add(instance);
        }

        #endregion

    }

}
