using System.Collections;
using System.Collections.Generic;
using System.Linq;
using con2.game;
using con2.main_menu;
using con2.messages;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

namespace con2.lobby
{

    public class LobbyManager : AMainManager
    {
        #region Private Variables

        private float _ServerTryAgainTimeout = 2f;
        [SerializeField] private GameObject _LoadingPanel;
        [SerializeField] private TutorialManager _TutorialManager;
        [SerializeField] private TextMeshProUGUI _InstructionsText;
        private int _MaxPlayers = 4;

        private Dictionary<int, bool> _PlayersStatuses = new Dictionary<int, bool>();
        #endregion

        public GameObject JoinPrompt, TempInvisibleWalls;

        #region Unity API

        protected override void Start()
        {
            base.Start();

            // Subscription to controllers events
            _DetectController.OnDisconnected += OnControllerDisconnected;
            
            // Audience & Networking
            _AudienceInteractionManager.OnDisconnected += OnDisconnectedFromServer;
            _AudienceInteractionManager.OnReceivedMessage += OnReceivedMessage;
            _AudienceInteractionManager.OnReceivedIngredientPollResults += OnReceivedIngredientPollResults;

            var hostAddress = PlayerPrefs.GetString(PlayerPrefsKeys.HOST_ADDRESS) + SocketInfo.SUFFIX_ADDRESS;
            Debug.Log("Host address is: " + hostAddress);

            _AudienceInteractionManager.SetURL(hostAddress);
            ConnectToServer();

            if (!MenuInfo.DoTutorial)
            {
                TempInvisibleWalls.SetActive(false);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartGameLoad();
            }

            if (PlayersInstances.Count < _MaxPlayers)
            {
                if ((Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                    && !PlayersInstances.ContainsKey(0))
                {
                    PressAToJoin(0);
                }
                if ((Input.GetKeyDown(KeyCode.Joystick2Button0) || Input.GetKeyDown(KeyCode.Alpha2))
                    && !PlayersInstances.ContainsKey(1))
                {
                    PressAToJoin(1);
                }
                if ((Input.GetKeyDown(KeyCode.Joystick3Button0) || Input.GetKeyDown(KeyCode.Alpha3))
                    && !PlayersInstances.ContainsKey(2))
                {
                    PressAToJoin(2);
                }
                if ((Input.GetKeyDown(KeyCode.Joystick4Button0) || Input.GetKeyDown(KeyCode.Alpha4))
                    && !PlayersInstances.ContainsKey(3))
                {
                    PressAToJoin(3);
                }
            }
        }

        void OnDisable()
        {
            //_DetectController.OnConnected -= OnControllerConnected;
            _DetectController.OnDisconnected -= OnControllerDisconnected;

            _AudienceInteractionManager.OnDisconnected -= OnDisconnectedFromServer;
            _AudienceInteractionManager.OnReceivedMessage -= OnReceivedMessage;

            _AudienceInteractionManager.OnReceivedIngredientPollResults -= OnReceivedIngredientPollResults;
        }

        #endregion

        #region Network

        void OnDisconnectedFromServer()
        {
            GameInfo.RoomId = "0000";
            GameInfo.Viewers = new List<Viewer>();

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
                PrepareIngredientPoll();
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
                        StartCoroutine(Transition.Get().SequenceIn(null, _LoadGame()));
                        break;
                    default: break;
                }
            }
            else
            {
                Debug.LogError(content.code + ": " + content.content);
            }
        }

        protected IEnumerator _LoadGame()
        {
            SceneManager.LoadSceneAsync(SceneNames.Game);
            yield return null;
        }

        #endregion

        private void PrepareIngredientPoll()
        {
            var ingredientA = Random.Range(0, (int)game.Ingredient.NOT_AN_INGREDIENT);
            int ingredientB;
            do
            {
                ingredientB = Random.Range(0, (int) game.Ingredient.NOT_AN_INGREDIENT);
            } while (ingredientB == ingredientA);

            StartCoroutine(WaitBeforeSendingBecauseIfNotSometimesTheServerHasntMadeTheRoom(ingredientA, ingredientB));
        }

        private IEnumerator WaitBeforeSendingBecauseIfNotSometimesTheServerHasntMadeTheRoom(int a, int b)
        {
            // This method name is way too long but it's 2:30am and at least it's explicit
            yield return new WaitForSeconds(1);
            // I'm not proud of this function though
            _AudienceInteractionManager.SendStartIngredientPoll(a, b);
        }

        private void SetInstructionText()
        {
            _InstructionsText.transform.parent.gameObject.SetActive(true);
            _InstructionsText.text = "Launch a fireball with <sprite=2> to ready up.";
        }

        public override void OnPlayerInitialized(PlayerManager playerManager)
        {
            base.OnPlayerInitialized(playerManager);
            _PlayersStatuses.Add(playerManager.ID, false);

            if (MenuInfo.DoTutorial)
            {
                _TutorialManager.OnPlayerInitialized(playerManager);
            }
            else
            {
                SetInstructionText();
                var fireballManager = playerManager.GetComponentInChildren<PlayerFireball>();
                fireballManager.OnFireballCasted += () => OnFireballCasted(playerManager.ID);
            }

            Debug.Log(playerManager.ID);
            foreach (var p in PlayersInstances)
            {
                p.Value.PlayerHUD.transform.SetSiblingIndex(p.Value.ID);
            }

            JoinPrompt.transform.SetAsLastSibling();
            if (PlayersInstances.Count + 1 >= _MaxPlayers)
                JoinPrompt.SetActive(false);
        }

        public IEnumerator StartGame()
        {
            _InstructionsText.text = "May the best witch or wizard win! \r\nLaunching the game in a few seconds...";

            yield return new WaitForSeconds(3);
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

            for (var i = 0; i < GameInfo.PlayerNumber; i++)
            {
                GamepadMgr.Pad(i).BlockGamepad(true);
            }
            //_LoadingPanel.SetActive(true);
            //_LoadingPanel.GetComponent<LoadingScreenManager>().Title.text = "Loading...";
            _AudienceInteractionManager?.SendPlayerCharacteristics(Ext.ToList(PlayersInstances.Values));
            _AudienceInteractionManager?.SendStopIngredientPoll();
        }

        private void OnReceivedIngredientPollResults(IngredientPoll poll)
        {
            GameInfo.ThemeIngredient = poll.ingredients.OrderByDescending(i => i.votes).First().id;
        }

        #region Controllers

        private void PressAToJoin(int i)
        {
            Debug.Log("Welcome player " + i);
            ++GameInfo.PlayerNumber;
            ActivatePlayer(true, i);
            if (GameInfo.PlayerNumber <= 1 && MenuInfo.DoTutorial)
            {
                _TutorialManager.Run();
            }
        }

        public void OnFireballCasted(int i)
        {
            _PlayersStatuses[i] = true;
            PlayersInstances[i].PlayerHUD.SetReadyActive();

            if (GameInfo.PlayerNumber < 2)
            {
                _InstructionsText.text = "You must be at least 2 players to play the game.";
                return;
            }

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
                if (PlayersInstances.Count + 1 < _MaxPlayers)
                {
                    JoinPrompt.SetActive(true);
                    JoinPrompt.transform.SetAsLastSibling();
                }
            }
        }

        public override List<IInputConsumer> GetInputConsumers(int playerIndex)
        {
            var inputConsumers = new List<IInputConsumer>();

            // Misc.
            var pmi = GetComponent<PauseMenuInstantiator>();
            inputConsumers.Add(pmi);

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

        #endregion

        #region Misc.

        public void BackToMenu()
        {
            StartCoroutine(Transition.Get().SequenceIn(null, _BackToMenu()));
        }

        protected IEnumerator _BackToMenu()
        {
            _AudienceInteractionManager.SendEndGame(false);
            SceneManager.LoadSceneAsync(SceneNames.MainMenu);
            yield return null;
        }

        #endregion

    }

}
