using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using con2.messages;
using SocketIO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.lobby
{

    public class LobbyManager : MonoBehaviour, IInputConsumer
    {
        [Tooltip("The prefab to use as UI for each player")]
        public GameObject PlayerUiPrefab;

        #region Private Variables

        [SerializeField] private GameObject _PlayersHolder;

        [Tooltip("Controllers detector")]
        [SerializeField] private DetectController _DetectController;

        private AudienceInteractionManager _AudienceInteractionManager;

        [SerializeField] private Text _ServerWarningText;

        [SerializeField] private Text _RoomId;
        [SerializeField] private Text _NbViewers;

        private SocketIOComponent _SocketIoComponent;
        private float _ServerTryAgainTimeout = 2f;
        private List<Tuple<bool, PlayerUiManager>> _PlayerUiManagers;

        #endregion

        #region Unity API

        void Start()
        {
            // Subscription to controllers events
            _DetectController.OnConnected += OnControllerConnected;
            _DetectController.OnDisconnected += OnControllerDisconnected;

            // Player UIs instantiation
            _PlayerUiManagers = new List<Tuple<bool, PlayerUiManager>>(4);
            InstantiatePlayerUi(0, "Gandalf the OG", Color.red);
            InstantiatePlayerUi(1, "Sabrina the Tahini Witch", Color.blue);
            InstantiatePlayerUi(2, "Snape the Punch-master", Color.green);
            InstantiatePlayerUi(3, "Herbione Grainger", Color.yellow);

            // If controllers are already connected we activate players UIs right from the start
            var controllerState = _DetectController.ControllersState;
            for (var i = 0; i < controllerState.Length; i++)
            {
                SetPlayerUiVisibility(controllerState[i], i);
            }

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
            DevMode();

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

        #region Custom Methods

        void OnGameUpdated()
        {
            _RoomId.text = "Room's PIN: " + GameInfo.RoomId;
            _NbViewers.text = "Number of viewers in the room: " + GameInfo.Viewers.Count;
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
            StartCoroutine(CheckServerConnection());
        }

        private IEnumerator CheckServerConnection()
        {
            yield return new WaitForSeconds(_ServerTryAgainTimeout);
            if (!_AudienceInteractionManager.IsConnectedToServer)
            {
                _ServerWarningText.gameObject.SetActive(true);
                ConnectToServer();
            }
            else
            {
                _ServerWarningText.gameObject.SetActive(false);
            }
        }

        void OnControllerConnected(int i)
        {
            Debug.Log("Welcome player " + i);
            SetPlayerUiVisibility(true, i);
        }

        void OnControllerDisconnected(int i)
        {
            Debug.Log("Player " + i + " is gone");
            SetPlayerUiVisibility(false, i);
        }

        void InstantiatePlayerUi(int i, string name, Color color)
        {
            var instance = Instantiate(PlayerUiPrefab, _PlayersHolder.transform);
            var playerUI = new Tuple<bool, PlayerUiManager>(false, instance.GetComponent<PlayerUiManager>());
            playerUI.Item2.SetActiveCanvas(false);
            playerUI.Item2.Label.text = name;
            playerUI.Item2.Color = color;
            _PlayerUiManagers.Add(playerUI);
        }

        void SetPlayerUiVisibility(bool inLobby, int i)
        {
            var tmp = new Tuple<bool, PlayerUiManager>(inLobby, _PlayerUiManagers[i].Item2);
            _PlayerUiManagers[i] = tmp;
            _PlayerUiManagers[i].Item2.SetActiveCanvas(inLobby);
        }

        public void BackToMenu()
        {
            _AudienceInteractionManager.SendEndGame(false);
            SceneManager.LoadSceneAsync(SceneNames.MainMenu);
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

        private void MakePlayerList()
        {
            var playerList = new List<Player>();
            for (var i = 0; i < _PlayerUiManagers.Count; i++)
            {
                if (_PlayerUiManagers[i].Item1)
                {
                    PlayersInfo.Name[i] = _PlayerUiManagers[i].Item2.Label.text;
                    PlayersInfo.Color[i] = _PlayerUiManagers[i].Item2.Color;
                    PlayersInfo.PlayerNumber = i + 1;

                    var player = new Player
                    {
                        id = i,
                        name = PlayersInfo.Name[i],
                        color = "#" + ColorUtility.ToHtmlStringRGBA(PlayersInfo.Color[i])
                    };
                    playerList.Add(player);
                }
            }
            _AudienceInteractionManager?.SendPlayerCharacteristics(playerList);
        }

        #endregion

        #region Dev Mode

        void DevMode()
        {
            ActivatePlayersFromKeyboard();
        }

        void ActivatePlayersFromKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetPlayerUiVisibility(true, 0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetPlayerUiVisibility(true, 1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetPlayerUiVisibility(true, 2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetPlayerUiVisibility(true, 3);
            }
        }

        #endregion

    }

}
