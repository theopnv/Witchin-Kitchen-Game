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
        private GameObject _PlayerInLobby;
        [SerializeField]
        private GameObject _PlayerOutOfLobby;

        [SerializeField] private GameObject _Model;

        [SerializeField] private Image ColorBox;

        #endregion

        void Update()
        {
            _Model?.transform?.Rotate(Vector3.up, -20 * Time.deltaTime);
        }

        public void SetActiveCanvas(bool inLobby)
        {
            _PlayerInLobby.SetActive(inLobby);
            _PlayerOutOfLobby.SetActive(!inLobby);
        }

    }

}
