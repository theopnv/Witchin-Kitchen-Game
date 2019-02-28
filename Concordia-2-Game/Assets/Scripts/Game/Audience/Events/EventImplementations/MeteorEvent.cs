using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class MeteorEvent : AbstractAudienceEvent
    {
        public float Angle;

        public int NumberOfMeteors;

        public float duration;

        public float Radius; // Change this if we want a square/rectangular map.

        public int YSpawn;

        [SerializeField]
        private GameObject _MeteorPrefab;

        private List<float> _SpawningFrequencies;
        private int _SpawningFrequenciesIndex = 0;

        void Start()
        {
            SetUpEvent();
        }

        public override void EventStart()
        {
            _SpawningFrequencies = new List<float>(NumberOfMeteors);
            var defaultFrequency = duration / NumberOfMeteors;
            var adjustedFrequency = defaultFrequency / 2;
            for (var i = 0; i < NumberOfMeteors; i++)
            {
                _SpawningFrequencies[i] = defaultFrequency + Random.Range(-adjustedFrequency, adjustedFrequency);
            }
        }

        public override IEnumerator EventImplementation()
        {
            yield return new WaitForSeconds(_SpawningFrequencies[_SpawningFrequenciesIndex++]);

            var groundPosition = new Vector3(
                Random.Range(transform.position.x - Radius, transform.position.x + Radius),
                0,
                Random.Range(transform.position.z - Radius, transform.position.z + Radius));
            var vector3Angle = new Vector3(0, Mathf.Sin(Mathf.Deg2Rad * Angle), Mathf.Cos(Mathf.Deg2Rad * Angle));
            var rayToTest = new Ray(groundPosition, vector3Angle);
            var targetPoint = rayToTest.GetPoint(YSpawn);

            var meteor = Instantiate(_MeteorPrefab, targetPoint, Quaternion.identity);
            var go = new GameObject();
            go.transform.position = groundPosition;
            meteor.GetComponent<Meteor>().GroundTarget = gameObject.transform;

            if (_SpawningFrequenciesIndex < _SpawningFrequencies.Count)
            {
                yield return EventImplementation();
            }

            yield return null;
        }

        public override Events.EventID GetEventID()
        {
            return Events.EventID.meteorites;
        }
    }

}
