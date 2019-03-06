using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class MeteorEvent : AbstractAudienceEvent
    {
        public float Angle;

        public int NumberOfMeteors;

        public float Duration;

        public float Radius; // Change this if we want a square/rectangular map.

        public int YSpawn;

        public float MeteoriteSpeed;

        [SerializeField]
        private GameObject _MeteorPrefab;

        private Queue<float> _SpawningFrequencies;

        void Start()
        {
            SetUpEvent();
        }

        public override void EventStart()
        {
            _SpawningFrequencies = new Queue<float>();
            var defaultFrequency = Duration / NumberOfMeteors; // e.g 15 meteors in 10 seconds = 1.5 meteors in 1 sec
            var adjustedFrequency = defaultFrequency / 2; // Randomize the frequencies a bit (make spawning more natural)
            for (var i = 0; i < NumberOfMeteors; i++)
            {
                _SpawningFrequencies.Enqueue(defaultFrequency + Random.Range(-adjustedFrequency, adjustedFrequency));
            }
        }

        public override IEnumerator EventImplementation()
        {
            yield return new WaitForSeconds(_SpawningFrequencies.Dequeue());

            var groundPosition = new Vector3(
                Random.Range(transform.position.x - Radius, transform.position.x + Radius),
                0,
                Random.Range(transform.position.z - Radius, transform.position.z + Radius));

            var vector3Angle = new Vector3(
                Mathf.Cos(Mathf.Deg2Rad * Angle), 
                Mathf.Sin(Mathf.Deg2Rad * Angle), 
                Mathf.Cos(Mathf.Deg2Rad * Angle));
            var ray = new Ray(groundPosition, vector3Angle);
            var targetPoint = ray.GetPoint(YSpawn);
            Debug.DrawLine(groundPosition, targetPoint, Color.green, 10);

            var meteor = Instantiate(_MeteorPrefab, targetPoint, Quaternion.identity);
            var manager = meteor.GetComponent<Meteor>();
            manager.Speed = MeteoriteSpeed;
            manager.SetDirection(groundPosition);

            if (_SpawningFrequencies.Count > 0)
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
