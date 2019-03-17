using System;
using System.Collections.Generic;
using System.Linq;
using con2.messages;
using Newtonsoft.Json;
using UnityEngine;
using SocketIO;
using Debug = UnityEngine.Debug;
using Players = con2.game.Players;

namespace con2
{

    /// <summary>
    /// For communication happening in the game
    /// </summary>
    public partial class AudienceInteractionManager : MonoBehaviour
    {
        public Action<Spell> OnSpellCasted;
        public Action<PollChoices> OnEventVoted;

        void GameStart()
        {
            _Socket.On(Command.RECEIVE_VOTES, OnReceiveEventVotes);
            _Socket.On(Command.SPELL_CAST_REQUEST, OnCastSpellRequest);
        }

        #region Emit

        public void SendPoll(PollChoices pollChoices)
        {
            Debug.Log("SendPoll");
            var serialized = JsonConvert.SerializeObject(pollChoices);
            _Socket.Emit(Command.LAUNCH_POLL, new JSONObject(serialized));
        }

        public void SendSpellCastRequest(Viewer viewer)
        {
            Debug.Log("SendSpellCastRequest");
            var serialized = JsonConvert.SerializeObject(viewer);
            _Socket.Emit(Command.LAUNCH_SPELL_CAST, new JSONObject(serialized));
        }

        public void SendGameStateUpdate()
        {
            var players = new List<Player>();
            for (var i = 0; i < Players.Dic.Count; i++)
            {
                var player = Players.GetPlayerByID(i);
                players.Add(new Player
                {
                    id = i,
                    color = "#" + ColorUtility.ToHtmlStringRGBA(player.Color),
                    name = player.Name,
                    potions = player.CompletedPotionCount,
                });
            }

            var game = new Game
            {
                players = players,
            };

            var serialized = JsonConvert.SerializeObject(game);
            _Socket?.Emit(Command.SEND_GAME_STATE, new JSONObject(serialized));
        }
        
        public void SendGameOutcome()
        {
            List<Player> leaderboards = Players.Dic
                .Select(x => x.Value)
                .OrderByDescending(x => x.CompletedPotionCount)
                .ThenByDescending(x => x.CollectedIngredientCount)
                .Select(x => new Player()
                {
                    color = ColorUtility.ToHtmlStringRGBA(x.Color),
                    id = x.ID,
                    name = x.Name,
                    ingredients = x.CollectedIngredientCount,
                    potions = x.CompletedPotionCount,
                }).ToList();

            var gameOutcome = new GameOutcome()
            {
                leaderboards = leaderboards.ToArray(),
            };

            var serialized = JsonConvert.SerializeObject(gameOutcome);
            _Socket.Emit(Command.GAME_OUTCOME, new JSONObject(serialized));
        }

        #endregion

        #region Receive

        private void OnGameMessage(Base content)
        {
            if ((int)content.code % 10 == 0) // Success codes always have their unit number equal to 0 (cf. protocol)
            {
                switch (content.code)
                {
                    default: break;
                }
            }
            else
            {
                Debug.LogError(content.content);
            }
        }

        private void OnReceiveEventVotes(SocketIOEvent e)
        {
            var content = JsonConvert.DeserializeObject<PollChoices>(e.data.ToString());
            OnEventVoted?.Invoke(content);
        }

        private void OnCastSpellRequest(SocketIOEvent e)
        {
            var content = JsonConvert.DeserializeObject<Spell>(e.data.ToString());
            OnSpellCasted?.Invoke(content);
        }

        #endregion

    }

}
