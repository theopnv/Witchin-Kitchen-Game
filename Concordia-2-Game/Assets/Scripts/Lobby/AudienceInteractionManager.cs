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
        private const string GAME_SCENE_NAME = "Game";

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

        public void SendPlayerCharacteristics()
        {
            var players = new Players
            {
                name1 = PlayersInfo.Name[0],
                color1 = ColorUtility.ToHtmlStringRGBA(PlayersInfo.Color[0]),

                name2 = PlayersInfo.Name[1],
                color2 = ColorUtility.ToHtmlStringRGBA(PlayersInfo.Color[1]),
            };
            var serialized = JsonConvert.SerializeObject(players);
            _Socket.Emit(
                Command.REGISTER_PLAYERS, 
                new JSONObject(serialized));
        }

        #endregion

        #region Receive

        private void OnMessage(SocketIOEvent e)
        {
            var content = JsonConvert.DeserializeObject<Base>(e.data.ToString());
            if ((int)content.Code % 10 == 0) // Success codes always have their unit number equal to 0 (cf. protocol)
            {
                Debug.Log(content.Content);
                switch (content.Code)
                {
                    case Code.register_players_success:
                        SceneManager.LoadSceneAsync(GAME_SCENE_NAME);
                        break;
                    default: break;
                }
            }
            else
            {
                Debug.LogError(content.Content);
            }
        }

        private void OnGameCreated(SocketIOEvent e)
        {
            Debug.Log("OnGameCreated");
            var game = JsonConvert.DeserializeObject<Game>(e.data.ToString());
            _RoomId.text = "Room's PIN: " + game.Id;
        }

        #endregion
    }

}
