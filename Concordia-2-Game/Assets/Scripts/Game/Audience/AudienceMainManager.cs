using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{
    public class AudienceMainManager : MonoBehaviour
    {
        [SerializeField] private Text _RoomPin;
        [SerializeField] private Text _ViewersNumber;

        // Start is called before the first frame update
        void Start()
        {
            UpdateHUD();

            var audienceManager = FindObjectOfType<AudienceInteractionManager>();
            if (audienceManager)
            {
                audienceManager.OnGameUpdated += UpdateHUD;
            }
        }

        void UpdateHUD()
        {
            _RoomPin.text = GameInfo.RoomId.ToString();
            _ViewersNumber.text = GameInfo.Viewers.Count.ToString();
        }

    }

}
