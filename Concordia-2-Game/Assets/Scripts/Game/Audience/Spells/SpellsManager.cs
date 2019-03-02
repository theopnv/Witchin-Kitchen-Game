using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using con2.game;
using con2.messages;
using UnityEngine;

namespace con2.game
{

    public class SpellsManager : MonoBehaviour
    {


        /// <summary>
        /// If we're broadcasting an event the spells timer stops.
        /// This is to avoid asking a viewer to cast a spell while he/she's voting.
        /// </summary>
        [SerializeField] private EventManager _EventManager;

        private AudienceInteractionManager _AudienceInteractionManager;

        private int _CurrentCastSpeller = 0;

        private Dictionary<Spells.SpellID, List<ISpellSubscriber>> _SpellSubscribers;

        #region Unity API

        // Start is called before the first frame update
        void Awake()
        {
            _SpellSubscribers = new Dictionary<Spells.SpellID, List<ISpellSubscriber>>();
            foreach (Spells.SpellID id in Enum.GetValues(typeof(Spells.SpellID)))
            {
                _SpellSubscribers.Add(id, new List<ISpellSubscriber>());
            }
        }

        private void Start()
        {
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            if (_AudienceInteractionManager != null)
            {
                _AudienceInteractionManager.SpellSubscribers = _SpellSubscribers;
            }

            InvokeRepeating("FakeSpellLauncher", 40, 40);
        }

        #endregion

        #region Custom methods

        private void FakeSpellLauncher()
        {
            if (!_EventManager.AnEventIsHappening && GameInfo.Viewers.Count > 0)
            {
                var viewer = GameInfo.Viewers[_CurrentCastSpeller];
                _AudienceInteractionManager.SendSpellCastRequest(viewer);

                ++_CurrentCastSpeller;
                if (_CurrentCastSpeller >= GameInfo.Viewers.Count)
                {
                    _CurrentCastSpeller = 0;
                }
            }
        }

        public void AddSubscriber(Spells.SpellID id, ISpellSubscriber subscriber)
        {
            _SpellSubscribers[id].Add(subscriber);
        }

        #endregion
    }

}
