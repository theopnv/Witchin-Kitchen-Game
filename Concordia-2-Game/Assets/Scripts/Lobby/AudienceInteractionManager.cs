using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using con2.lobby.messages;
using Newtonsoft.Json;
using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace con2.lobby
{

    public class AudienceInteractionManager : MonoBehaviour
    {
        private SocketIOComponent _Socket;

        [SerializeField]
        private Text _RoomId;

        // Start is called before the first frame update
        void Start()
        {
            _Socket = GetComponent<SocketIOComponent>();
            
            _Socket.On(Command.GAME_CREATED, OnGameCreated);
            _Socket.On(Command.MESSAGE, OnMessage);

            // Small delay between object instantiation and first use.
            StartCoroutine("Authenticate");
        }

        #region Emit

        private IEnumerator Authenticate()
        {
            yield return new WaitForSeconds(1);
            _Socket.Emit(Command.MAKE_GAME);
        }

        /// <summary>
        /// Return true if connected to server
        /// False otherwise
        /// </summary>
        /// <returns></returns>
        public bool SendPlayerCharacteristics()
        {
            if (!IsConnectedToServer())
            {
                return false;
            }

            var playerList = new List<Player>();
            for (var i = 0; i < PlayersInfo.PlayerNumber; i++)
            {
                var player = new Player
                {
                    name = PlayersInfo.Name[i],
                    color = ColorUtility.ToHtmlStringRGBA(PlayersInfo.Color[i])
                };
                playerList.Add(player);
            }

            var players = new Players
            {
                players = playerList,
            };

            var serialized = JsonConvert.SerializeObject(players);
            _Socket.Emit(
                Command.REGISTER_PLAYERS, 
                new JSONObject(serialized));

            return true;
        }

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
            if ((int)content.code % 10 == 0) // Success codes always have their unit number equal to 0 (cf. protocol)
            {
                Debug.Log(content.content);
                switch (content.code)
                {
                    case Code.register_players_success:
                        SceneManager.LoadSceneAsync(SceneNames.Game);
                        break;
                    default: break;
                }
            }
            else
            {
                Debug.LogError(content.content);
            }
        }

        private void OnGameCreated(SocketIOEvent e)
        {
            Debug.Log("OnGameCreated");
            var game = JsonConvert.DeserializeObject<Game>(e.data.ToString());
            _RoomId.text = "Room's PIN: " + game.id;
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
