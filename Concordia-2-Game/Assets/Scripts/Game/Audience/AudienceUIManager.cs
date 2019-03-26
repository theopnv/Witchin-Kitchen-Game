using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{
    public class AudienceUIManager : MonoBehaviour
    {
        [SerializeField] private Text _RoomPin;
        [SerializeField] private Text _ViewersNumber;

        private AudienceInteractionManager _AudienceManager;

        // Start is called before the first frame update
        void Start()
        {
            UpdateHUD();

            _AudienceManager = FindObjectOfType<AudienceInteractionManager>();
            if (_AudienceManager)
            {
                _AudienceManager.OnGameUpdated += UpdateHUD;
            }
        }

        void OnDisable()
        {
            if (_AudienceManager)
            {
                _AudienceManager.OnGameUpdated -= UpdateHUD;
            }
        }

        public void UpdateHUD()
        {
            try
            {
                _ViewersNumber.text = GameInfo.Viewers.Count.ToString();
                _RoomPin.text = GameInfo.RoomId;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

    }

}
