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
            detectController.OnConnected += i => {
                if (i == OwnerId)
                {
                    DisableMessage();
                }
            };
            detectController.OnDisconnected += i =>
            {
                if (i == OwnerId)
                {
                    EnableMessage("Your controller was disconnected.");
                }
            };
        }

        void EnableMessage(string message)
        {
            Message.text = message;
            Message.gameObject.SetActive(true);
        }

        void DisableMessage()
        {
            Message.gameObject.SetActive(false);
        }

    }

}
