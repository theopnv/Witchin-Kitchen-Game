using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;
using Key = con2.PlayerPrefsKeys;

namespace con2.settings
{

    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private InputField _AddressInputField;
        [SerializeField] private Text _FeedbackText;

        private const string ERROR_EMPTY_ADDRESS = "Please enter the address of the hosting server.";
        private const string SUCCESSFULLY_SAVED = "Preferences successfully saved.";

        void Start()
        {
            _AddressInputField.text = PlayerPrefs.GetString(Key.HOST_ADDRESS);
        }

        public void OnSaveClick()
        {
            if (_AddressInputField.text.IsNullOrEmpty())
            {
                SetFeedbackText(true, ERROR_EMPTY_ADDRESS, Color.red);
            }
            else
            {
                PlayerPrefs.SetString(Key.HOST_ADDRESS, _AddressInputField.text);
            }

            SetFeedbackText(true, SUCCESSFULLY_SAVED, Color.green);
        }

        private IEnumerator UnsetFeedbackText()
        {
            yield return new WaitForSeconds(5);
            _FeedbackText.gameObject.SetActive(false);
            _FeedbackText.text = "";
        }

        private void SetFeedbackText(bool active, string desc, Color color)
        {
            _FeedbackText.text = desc;
            _FeedbackText.color = color;
            _FeedbackText.gameObject.SetActive(true);
            StartCoroutine("UnsetFeedbackText");
        }
    }

}
