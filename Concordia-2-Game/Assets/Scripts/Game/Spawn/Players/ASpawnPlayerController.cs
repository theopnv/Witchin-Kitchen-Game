using System;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public abstract class ASpawnPlayerController : MonoBehaviour
    {
        [SerializeField] protected GameObject _PlayerZonePrefab;
        [SerializeField] protected float _KitchensDistanceFromCenter = 4f;
        [SerializeField] protected GameObject _SpawnPointPrefab;

        public abstract Transform GetZoneSpawnPosition(int i);
        public abstract Vector3 GetPlayerSpawnPositionInZone(int i);

        public virtual void Start()
        {
            // Initialize players
            for (var i = 0; i < PlayersInfo.PlayerNumber; i++)
            {
                InstantiatePlayer(i);
            }
        }

        public void InstantiatePlayer(int i, Action onPlayerInitialized = null)
        {
            var instance = Instantiate(_PlayerZonePrefab, GetZoneSpawnPosition(i));
            var playerZoneManager = instance.GetComponent<PlayerZoneManager>();
            playerZoneManager.OwnerId = i;
            playerZoneManager.OnPlayerInitialized += onPlayerInitialized;
            playerZoneManager.PlayerSpawnPosition = GetPlayerSpawnPositionInZone(i);
        }

    }
}