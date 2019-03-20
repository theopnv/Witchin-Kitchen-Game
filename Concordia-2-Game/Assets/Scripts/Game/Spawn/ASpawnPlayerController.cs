using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public abstract class ASpawnPlayerController : MonoBehaviour
    {
        [SerializeField] protected GameObject _PlayerZonePrefab;
        [SerializeField] protected float _KitchensDistanceFromCenter = 4f;
        [SerializeField] protected GameObject _SpawnPointPrefab;
        protected int _NbPlayersInstantiated = 0;

        public virtual void Awake()
        {
            SwitchToGameContext();
        }

        private void SwitchToGameContext()
        {
            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            //Switch control context to game once players are spawned
            var contextSwitcher = managers.GetComponentInChildren<InputContextSwitcher>();
            contextSwitcher.SetToGameContext();
        }

        public abstract Transform GetZoneSpawnPosition(int i);
        public abstract Vector3 GetPlayerSpawnPositionInZone(int i);

        public void InstantiatePlayer(int i)
        {
            var instance = Instantiate(_PlayerZonePrefab, GetZoneSpawnPosition(i));
            var playerZoneManager = instance.GetComponent<PlayerZoneManager>();
            playerZoneManager.OwnerId = i;
            playerZoneManager.PlayerSpawnPosition = GetPlayerSpawnPositionInZone(i);
        }

    }
}