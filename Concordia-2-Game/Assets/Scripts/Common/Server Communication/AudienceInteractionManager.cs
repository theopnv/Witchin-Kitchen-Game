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

        void Start()
        {
            _MessageFunctionMapper = new Dictionary<string, Delegate>()
            {
                { SceneNames.Lobby, (Action<Base>)OnLobbyMessage },
                { SceneNames.Game, (Action<Base>)OnGameMessage },
            };

            _Socket = GetComponent<SocketIOComponent>();

            _Socket.On(Command.MESSAGE, OnMessage);

            LobbyStart();
            GameStart();
        }

        #region Emit

        /// <summary>
        /// Teel the server that we exited the room and that it can be deleted.
        /// </summary>
        /// <returns></returns>
        public void ExitRoom()
        {
            if (IsConnectedToServer())
            {
                _Socket.Emit(Command.QUIT_GAME);
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

        #endregion

        #region Custom Methods

        private bool IsConnectedToServer()
        {
            return _Socket.sid != null;
        }

        #endregion
    }

}
