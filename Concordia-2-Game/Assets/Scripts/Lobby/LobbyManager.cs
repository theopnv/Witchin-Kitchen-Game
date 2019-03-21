using System;
using System.Collections;
using System.Collections.Generic;
using con2.game;
using con2.messages;
using SocketIO;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.lobby
{

    public class LobbyManager : AMainManager, IInputConsumer
    {
        #region Private Variables
        
        [Tooltip("Controllers detector")]
        [SerializeField] private DetectController _DetectController;
        [SerializeField] private Text _RoomPin;
        [SerializeField] private Text _ViewersNb;

        private AudienceInteractionManager _AudienceInteractionManager;
        private MessageFeedManager _MessagefeedManager;
        private SocketIOComponent _SocketIoComponent;
        private float _ServerTryAgainTimeout = 2f;

        #endregion

        #region Unity API

        void Start()
        {
            // Subscription to controllers events
            _DetectController.OnConnected += OnControllerConnected;
            _DetectController.OnDisconnected += OnControllerDisconnected;

            // If controllers are already connected we activate players UIs right from the start
            var controllerState = _DetectController.ControllersState;
            for (var i = 0; i < controllerState.Length; i++)
            {
                ActivatePlayer(controllerState[i], i);
            }

            _MessagefeedManager = GetComponent<MessageFeedManager>();

            // Audience & Networking
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            _AudienceInteractionManager.OnGameUpdated += OnGameUpdated;
            _AudienceInteractionManager.OnDisconnected += OnDisconnectedFromServer;
            var hostAddress = PlayerPrefs.GetString(PlayerPrefsKeys.HOST_ADDRESS) + SocketInfo.SUFFIX_ADDRESS;
            Debug.Log("Host address is: " + hostAddress);

            _AudienceInteractionManager.SetURL(hostAddress);
            ConnectToServer();
        }

        void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                MakePlayerList();
            }
        }

        void OnDisable()
        {
            _AudienceInteractionManager.OnDisconnected -= OnDisconnectedFromServer;
            _AudienceInteractionManager.OnGameUpdated -= OnGameUpdated;
        }

        #endregion

        #region Network

        void OnGameUpdated()
        {
            _RoomPin.text = GameInfo.RoomId;
            _ViewersNb.text = GameInfo.Viewers.Count.ToString();
        }

        void OnDisconnectedFromServer()
        {
            GameInfo.RoomId = "0000";
            GameInfo.Viewers = new List<Viewer>();
            OnGameUpdated();

            ConnectToServer();
        }

        void ConnectToServer()
        {
            _AudienceInteractionManager.Connect();
            if (!_AudienceInteractionManager.IsConnectedToServer)
            {
                StartCoroutine(CheckServerConnection());
            }
        }

        private IEnumerator CheckServerConnection()
        {
            yield return new WaitForSeconds(_ServerTryAgainTimeout);
            if (_AudienceInteractionManager.IsConnectedToServer)
            {
                _MessagefeedManager.AddMessageToFeed("Connected to server", MessageFeedManager.MessageType.success);
            }
            else
            {
                _MessagefeedManager.AddMessageToFeed("Can't reach the server", MessageFeedManager.MessageType.error);
                _MessagefeedManager.AddMessageToFeed("Check your internet connection", MessageFeedManager.MessageType.error);
                ConnectToServer();
            }
        }

        #endregion

        #region Controllers

        void OnControllerConnected(int i)
        {
            Debug.Log("Welcome player " + i);
            ActivatePlayer(true, i);
        }

        void OnControllerDisconnected(int i)
        {
            Debug.Log("Player " + i + " is gone");
            ActivatePlayer(false, i);
        }

        public override List<IInputConsumer> GetInputConsumers(int playerIndex)
        {
            var inputConsumers = new List<IInputConsumer>();

            // Misc.
            inputConsumers.Add(this);

            // Fight
            var player = Players[playerIndex];
            inputConsumers.Add(player.GetComponent<FightStun>());

            // Kitchens
            var kitchenParents = GameObject.FindGameObjectsWithTag(Tags.KITCHEN);
            var kitchenStations = new List<ACookingMinigame>();
            foreach (var kitchen in kitchenParents)
            {
                var stations = kitchen.GetComponentsInChildren<ACookingMinigame>();
                kitchenStations.AddRange(stations);
            }

            inputConsumers.AddRange(kitchenStations);

            // Players
            inputConsumers.Add(player.GetComponent<PlayerInputController>());

            return inputConsumers;
        }

        public bool ConsumeInput(GamepadAction input)
        {
            if (input.GetActionID() == GamepadAction.ID.INTERACT)
            {
                MakePlayerList();
                return true;
            }

            if (input.GetActionID() == GamepadAction.ID.PUNCH)
            {
                BackToMenu();
                return true;
            }

            return false;
        }

        #endregion

        #region Players

        private void MakePlayerList()
        {
            var playerList = new List<Player>();
            for (var i = 0; i < PlayersInfo.PlayerNumber; i++)
            {
                var player = new Player
                {
                    id = i,
                    name = PlayersInfo.Name[i],
                    color = "#" + ColorUtility.ToHtmlStringRGBA(PlayersInfo.Color[i])
                };
                playerList.Add(player);
            }
            _AudienceInteractionManager?.SendPlayerCharacteristics(playerList);
        }

        #endregion

        #region Misc.

        public void BackToMenu()
        {
            _AudienceInteractionManager.SendEndGame(false);
            SceneManager.LoadSceneAsync(SceneNames.MainMenu);
        }

        #endregion

    }

}
