using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace con2.game
{

    public class SpawnPlayersController : MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Transform[] _PlayerZoneSpawnPositions;

        [SerializeField] private GameObject _PlayerZonePrefab;

        private int _NbPlayersInstantiated = 0;

        #endregion

        void Start()
        {

            // Initialize players
            for (var i = 0; i < PlayersInfo.PlayerNumber; i++)
            {
                var instance = Instantiate(_PlayerZonePrefab, _PlayerZoneSpawnPositions[i].transform);
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


    }
}
