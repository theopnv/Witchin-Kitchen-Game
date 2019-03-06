using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using con2.messages;
using UnityEngine;
using UnityEngine.UI;
using Event = con2.messages.Event;

namespace con2.game
{

    public class EventManager : MonoBehaviour
    {

        public bool AnEventIsHappening;
        public Text m_audienceEventText;

        #region Private Attrubutes

        private AudienceInteractionManager _AudienceInteractionManager;

        private Dictionary<Events.EventID, List<IEventSubscriber>> _EventSubscribers;

        #endregion

        #region Unity API

        private void Start()
        {
            SetUp();
        }

        #endregion

        #region Custom Methods

        public void SetUp()
        {
            m_audienceEventText.enabled = false;

            if (_EventSubscribers == null)
            {
                _EventSubscribers = new Dictionary<Events.EventID, List<IEventSubscriber>>();
                foreach (Events.EventID id in Enum.GetValues(typeof(Events.EventID)))
                {
                    _EventSubscribers.Add(id, new List<IEventSubscriber>());
                }

                _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
                if (_AudienceInteractionManager != null)
                {
                    _AudienceInteractionManager.EventSubscribers = _EventSubscribers;
                }
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
                events = new List<Event> { pollEventA, pollEventB},
                deadline = "", // not used
                duration = pollingTime,
            };

            _AudienceInteractionManager?.SendPoll(poll);
            StartCoroutine("StartEvent", pollingTime);

            m_audienceEventText.text = "Time for an audience event, spectators vote on your phone!";
            m_audienceEventText.enabled = true;
        }

        private IEnumerator StartEvent(float pollingTime)
        {
            AnEventIsHappening = true;
            yield return new WaitForSeconds(pollingTime);
            AnEventIsHappening = false;
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

        #endregion


    }

}
