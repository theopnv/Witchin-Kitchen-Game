using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using con2.game;
using static con2.game.Events;

namespace con2.game
{

    public abstract class AbstractAudienceEvent : MonoBehaviour, IEventSubscriber
    {
        protected MessageFeedManager _MessageFeedManager;

        public abstract void EventStart();
        public abstract IEnumerator EventImplementation();
        public abstract EventID GetEventID();

        public void SetUpEvent()
        {
            _MessageFeedManager = FindObjectOfType<MessageFeedManager>();
            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            var eventManager = managers.GetComponentInChildren<EventManager>();
            eventManager.SetUp();
            eventManager.AddSubscriber(GetEventID(), this);
        }

        public void ActivateEventMode()
        {
            EventStart();
            StartCoroutine(RunEvent(EventImplementation));
        }

        public IEnumerator RunEvent(Func<IEnumerator> currentEvent)
        {
            yield return StartCoroutine(currentEvent());
        }
    }
}
