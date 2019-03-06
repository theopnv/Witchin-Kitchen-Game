using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using con2.messages;

namespace con2.game
{

    public abstract class ASpell : MonoBehaviour, ISpellSubscriber
    {
        protected Spells.SpellID ID;
        protected messages.Player _TargetedPlayer;

        public abstract IEnumerator SpellImplementation();
        public abstract Spells.SpellID GetSpellID();

        public void SetUpSpell()
        {
            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            var eventManager = managers.GetComponentInChildren<SpellsManager>();
            eventManager.AddSubscriber(GetSpellID(), this);

            var mgm = managers.GetComponentInChildren<MainGameManager>();
        }

        public void ActivateSpellMode(messages.Player targetedPlayer)
        {
            _TargetedPlayer = targetedPlayer;
            StartCoroutine(RunEvent(SpellImplementation));
        }

        public IEnumerator RunEvent(Func<IEnumerator> currentEvent)
        {
            yield return StartCoroutine(currentEvent());
            EventEnd();
        }

        private void EventEnd()
        {
            //m_eventText.text = "";
            //m_eventText.enabled = false;
        }
    }
}
