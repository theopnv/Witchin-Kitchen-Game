using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProBuilder2.Common;
using UnityEngine;

namespace con2.game
{

    public class SpawnPlayersController : MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private GameObject _SpawnPointPrefab;

        [SerializeField] private GameObject _PlayerZonePrefab;

        [SerializeField] private float _KitchensDistanceFromCenter = 4f;

        private List<Transform> _PlayerZoneSpawnPositions;

        private int _NbPlayersInstantiated = 0;

        #endregion

        void Awake()
        {
            PrepareSpawnZone();

            // Initialize players
            for (var i = 0; i < PlayersInfo.PlayerNumber; i++)
            {
                var instance = Instantiate(_PlayerZonePrefab, _PlayerZoneSpawnPositions[i]);
                var playerZoneManager = instance.GetComponent<PlayerZoneManager>();
                playerZoneManager.OwnerId = i;
                playerZoneManager.OnPlayerInstantiated += OnPlayerInstantiated;
            }
        }

        void OnPlayerInstantiated()
        {
            ++_NbPlayersInstantiated;
            // If all players were instantiated
            if (_NbPlayersInstantiated == PlayersInfo.PlayerNumber)
            {
                var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
                //Switch control context to game once players are spawned
                var contextSwitcher = managers.GetComponentInChildren<InputContextSwitcher>();
                contextSwitcher.SetToGameContext();
            }
        }

        private void PrepareSpawnZone()
        {
            Debug.Log(PlayersInfo.PlayerNumber);

            var increment = 360 / (PlayersInfo.PlayerNumber != 0 ? PlayersInfo.PlayerNumber : 1);
            _PlayerZoneSpawnPositions = new List<Transform>();
            for (var i = 0; i < PlayersInfo.PlayerNumber; i++)
            {
                var radians = increment * i * Mathf.Deg2Rad;
                var pos = new Vector3()
                {
                    x = Mathf.Cos(radians),
                    y = 0,
                    z = Mathf.Sin(radians),
                };
                pos *= _KitchensDistanceFromCenter;
                var spawnPoint = Instantiate(_SpawnPointPrefab, pos, Quaternion.identity);
                _PlayerZoneSpawnPositions.Add(spawnPoint.transform);
            }
        }

    }
}
