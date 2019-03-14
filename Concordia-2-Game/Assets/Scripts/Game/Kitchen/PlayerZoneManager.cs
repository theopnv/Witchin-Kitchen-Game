using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField]
        [Tooltip("List of HUD rectangles icons")]
        public List<Sprite> BackgroundRectangles = new List<Sprite>();

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
            Owner.Name = PlayersInfo.Name[Owner.ID];
            Owner.Color = ColorsManager.Get().PlayerMeshColors[Owner.ID];
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
            Owner.PlayerHUD.OwnerId = Owner.ID;
            var name = instance.transform.Find("Organizer/Recipe/Name");
            name.GetComponentInChildren<Image>().sprite = BackgroundRectangles[Owner.ID];
            name.GetComponentInChildren<Text>().text = Owner.Name;
        }
    }

}
