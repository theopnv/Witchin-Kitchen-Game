using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

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
        [SerializeField]
        private DetectController _detectController;

        private PlayerUiManager[] _playerUiManagers;

        #endregion

        #region Unity API
        
        void Start()
        {
            // Subscription to controllers events
            _detectController.OnConnected += OnControllerConnected;
            _detectController.OnDisconnected += OnControllerDisconnected;

            // Player UIs instantiation
            _playerUiManagers = new PlayerUiManager[2];
            InstantiatePlayerUi(0, "Player 1", Color.red);
            InstantiatePlayerUi(1, "Player 2", Color.blue);

            // If controllers are already connected we activate players UIs right from the start
            var controllerState = _detectController.ControllersState;
            for (var i = 0; i < controllerState.Length; i++)
            {
                SetPlayerUiVisibility(controllerState[i], i);
            }
        }

        void Update()
        {
            DevMode();
        }

        void OnDestroy()
        {
            _detectController.OnConnected -= OnControllerConnected;
            _detectController.OnDisconnected -= OnControllerDisconnected;
        }

        #endregion

        #region Custom Methods

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
            _playerUiManagers[i] = instance.GetComponent<PlayerUiManager>();
            _playerUiManagers[i].Label.text = name;
            // TODO: Temporary hard coding the colors for each player. Make that dynamic.
            _playerUiManagers[i].Color = color;
        }

        void SetPlayerUiVisibility(bool inLobby, int i)
        {
            _playerUiManagers[i].SetActiveCanvas(inLobby);
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
