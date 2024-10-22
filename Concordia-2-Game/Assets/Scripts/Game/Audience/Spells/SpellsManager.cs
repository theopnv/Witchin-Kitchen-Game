﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using con2.game;
using con2.messages;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        private AMainManager _MainManager;

        private int _CurrentCastSpeller = 0;

        private Dictionary<Spells.SpellID, List<ISpellSubscriber>> _SpellSubscribers;

        private float _LastSpellCasted = 0f;
        private float _SpellFrequency = 40f;

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
            _MainManager = FindObjectOfType<AMainManager>();
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            _AudienceInteractionManager.OnSpellCasted += OnSpellCasted;
        }

        void OnDisable()
        {
            _AudienceInteractionManager.OnSpellCasted -= OnSpellCasted;
        }

        void Update()
        {
            _LastSpellCasted += Time.deltaTime;
            if (_LastSpellCasted >= _SpellFrequency && SceneManager.GetActiveScene().name == SceneNames.Game)
            {
                LaunchSpellRequest(-1); // Send -1 as playerId if no player completed a potion
            }
        }

        #endregion

        #region Custom methods

        public void LaunchSpellRequest(int playerId)
        {
            if (GameInfo.Viewers?.Count > 0)
            {
                var viewer = GameInfo.Viewers[_CurrentCastSpeller];
                _LastSpellCasted = 0f;
                _AudienceInteractionManager.SendSpellCastRequest(playerId, viewer);

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

        public void OnSpellCasted(Spell spell)
        {
            StartCoroutine(LaunchSpellAfterDelay(spell));
            //Add audio "Audience Spell!"
        }

        private IEnumerator LaunchSpellAfterDelay(Spell spell)
        {
            yield return new WaitForSeconds(1.0f);

            if (spell.caster == null)
            {
                spell.caster = new Viewer();
                spell.caster.name = "The Audience";
            }

            Debug.Log(spell.caster.name + " casted a spell: " + Spells.EventList[(Spells.SpellID)spell.spellId]);
            LogSpellInPlayerHUD(spell);
            var key = (Spells.SpellID)spell.spellId;
            if (_SpellSubscribers.ContainsKey(key))
            {
                _SpellSubscribers[key].ForEach((subscriber) =>
                {
                    subscriber.ActivateSpellMode(spell.targetedPlayer);
                });
            }
            else
            {
                Debug.LogError("Spell Key not found");
            }
        }

        private void LogSpellInPlayerHUD(Spell spell)
        {
            var message = spell.caster.name + " casted " + Spells.EventList[(Spells.SpellID) spell.spellId] + " on you";
            var player = _MainManager.GetPlayerById(spell.targetedPlayer.id);
            player.SendMessageToPlayerInHUD(message, Color.white);
        }

        #endregion
    }

}
