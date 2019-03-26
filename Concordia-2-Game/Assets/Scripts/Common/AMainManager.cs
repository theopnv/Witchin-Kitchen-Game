using System;
using System.Collections.Generic;
using con2.game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2
{
    public abstract class AMainManager : MonoBehaviour
    {
        [Tooltip("Controllers detector")]
        [SerializeField] protected DetectController _DetectController;
        [SerializeField] protected Text _RoomPin;
        [SerializeField] protected Text _ViewersNb;

        protected AudienceInteractionManager _AudienceInteractionManager;
        protected MessageFeedManager _MessageFeedManager;

        public Dictionary<int, PlayerManager> PlayersInstances = new Dictionary<int, PlayerManager>();

        public abstract List<IInputConsumer> GetInputConsumers(int playerIndex);

        protected virtual void Awake()
        {
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            _MessageFeedManager = FindObjectOfType<MessageFeedManager>();
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }

        public PlayerManager GetPlayerById(int i)
        {
            if (PlayersInstances.ContainsKey(i))
            {
                return PlayersInstances[i];
            }

            return null;
        }

        public virtual void OnPlayerInitialized(PlayerManager player)
        {
            if (!PlayersInstances.ContainsKey(player.ID))
            {
                PlayersInstances.Add(player.ID, player);
            }
            // Reassign Game Context, with newly added input consumers of the player
            // TODO: Optimize this if possible
            FindObjectOfType<InputContextSwitcher>().SetToGameContext(player.ID);
        }

        protected void ActivatePlayer(bool activate, int i)
        {
            if (activate)
            {
                GetComponent<ASpawnPlayerController>().InstantiatePlayer(i, OnPlayerInitialized);
            }
            else
            {
                GamepadMgr.Pad(i).SwitchGamepadContext(new List<IInputConsumer>(), i);
                var playerZone = PlayersInstances[i].GetComponentInParent<PlayerZoneManager>();
                Destroy(playerZone.gameObject);
                PlayersInstances.Remove(i);
                --GameInfo.PlayerNumber;
            }
        }

    }

}