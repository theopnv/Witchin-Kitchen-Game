using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class PlayerZoneManager : MonoBehaviour
    {
        [HideInInspector]
        public int OwnerId;
        [HideInInspector]
        public PlayerManager Owner;
        [HideInInspector]
        public Action OnPlayerInstantiated;

        [SerializeField] private GameObject _PlayerPrefab;
        [SerializeField] private GameObject _PlayerSpawnPosition;
        [SerializeField] private GameObject _PlayerHUDPrefab;

        void Start()
        {
            InitPlayer();
            InitHUD();
            InitKitchen();
            OnPlayerInstantiated?.Invoke();
        }

        void InitPlayer()
        {
            var player = Instantiate(_PlayerPrefab, _PlayerSpawnPosition.transform);
            Owner = player.GetComponent<PlayerManager>();

            Owner.ID = OwnerId;
            Owner.Name = "Player " + Owner.ID;
            Owner.Color = PlayersInfo.Color[Owner.ID];
        }

        void InitKitchen()
        {
            var km = GetComponentInChildren<KitchenManager>();
            km.SetOwner(Owner.ID);
        }

        void InitHUD()
        {
            var playersHUDZone = GameObject.FindGameObjectWithTag(Tags.PLAYERS_HUD_ZONE);
            var instance = Instantiate(_PlayerHUDPrefab, playersHUDZone.transform);
            Owner.PlayerHUD = instance.GetComponent<PlayerHUD>();
        }
    }

}
