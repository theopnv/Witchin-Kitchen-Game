using System.Collections;
using System.Collections.Generic;
using System.Linq;
using con2.game;
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

        #region Unity API

        // Start is called before the first frame update
        void Start()
        {
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            InvokeRepeating("FakeSpellLauncher", 40, 40);
        }

        #endregion

        #region Custom methods

        private void FakeSpellLauncher()
        {
            if (!_EventManager.AnEventIsHappening)
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
        
        #endregion
    }

}
