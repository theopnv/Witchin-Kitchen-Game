using System;
using System.Collections;
using System.Collections.Generic;
using con2.messages;
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
        public Vector3 PlayerSpawnPosition;

        [SerializeField] private GameObject _FemalePrefab;
        [SerializeField] private GameObject _MalePrefab;
        [SerializeField] private GameObject _PlayerHUDPrefab;

        [SerializeField]
        [Tooltip("List of HUD rectangles icons")]
        public List<Sprite> BackgroundRectangles = new List<Sprite>();

        public Action OnPlayerInitialized;

        void Start()
        {
            InitPlayer();
            InitHUD();
            InitKitchen();
            OnPlayerInitialized?.Invoke();
        }

        void InitPlayer()
        {
            var player = Instantiate(
                OwnerId % 2 == 0 ? _MalePrefab : _FemalePrefab,
                PlayerSpawnPosition, Quaternion.identity);
            player.transform.forward = -PlayerSpawnPosition;

            Owner = player.GetComponent<PlayerManager>();

            Owner.ID = OwnerId;
            Owner.Name = PlayersInfo.Name[Owner.ID];
            Owner.Color = ColorsManager.Get().PlayerMeshColors[Owner.ID];
            Owner.CompletedPotionCount = 0;
            Owner.CollectedIngredientCount = 0;
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
            var rect = name.GetComponentInChildren<Image>();
            rect.sprite = BackgroundRectangles[Owner.ID];
            rect.color = ColorsManager.Get().PlayerMeshColors[Owner.ID];
            name.GetComponentInChildren<Text>().text = Owner.Name;
        }
    }

}
