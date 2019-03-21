using System;
using System.Collections.Generic;
using System.Linq;
using con2.game;
using con2.messages;
using Newtonsoft.Json;
using UnityEngine;
using SocketIO;
using Debug = UnityEngine.Debug;

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

        public void SendSpellCastRequest(int playerId, Viewer viewer)
        {
            Debug.Log("SendSpellCastRequest");
            var player = new messages.Player() {id = playerId};
            if (playerId != -1)
            {
                var manager = FindObjectOfType<AMainManager>();
                player.name = manager.GetPlayerById(playerId).Name;
            }

            var spellRequest = new SpellRequest()
            {
                fromPlayer = player,
                targetedViewer = viewer,
            };
            var serialized = JsonConvert.SerializeObject(spellRequest);
            _Socket.Emit(Command.LAUNCH_SPELL_CAST, new JSONObject(serialized));
        }

        public void SendGameStateUpdate()
        {
            var players = new List<messages.Player>();
            var manager = FindObjectOfType<AMainManager>();
            for (var i = 0; i < manager.PlayersInstances.Count; i++)
            {
                var player = manager.GetPlayerById(i);
                players.Add(new messages.Player
                {
                    id = i,
                    color = "#" + ColorUtility.ToHtmlStringRGBA(ColorsManager.Get().PlayerAppColors[i]),
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
            var manager = FindObjectOfType<AMainManager>();
            List<messages.Player> leaderboards = manager.PlayersInstances
                .Select(x => x.Value)
                .OrderByDescending(x => x.CompletedPotionCount)
                .ThenByDescending(x => x.CollectedIngredientCount)
                .Select(x => new messages.Player()
                {
                    color = ColorUtility.ToHtmlStringRGBA(ColorsManager.Get().PlayerAppColors[x.ID]),
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
