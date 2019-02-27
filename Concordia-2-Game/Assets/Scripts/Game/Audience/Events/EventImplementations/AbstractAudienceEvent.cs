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
        protected Text m_eventText;
        protected EventID ID;

        public abstract IEnumerator EventImplementation();
        public abstract EventID GetEventID();

        public void SetUpEvent()
        {
            GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            EventManager eventManager = managers.GetComponentInChildren<EventManager>();
            eventManager.AddSubscriber(GetEventID(), this);

            MainGameManager mgm = managers.GetComponentInChildren<MainGameManager>();
            m_eventText = eventManager.m_audienceEventText;
        }

        public void ActivateEventMode()
        {
            StartCoroutine(RunEvent(EventImplementation));
        }

        public IEnumerator RunEvent(Func<IEnumerator> currentEvent)
        {
            yield return StartCoroutine(currentEvent());
            EventEnd();
        }

        private void EventEnd()
        {
            m_eventText.text = "";
            m_eventText.enabled = false;
        }
    }
}
