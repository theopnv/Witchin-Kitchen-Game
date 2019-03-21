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

        public void InstantiatePlayer(int i, Action onPlayerInitialized = null)
        {
            var instance = Instantiate(_PlayerZonePrefab, GetZoneSpawnPosition(i));
            var playerZoneManager = instance.GetComponent<PlayerZoneManager>();
            playerZoneManager.OnPlayerInitialized += onPlayerInitialized;
            playerZoneManager.OwnerId = i;
            playerZoneManager.PlayerSpawnPosition = GetPlayerSpawnPositionInZone(i);
        }

    }
}