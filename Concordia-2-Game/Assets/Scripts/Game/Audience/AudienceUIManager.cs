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
            _AudienceManager.OnGameUpdated -= UpdateHUD;
        }

        public void UpdateHUD()
        {
            if (_ViewersNumber == null)
            {
                _ViewersNumber = GameObject.FindWithTag("ViewersNumber").GetComponent<Text>();
                Debug.Log("FUCK ");
            }

            Debug.Log("From UpdateHUD: " + GameInfo.Viewers.Count);
            _ViewersNumber.text = GameInfo.Viewers.Count.ToString();

            _RoomPin.text = GameInfo.RoomId;
        }

    }

}
