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
using WebSocketSharp;

namespace con2.lobby
{

    public class LobbyManager : AMainManager, IInputConsumer
    {
        #region Private Variables

        private float _ServerTryAgainTimeout = 2f;
        [SerializeField] private GameObject _LoadingPanel;
        [SerializeField] private TutorialManager _TutorialManager;

        #endregion

        #region Unity API

        protected override void Start()
        {
            base.Start();

            // Subscription to controllers events
            _DetectController.OnConnected += OnControllerConnected;
            _DetectController.OnDisconnected += OnControllerDisconnected;
            
            // Audience & Networking
            _AudienceInteractionManager.OnGameUpdated += OnGameUpdated;
            _AudienceInteractionManager.OnDisconnected += OnDisconnectedFromServer;
            _AudienceInteractionManager.OnReceivedMessage += OnReceivedMessage;

            var hostAddress = PlayerPrefs.GetString(PlayerPrefsKeys.HOST_ADDRESS) + SocketInfo.SUFFIX_ADDRESS;
            Debug.Log("Host address is: " + hostAddress);

            _AudienceInteractionManager.SetURL(hostAddress);
            ConnectToServer();
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartGameLoad();
            }
        }

        void OnDisable()
        {
            _AudienceInteractionManager.OnDisconnected -= OnDisconnectedFromServer;
            _AudienceInteractionManager.OnGameUpdated -= OnGameUpdated;
            _AudienceInteractionManager.OnReceivedMessage -= OnReceivedMessage;
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
                _MessageFeedManager.AddMessageToFeed("Connected to server", MessageFeedManager.MessageType.success);
            }
            else
            {
                _MessageFeedManager.AddMessageToFeed("Can't reach the server", MessageFeedManager.MessageType.error);
                _MessageFeedManager.AddMessageToFeed("Check your internet connection", MessageFeedManager.MessageType.error);
                ConnectToServer();
            }
        }

        private void OnReceivedMessage(messages.Base content)
        {
            if ((int)content.code % 10 == 0) // Success codes always have their unit number equal to 0 (cf. protocol)
            {
                Debug.Log(content.content);
                switch (content.code)
                {
                    case Code.register_players_success:
                        StartCoroutine(ExitLobby());
                        break;
                    default: break;
                }
            }
            else
            {
                Debug.LogError(content.code + ": " + content.content);
            }
        }

        #endregion

        public void StartGameLoad()
        {
            if (!_AudienceInteractionManager.IsConnectedToServer
                || GameInfo.RoomId == "0000")
            {
                return;
            }
            _LoadingPanel.SetActive(true);
            _LoadingPanel.GetComponent<LoadingScreenManager>().Title.text = "Loading...";
            _AudienceInteractionManager?.SendPlayerCharacteristics(PlayersInstances.Values.ToList());
        }

        private IEnumerator ExitLobby()
        {
            for (var i = 0; i < GameInfo.PlayerNumber; i++)
            {
                GamepadMgr.Pad(i).BlockGamepad(true);
            }
            yield return new WaitForSeconds(2f);
            SceneManager.LoadSceneAsync(SceneNames.Game);
        }

        #region Controllers

        void OnControllerConnected(int i)
        {
            Debug.Log("Welcome player " + i);
            ++GameInfo.PlayerNumber;
            ActivatePlayer(true, i);
            _TutorialManager.Run();
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
            var player = PlayersInstances[playerIndex];
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
            if (input.GetActionID() == GamepadAction.ID.START)
            {
                StartGameLoad();
                return true;
            }
            //if (input.GetActionID() == GamepadAction.ID.INTERACT)
            //{
            //    StartGameLoad();
            //    return true;
            //}

            //if (input.GetActionID() == GamepadAction.ID.PUNCH)
            //{
            //    BackToMenu();
            //    return true;
            //}

            return false;
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
