using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using con2.game;
using con2.messages;
using Newtonsoft.Json;
using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Ingredient = con2.messages.Ingredient;

namespace con2
{

    /// <summary>
    /// For communication happening in the lobby
    /// </summary>
    public partial class AudienceInteractionManager : MonoBehaviour
    {
        public Action<Base> OnReceivedMessage;
        public Action<IngredientPoll> OnReceivedIngredientPollResults;

        void LobbyStart()
        {
            _Socket.On(Command.GAME_CREATED, OnGameCreated);
            _Socket.On(Command.INGREDIENT_POLL_RESULTS, OnIngredientPollResults);
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
        public bool SendPlayerCharacteristics(List<PlayerManager> playerList)
        {
            var players = playerList.Select(p => new messages.Player()
            {
                id = p.ID,
                name = p.Name,
                color = "#" + ColorUtility.ToHtmlStringRGBA(ColorsManager.Get().PlayerAppColors[p.ID]),

            }).ToList();

            var newtorkedList = new messages.Players {players = players};

            var serialized = JsonConvert.SerializeObject(newtorkedList);
            _Socket.Emit(
                Command.REGISTER_PLAYERS, 
                new JSONObject(serialized));

            return true;
        }

        public void SendStartIngredientPoll(int a, int b)
        {
            var poll = new IngredientPoll()
            {
                ingredients = new List<Ingredient>()
                {
                    new Ingredient() {id = a, votes = 0},
                    new Ingredient() {id = b, votes = 0},
                }
            };
            var serialized = JsonConvert.SerializeObject(poll);
            _Socket.Emit(
                Command.START_INGREDIENT_POLL,
                new JSONObject(serialized));
        }

        public void SendStopIngredientPoll()
        {
            _Socket.Emit(Command.STOP_INGREDIENT_POLL);
        }

        #endregion

        #region Receive

        private void OnLobbyMessage(Base content)
        {
            OnReceivedMessage?.Invoke(content);
        }

        private void OnGameCreated(SocketIOEvent e)
        {
            Debug.Log("OnGameCreated");
            var game = JsonConvert.DeserializeObject<Game>(e.data.ToString());
            GameInfo.RoomId = game.pin;
            GameInfo.Viewers = game.viewers;
            OnGameUpdated?.Invoke();
        }

        private void OnIngredientPollResults(SocketIOEvent e)
        {
            Debug.Log("OnIngredientPollResults");
            var pollResults = JsonConvert.DeserializeObject<IngredientPoll>(e.data.ToString());
            OnReceivedIngredientPollResults?.Invoke(pollResults);
        }

        #endregion
        
    }

}
