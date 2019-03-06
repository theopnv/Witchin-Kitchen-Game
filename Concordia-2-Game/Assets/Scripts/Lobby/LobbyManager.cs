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

    public class LobbyManager : MonoBehaviour
    {
        [Tooltip("The prefab to use as UI for each player")]
        public GameObject PlayerUiPrefab;

        [Tooltip("Position of players' UIs")]
        public Transform[] PlayerUiPositions;
        
        #region Private Variables

        [Tooltip("Controllers detector")]
        [SerializeField] private DetectController _DetectController;

        private AudienceInteractionManager _AudienceInteractionManager;

        [SerializeField] private Text _ServerWarningText;

        [SerializeField] private Text _RoomId;
        [SerializeField] private Text _NbViewers;

        private SocketIOComponent _SocketIoComponent;
        private float _ServerTryAgainTimeout = 2f;
        private PlayerUiManager[] _PlayerUiManagers;

        #endregion

        #region Unity API

        void Start()
        {
            // Subscription to controllers events
            _DetectController.OnConnected += OnControllerConnected;
            _DetectController.OnDisconnected += OnControllerDisconnected;

            // Player UIs instantiation
            _PlayerUiManagers = new PlayerUiManager[2];
            InstantiatePlayerUi(0, "Player 0", Color.red);
            InstantiatePlayerUi(1, "Player 1", Color.blue);

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

            if (Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown("joystick button 0"))
            {
                _AudienceInteractionManager.SendPlayerCharacteristics();
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
            var instance = Instantiate(PlayerUiPrefab, PlayerUiPositions[i]);
            _PlayerUiManagers[i] = instance.GetComponent<PlayerUiManager>();
            _PlayerUiManagers[i].SetActiveCanvas(false);
            _PlayerUiManagers[i].Label.text = name;

            _PlayerUiManagers[i].Color = color;
            PlayersInfo.Color[i] = color;
        }

        void SetPlayerUiVisibility(bool inLobby, int i)
        {
            _PlayerUiManagers[i].SetActiveCanvas(inLobby);
        }

        public void BackToMenu()
        {
            _AudienceInteractionManager.ExitRoom(false);
            SceneManager.LoadSceneAsync(SceneNames.MainMenu);
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
        }

        #endregion
    }

}
