using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using con2.game;
using con2.messages;
using SocketIO;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.lobby
{

    public class LobbyManager : MonoBehaviour, IInputConsumer
    {
        #region Private Variables

        private int _PlayerNb = 0;
        private Dictionary<int, Tuple<string, Color>> _Players = new Dictionary<int, Tuple<string, Color>>()
        {
            { 0, new Tuple<string, Color>("Gandalf the OG", Color.red) },
            { 1, new Tuple<string, Color>("Sabrina the Tahini Witch", Color.blue) },
            { 2, new Tuple<string, Color>("Snape the Punch-master", Color.green) },
            { 3, new Tuple<string, Color>("Herbione Granger", Color.yellow) },
        };

        [Tooltip("Controllers detector")]
        [SerializeField] private DetectController _DetectController;

        private AudienceInteractionManager _AudienceInteractionManager;

        [SerializeField] private Text _ServerWarningText;

        [SerializeField] private Text _RoomId;
        [SerializeField] private Text _NbViewers;

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
            ActivatePlayer(true, i);
        }

        void OnControllerDisconnected(int i)
        {
            Debug.Log("Player " + i + " is gone");
            ActivatePlayer(false, i);
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

        private void ActivatePlayer(bool activate, int i)
        {
            if (activate)
            {
                ++_PlayerNb;
                GetComponent<SpawnPlayersControllerLobby>().InstantiatePlayer(i);
            }
            else
            {
                --_PlayerNb;
            }
        }

        private void MakePlayerList()
        {
            var playerList = new List<Player>();
            for (var i = 0; i < _PlayerNb; i++)
            {
                PlayersInfo.Name[i] = _Players[i].Item1;
                PlayersInfo.Color[i] = _Players[i].Item2;
                PlayersInfo.PlayerNumber = _PlayerNb;

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

        #region Dev Mode

        void DevMode()
        {
            ActivatePlayersFromKeyboard();
        }

        void ActivatePlayersFromKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ActivatePlayer(true, 0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ActivatePlayer(true, 1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ActivatePlayer(true, 2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ActivatePlayer(true, 3);
            }
        }

        #endregion
    }

}
