using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using con2.messages;
using UnityEngine;
using Event = con2.messages.Event;

namespace con2.game
{

    public class EventManager : MonoBehaviour
    {

        #region Private Attrubutes

        private AudienceInteractionManager _AudienceInteractionManager;

        private Dictionary<Events.EventID, List<IEventSubscriber>> _EventSubscribers;

        #endregion

        #region Unity API

        private void Start()
        {
            _EventSubscribers = new Dictionary<Events.EventID, List<IEventSubscriber>>();
            foreach (Events.EventID id in Enum.GetValues(typeof(Events.EventID)))
            {
                _EventSubscribers.Add(id, new List<IEventSubscriber>());
            }

            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            _AudienceInteractionManager.EventSubscribers = _EventSubscribers;

            //Set up audience events
            AbstractAudienceEvent[] eventFunctions = GetComponents<AbstractAudienceEvent>();
            foreach (AbstractAudienceEvent e in eventFunctions)
            {
                e.SetUpEvent();
            }
        }

        #endregion

        #region Custom Methods
        
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
            // yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss
            // 2019-02-18T17:00:48Z
            var deadline = new DateTime(
                year:DateTime.Now.Year, 
                month:DateTime.Now.Month, 
                day:DateTime.Now.Day, 
                hour:DateTime.Now.Hour, 
                minute:DateTime.Now.Minute, 
                second:DateTime.Now.Second).ToUniversalTime();
            var deadlineAsStr = deadline
                .Add(new TimeSpan(0, 0, 0, pollingTime))
                .ToString("u");
            Debug.Log(deadline);
            var poll = new PollChoices()
            {
                events = new List<Event> { pollEventA, pollEventB},
                deadline = deadlineAsStr,
            };

            _AudienceInteractionManager.SendPoll(poll);
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
