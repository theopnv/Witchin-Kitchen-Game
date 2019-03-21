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

        public Texture Texture {
            set {
                var skinRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
                skinRenderer.material.mainTexture = value;
                Players.Dic[ID] = this;
            }
        }

        private int _CompletedPotionCount;

        public int CompletedPotionCount
        {
            get => _CompletedPotionCount;
            set
            {
                _CompletedPotionCount = value;
                Players.Dic[ID] = this;
                _PlayerHUD.Score.text = _CompletedPotionCount.ToString();
            }
        }

        private int _CollectedIngredientCount;

        public int CollectedIngredientCount
        {
            get => _CollectedIngredientCount;
            set
            {
                _CollectedIngredientCount = value;
                Players.Dic[ID] = this;
            }
        }

        private Rank _Rank;

        public Rank PlayerRank
        {
            get => _Rank;
            set
            {
                _Rank = value;
                Players.Dic[ID] = this;
            }
        }

        public enum Rank
        {
            FIRST,
            MIDDLE,
            LAST
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
            yield return new WaitForSeconds(5f);
            _PlayerHUD.Message.text = "";
        }

        #endregion

    }

}
