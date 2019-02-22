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

        private int _LastCastSpeller = -1; 

        #region Unity API

        // Start is called before the first frame update
        void Start()
        {
            _AudienceInteractionManager = FindObjectOfType<AudienceInteractionManager>();
            InvokeRepeating("FakeSpellLauncher", 5, 40);
        }

        #endregion

        #region Custom methods

        private void FakeSpellLauncher()
        {
            if (_EventManager.AnEventIsHappening)
            {
                ++_LastCastSpeller;
                if (_LastCastSpeller >= GameInfo.Viewers.Count)
                {
                    _LastCastSpeller = -1;
                }

                var viewer = GameInfo.Viewers[_LastCastSpeller];
                _AudienceInteractionManager.SendSpellCastRequest(viewer);
            }
        }
        
        #endregion
    }

}
