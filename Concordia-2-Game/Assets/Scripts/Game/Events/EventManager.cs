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

        #endregion

        #region Unity API

        private void Start()
        {
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            StartCoroutine(FakePollStarter());
        }

        #endregion

        #region Custom Methods

        private IEnumerator FakePollStarter()
        {
            yield return new WaitForSeconds(2);
            StartPoll(Events.EventID.freezing_rain, Events.EventID.freezing_rain, 30);
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
            // yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss
            // 2019-02-18T17:00:48Z
            var deadline = new DateTime(
                year:DateTime.Now.Year, 
                month:DateTime.Now.Month, 
                day:DateTime.Now.Day, 
                hour:DateTime.Now.Hour, 
                minute:DateTime.Now.Minute, 
                second:pollingTime,
                DateTimeKind.Utc).ToString("u");
            Debug.Log(deadline);
            var poll = new PollChoices()
            {
                events = new List<Event> { pollEventA, pollEventB},
                deadline = deadline,
            };

            _AudienceInteractionManager.SendPoll(poll);
        }

        #endregion


    }

}
