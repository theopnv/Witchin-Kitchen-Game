using System.Collections;
using System.Collections.Generic;
using System.Linq;
using con2.game;
using con2.main_menu;
using con2.messages;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

namespace con2.lobby
{

    public class LobbyManager : AMainManager, IInputConsumer
    {
        #region Private Variables

        private float _ServerTryAgainTimeout = 2f;
        [SerializeField] private GameObject _LoadingPanel;
        [SerializeField] private TutorialManager _TutorialManager;
        [SerializeField] private TextMeshProUGUI _InstructionsText;

        private Dictionary<int, bool> _PlayersStatuses = new Dictionary<int, bool>();
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
                if (!MenuInfo.DoTutorial)
                {
                    SetInstructionText();
                }
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

        private void SetInstructionText()
        {
            _InstructionsText.transform.parent.gameObject.SetActive(true);
            _InstructionsText.text = "Launch a fireball with [Right Trigger] when you are ready.";
        }

        public override void OnPlayerInitialized(PlayerManager playerManager)
        {
            base.OnPlayerInitialized(playerManager);
            if (MenuInfo.DoTutorial)
            {
                _TutorialManager.OnPlayerInitialized(playerManager);
            }
            else
            {
                _PlayersStatuses.Add(playerManager.ID, false);
                var fireballManager = playerManager.GetComponentInChildren<PlayerFireball>();
                fireballManager.OnFireballCasted += () => OnFireballCasted(playerManager.ID);
            }
        }

        public IEnumerator StartGame()
        {
            _InstructionsText.text = "May the best win! \r\nLaunching the game in a few seconds...";

            yield return new WaitForSeconds(4);
            _InstructionsText.transform.parent.gameObject.SetActive(false);
            StartGameLoad();
        }

        public void StartGameLoad()
        {
            if (!_AudienceInteractionManager.IsConnectedToServer
                || GameInfo.RoomId == "0000")
            {
                return;
            }
            
            _LoadingPanel.SetActive(true);
            _LoadingPanel.GetComponent<LoadingScreenManager>().Title.text = "Loading...";
            _AudienceInteractionManager?.SendPlayerCharacteristics(Ext.ToList(PlayersInstances.Values));
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
            if (GameInfo.PlayerNumber <= 1 && MenuInfo.DoTutorial)
            {
                _TutorialManager.Run();
            }
        }

        void OnFireballCasted(int i)
        {
            _PlayersStatuses[i] = true;
            PlayersInstances[i].PlayerHUD.SetReadyActive();
            if (_PlayersStatuses.All(p => p.Value))
            {
                StartCoroutine(StartGame());
            }
        }

        void OnControllerDisconnected(int i)
        {
            Debug.Log("Player " + i + " is gone");
            ActivatePlayer(false, i);
            if (_PlayersStatuses.ContainsKey(i))
            {
                _PlayersStatuses.Remove(i);
            }
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
