using System;
using System.Collections;
using System.Collections.Generic;
using con2.messages;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        [SerializeField] private GameObject _Kitchen;
        [SerializeField] private GameObject _FemalePrefab;
        [SerializeField] private GameObject _MalePrefab;
        [SerializeField] private GameObject _PlayerHUDPrefab;

        public Action<PlayerManager> OnPlayerInitialized;

        void Start()
        {
            var owner = InitPlayer();
            InitHUD();
            InitKitchen();
            OnPlayerInitialized?.Invoke(owner);
            OnPlayerInitialized = null;
        }

        PlayerManager InitPlayer()
        {
            var player = Instantiate(
                OwnerId % 2 == 0 ? _MalePrefab : _FemalePrefab,
                PlayerSpawnPosition, Quaternion.identity);
            player.transform.forward = -PlayerSpawnPosition;
            player.transform.SetParent(transform);

            Owner = player.GetComponent<PlayerManager>();

            Owner.ID = OwnerId;
            Owner.Name = Players.Info[OwnerId].Name;
            Owner.Texture = ColorsManager.Get().PlayerColorTextures[Owner.ID];
            var normalTex = ColorsManager.Get().PlayerNormalTextures[Owner.ID];
            if (normalTex != null)
                Owner.NormalTexture = normalTex;
            var occlusionTex = ColorsManager.Get().PlayerOcclusionTextures[Owner.ID];
            if (occlusionTex != null)
                Owner.OcclusionTexture = occlusionTex;
            var roughnessTex = ColorsManager.Get().PlayerRoughnessTextures[Owner.ID];
            if (roughnessTex != null)
                Owner.RoughnessTexture = roughnessTex;
            Owner.CompletedPotionCount = 0;
            Owner.CollectedIngredientCount = 0;

            return Owner;
        }

        void InitKitchen()
        {
            _Kitchen.SetActive(true);
            var km = GetComponentInChildren<KitchenManager>();
            km.SetOwner(Owner);
        }

        void InitHUD()
        {
            var playersHUDZone = GameObject.FindGameObjectWithTag(Tags.PLAYERS_HUD_ZONE);
            var instance = Instantiate(_PlayerHUDPrefab, playersHUDZone.transform);
            Owner.PlayerHUD = instance.GetComponent<PlayerHUD>();
            Owner.PlayerHUD.Manager = Owner;
            Owner.PlayerHUD.OwnerId = Owner.ID;
            var backdrop = instance.transform.Find("Organizer");
            var rect = backdrop.GetComponent<Image>();
            rect.color = ColorsManager.Get().PlayerMeshColors[Owner.ID];
        }
    }

}
