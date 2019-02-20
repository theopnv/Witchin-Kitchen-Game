using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        [SerializeField]
        private DetectController _DetectController;

        private PlayerUiManager[] _PlayerUiManagers;

        [SerializeField]
        private AudienceInteractionManager _AudienceInteractionManager;

        [SerializeField]
        private Text _ServerWarningText;

        #endregion

        #region Unity API
        
        void Start()
        {
            // Subscription to controllers events
            _DetectController.OnConnected += OnControllerConnected;
            _DetectController.OnDisconnected += OnControllerDisconnected;

            // Player UIs instantiation
            _PlayerUiManagers = new PlayerUiManager[2];
            InstantiatePlayerUi(0, "Player 1", Color.red);
            InstantiatePlayerUi(1, "Player 2", Color.blue);

            // If controllers are already connected we activate players UIs right from the start
            var controllerState = _DetectController.ControllersState;
            for (var i = 0; i < controllerState.Length; i++)
            {
                SetPlayerUiVisibility(controllerState[i], i);
            }
        }

        void Update()
        {
            DevMode();

            if (Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown("joystick button 0"))
            {
                ReadyToStartGame();
            }
        }

        void OnDestroy()
        {
            _DetectController.OnConnected -= OnControllerConnected;
            _DetectController.OnDisconnected -= OnControllerDisconnected;
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
            _PlayerUiManagers[i] = instance.GetComponent<PlayerUiManager>();
            _PlayerUiManagers[i].SetActiveCanvas(false);
            _PlayerUiManagers[i].Label.text = name;

            // TODO: Temporary hard coding the colors for each player. Make that dynamic.
            _PlayerUiManagers[i].Color = color;
            PlayersInfo.Color[i] = color;
        }

        void SetPlayerUiVisibility(bool inLobby, int i)
        {
            _PlayerUiManagers[i].SetActiveCanvas(inLobby);
        }

        private void ReadyToStartGame()
        {
            if (!_AudienceInteractionManager.SendPlayerCharacteristics())
            {
                _ServerWarningText.gameObject.SetActive(true);
            }
            else
            {
                _ServerWarningText.gameObject.SetActive(false);
            }
        }

        public void BackToMenu()
        {
            _AudienceInteractionManager.ExitRoom();
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
