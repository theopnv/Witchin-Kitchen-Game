using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using con2.messages;
using Newtonsoft.Json;
using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace con2
{

    /// <summary>
    /// For communication happening in the lobby
    /// </summary>
    public partial class AudienceInteractionManager : MonoBehaviour
    {
        void LobbyStart()
        {
            _Socket.On(Command.GAME_CREATED, OnGameCreated);
        }

        public void SetURL(string url)
        {
            _Socket?.ResetUrl(url);
        }

        public void Connect()
        {
            _Socket?.Connect();
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
        public bool SendPlayerCharacteristics(List<Player> playerList)
        {
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
        
        #endregion

        #region Receive

        private void OnLobbyMessage(Base content)
        {
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
            GameInfo.RoomId = game.pin;
            GameInfo.Viewers = game.viewers;
            OnGameUpdated?.Invoke();
        }

        #endregion
        
    }

}
