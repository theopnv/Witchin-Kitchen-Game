using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{

    public class PlayerHUD : MonoBehaviour
    {
        public int OwnerId = -1;
        public Text Message;
        public Text Recipe;
        public Text Score;

        void Start()
        {
            var detectController = FindObjectOfType<DetectController>();
            detectController.OnDisconnected += i =>
            {
                if (i == OwnerId)
                {
                    Players.GetPlayerByID(OwnerId).SendMessageToPlayerInHUD("Your controller was disconnected.", Color.red, true);
                }
            };
            detectController.OnConnected += i =>
            {
                if (i == OwnerId)
                {
                    Players.GetPlayerByID(OwnerId).SendMessageToPlayerInHUD("", Color.red);
                }
            };
        }
    }

}
