using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

namespace con2.game
{

    public class PlayerManager : MonoBehaviour
    {
        #region Attributes

        private int _ID;
        public int ID
        {
            get => _ID;
            set
            {
                _ID = value;
                GetComponent<PlayerInputController>()?.SetPlayerIndex(_ID);
                if (Players.Dic.ContainsKey(ID))
                {
                    Players.Dic[ID] = this;
                }
                else
                {
                    Players.Dic.Add(ID, this);
                }
            }
        }

        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                gameObject.name = _Name;
                Players.Dic[ID] = this;
            }
        }

        private Color _Color;
        public Color Color
        {
            get => _Color;
            set
            {
                _Color = value;
                GetComponent<Renderer>().material.color = value;
                Players.Dic[ID] = this;
            }
        }

        private int _Score;

        public int Score
        {
            get => _Score;
            set
            {
                _Score = value;
                Players.Dic[ID] = this;
            }
        }

        private PlayerHUD _PlayerHUD;

        public PlayerHUD PlayerHUD
        {
            get => _PlayerHUD;
            set
            {
                _PlayerHUD = value;
                Players.Dic[ID] = this;
            }
        }

        public void SendMessageToPlayerInHUD(string message, Color color, bool stick = false)
        {
            _PlayerHUD.Message.color = color;
            _PlayerHUD.Message.text = message;
            if (!stick)
            {
                StartCoroutine(RemoveMessage());
            }
        }

        private IEnumerator RemoveMessage()
        {
            yield return new WaitForSeconds(3f);
            _PlayerHUD.Message.text = "";
        }

        #endregion

    }

}
