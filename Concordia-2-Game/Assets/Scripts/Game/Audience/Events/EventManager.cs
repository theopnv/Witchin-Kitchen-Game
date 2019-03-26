using System;
using System.Collections;
using System.Collections.Generic;
using con2.messages;
using UnityEngine;
using Event = con2.messages.Event;
using Random = UnityEngine.Random;

namespace con2.game
{

    public class EventManager : MonoBehaviour
    {

        #region Private Attrubutes

        private AudienceInteractionManager _AudienceInteractionManager;

        private Dictionary<Events.EventID, List<IEventSubscriber>> _EventSubscribers;

        private MessageFeedManager _MessageFeedManager;

        [SerializeField] private AudioSource _AudioSource;

        #endregion

        #region Unity API

        private void Start()
        {
            SetUp();
            _MessageFeedManager = FindObjectOfType<MessageFeedManager>();

            _AudioSource = GetComponent<AudioSource>();
        }

        private void OnDisable()
        {
            _AudienceInteractionManager.OnEventVoted -= OnReceiveEventVotes;
        }

        #endregion

        #region Custom Methods

        public void SetUp()
        {
            if (_EventSubscribers == null)
            {
                _EventSubscribers = new Dictionary<Events.EventID, List<IEventSubscriber>>();
                foreach (Events.EventID id in Enum.GetValues(typeof(Events.EventID)))
                {
                    _EventSubscribers.Add(id, new List<IEventSubscriber>());
                }

                _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
                _AudienceInteractionManager.OnEventVoted += OnReceiveEventVotes;
            }
        }

        /// <summary>
        /// Call this method to start an audience poll (events).
        /// At the end, a callback will take care of instantiating the event in the arena.
        /// </summary>
        /// <param name="eventA">First event</param>
        /// <param name="eventB">Second event</param>
        /// <param name="pollingTime">Polling time in seconds</param>
        public void StartPoll(Events.EventID eventA, Events.EventID eventB, int pollingTime)
        {
            var pollEventA = new messages.Event { id = (int)eventA, votes = 0 };
            var pollEventB = new messages.Event { id = (int)eventB, votes = 0 };

            var poll = new PollChoices()
            {
                events = new List<Event> { pollEventA, pollEventB },
                deadline = "", // not used
                duration = pollingTime,
            };

            _AudienceInteractionManager?.SendPoll(poll);
            StartCoroutine("StartEvent", pollingTime);

            var msg = "Spectators, vote for an event!";
            _MessageFeedManager.AddMessageToFeed(msg, MessageFeedManager.MessageType.generic);
        }

        private IEnumerator StartEvent(float pollingTime)
        {
            yield return new WaitForSeconds(pollingTime);
        }

        /// <summary>
        /// Add a subsystem that will listen to some events
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subscriber"></param>
        public void AddSubscriber(Events.EventID id, IEventSubscriber subscriber)
        {
            _EventSubscribers[id].Add(subscriber);
        }

        private void OnReceiveEventVotes(PollChoices pollChoices)
        {
            var voteA = pollChoices.events[0];
            var voteB = pollChoices.events[1];
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

            _AudioSource?.Play();
            StartCoroutine(LaunchEventAfterDelay(chosenEvent));
        }

        private IEnumerator LaunchEventAfterDelay(Event chosenEvent)
        {
            yield return new WaitForSeconds(3.0f);

            var key = (Events.EventID)chosenEvent.id;
            if (_EventSubscribers.ContainsKey(key))
            {
                _EventSubscribers[key]
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

        #endregion


    }

}
