using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using con2.messages;
using Newtonsoft.Json;
using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;

namespace con2
{
    /// <summary>
    /// Centralize communication for all scenes.
    /// </summary>
    public partial class AudienceInteractionManager : MonoBehaviour
    {
        private SocketIOComponent _Socket;

        /// <summary>
        /// Ensures that messages are contextualized.
        /// Only relevant messages are received in the current scene.
        /// </summary>
        private Dictionary<string, Delegate> _MessageFunctionMapper;

        public Action OnConnected;
        public Action OnDisconnected;
        public Action OnGameUpdated;

        [HideInInspector] public bool IsConnectedToServer;

        void Start()
        {
            _MessageFunctionMapper = new Dictionary<string, Delegate>()
            {
                { SceneNames.Lobby, (Action<Base>)OnLobbyMessage },
                { SceneNames.Game, (Action<Base>)OnGameMessage },
            };

            _Socket = GetComponent<SocketIOComponent>();

            _Socket.On(Command.CONNECT, e =>
            {
                IsConnectedToServer = true;
                // Small delay between object instantiation and first use.
                StartCoroutine("Authenticate");
                OnConnected?.Invoke();
            });
            _Socket.On(Command.DISCONNECT, e =>
            {
                IsConnectedToServer = false;
                OnDisconnected?.Invoke();
            });

            _Socket.On(Command.MESSAGE, OnMessage);
            _Socket.On(Command.GAME_UPDATE, OnGameUpdate);

            LobbyStart();
            GameStart();
        }

        #region Emit

        /// <summary>
        /// Tell the server that we exited the room and that it can be deleted.
        /// </summary>
        /// <returns></returns>
        public void ExitRoom(bool gameFinished, int winnerIdx = -1)
        {
            if (IsConnectedToServer)
            {
                var winner = gameFinished && winnerIdx != -1
                    ? new Player()
                    {
                        name = PlayersInfo.Name[winnerIdx],
                        color = ColorUtility.ToHtmlStringRGBA(PlayersInfo.Color[winnerIdx]),
                    }
                    : null;
                var gameOutcome = new GameOutcome()
                {
                    gameFinished = gameFinished,
                    winner = winner,
                };
                var serialized = JsonConvert.SerializeObject(gameOutcome);
                _Socket.Emit(Command.QUIT_GAME, new JSONObject(serialized));
            }
        }

        #endregion

        #region Receive

        private void OnMessage(SocketIOEvent e)
        {
            var content = JsonConvert.DeserializeObject<Base>(e.data.ToString());
            var currentSceneName = SceneManager.GetActiveScene().name;

            _MessageFunctionMapper[currentSceneName]?.DynamicInvoke(content);
        }

        private void OnGameUpdate(SocketIOEvent e)
        {
            Debug.Log("OnGameUpdate");
            var game = JsonConvert.DeserializeObject<Game>(e.data.ToString());
            GameInfo.Viewers = game.viewers;
            OnGameUpdated?.Invoke();
        }

        #endregion

        #region Custom Methods

        //private bool IsConnectedToServer()
        //{
        //    return _Socket.sid != null;
        //}

        #endregion
    }

}
