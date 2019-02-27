using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using con2.game;
using con2.messages;
using Newtonsoft.Json;
using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Event = con2.messages.Event;

namespace con2
{

    /// <summary>
    /// For communication happening in the game
    /// </summary>
    public partial class AudienceInteractionManager : MonoBehaviour
    {
        [HideInInspector]
        public Dictionary<Events.EventID, List<IEventSubscriber>> EventSubscribers;

        [HideInInspector]
        public Dictionary<Spells.SpellID, List<ISpellSubscriber>> SpellSubscribers;

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

        #endregion

        #region Receive

        private void OnGameMessage(Base content)
        {
            if ((int)content.code % 10 == 0) // Success codes always have their unit number equal to 0 (cf. protocol)
            {
                Debug.Log(content.content);
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
            var voteA = content.events[0];
            var voteB = content.events[1];
            Debug.Log("Votes for A: " + voteA.votes);
            Debug.Log("Votes for B: " + voteB.votes);

            Event chosenEvent;
            if (voteA.votes == voteB.votes)
            {
                chosenEvent = Random.Range(0, 2) == 0 ? voteA : voteB;
            }
            else
            {
                chosenEvent = voteA.votes > voteB.votes ? voteA : voteB;
            }

            BroadcastPollResults(chosenEvent);
        }

        public void BroadcastPollResults(Event chosenEvent)
        {
            Debug.Log("Results of the poll: " +
                      Events.EventList[(Events.EventID)chosenEvent.id] +
                      " was voted");
            var key = (Events.EventID) chosenEvent.id;

            if (EventSubscribers.ContainsKey(key))
            {
                EventSubscribers[key]
                    .ForEach((subscriber) =>
                    {
                        subscriber.ActivateEventMode();
                    });
            }
            else
            {
                Debug.LogError("Event key not found");
            }
        }

        private void OnCastSpellRequest(SocketIOEvent e)
        {
            var content = JsonConvert.DeserializeObject<Spell>(e.data.ToString());
            Debug.Log("Casted spell: " + Spells.EventList[(Spells.SpellID)content.spellId]);

            var key = (Spells.SpellID) content.spellId;
            if (SpellSubscribers.ContainsKey(key))
            {
                SpellSubscribers[key].ForEach((subscriber) =>
                {
                    subscriber.ActivateSpellMode(content.targetedPlayer);
                });
            }
            else
            {
                Debug.LogError("Spell Key not found");
            }
            
        }

        #endregion

    }

}
