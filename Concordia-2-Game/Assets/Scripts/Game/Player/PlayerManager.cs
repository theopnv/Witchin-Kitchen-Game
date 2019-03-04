using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                GetComponent<PlayerInputController>().SetPlayerIndex(_ID);
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
            }
        }

        public int Score;

        #endregion

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
