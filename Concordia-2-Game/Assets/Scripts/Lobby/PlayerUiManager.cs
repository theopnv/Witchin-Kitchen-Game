using UnityEngine;
using UnityEngine.UI;

namespace con2.lobby
{

    public class PlayerUiManager : MonoBehaviour
    {
        /// <summary>
        /// Player Numero
        /// </summary>
        public Text Label;

        /// <summary>
        /// Color of the player
        /// </summary>
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                // Automatically update colors for the whole Player UI
                ColorBox.color = value;
            }
        }

        private Color _color;

        #region Private Variables

        /// <summary>
        /// Shortcuts to turn on/off canvases
        /// </summary>
        [SerializeField]
        private GameObject PlayerInLobby;
        [SerializeField]
        private GameObject PlayerOutOfLobby;

        [SerializeField] private Image ColorBox;

        #endregion

        public void SetActiveCanvas(bool inLobby)
        {
            PlayerInLobby.SetActive(inLobby);
            PlayerOutOfLobby.SetActive(!inLobby);
        }

    }

}
