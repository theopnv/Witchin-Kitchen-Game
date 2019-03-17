using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{

    public class MessageFeedManager : MonoBehaviour
    {
        public enum MessageType
        {
            error,
            arena_event,
            generic,
        }

        // Private
        [SerializeField]
        private GameObject _MessageFeedPlaceholder;

        private List<Text> _MessageTexts;

        [SerializeField]
        private GameObject _GlobalMessagePrefab;

        private float _LastMessageIntantiationTime;

        private Dictionary<MessageType, Color> MessageTypeLookupTable = new Dictionary<MessageType, Color>
        {
            { MessageType.error, Color.red },
            { MessageType.arena_event, Color.cyan },
            { MessageType.generic, Color.white },
        };

        // Public
        public float TimeBeforeFade = 15f;

        #region Unity API

        // Start is called before the first frame update
        void Start()
        {
            _MessageTexts = new List<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            _LastMessageIntantiationTime += Time.deltaTime;
            if (_MessageTexts.Count > 0 && _LastMessageIntantiationTime >= TimeBeforeFade)
            {
                _LastMessageIntantiationTime = 0f; // Even if no text spawned, this prevents re-entering the condition at the next frame and therefore accelerating the fading effect.
                StartCoroutine(FadeOut());
            }
        }

        #endregion

        #region Custom methods

        private IEnumerator FadeOut()
        {
            var fadeOutDuration = 5f;
            for (var t = 0.01f; t < fadeOutDuration; t += Time.deltaTime)
            {
                foreach (var msg in _MessageTexts)
                {
                    var originalColor = msg.color;
                    msg.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(.25f, t / fadeOutDuration));
                }

                yield return null;
            }
            DestroyAllMessages();
        }

        private void DestroyFirstMessage()
        {
            if (_MessageTexts.Count == 0)
            {
                return;
            }

            var msg = _MessageTexts[0];
            Destroy(msg.gameObject);
            _MessageTexts.Remove(msg);
        }

        private void DestroyAllMessages()
        {
            foreach (var text in _MessageTexts)
            {
                Destroy(text);
            }
            _MessageTexts.Clear();
        }

        /// <summary>
        /// Displays a message in the feed with a specific color depending on its type.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="msgType"></param>
        public void AddMessageToFeed(string message, MessageType msgType)
        {
            _LastMessageIntantiationTime = 0f;
            const int maxMessagesInFeed = 3;

            if (_MessageTexts.Count >= maxMessagesInFeed)
            {
                DestroyFirstMessage();
            }

            var instance = Instantiate(_GlobalMessagePrefab, _MessageFeedPlaceholder.transform);
            var text = instance.GetComponent<Text>();
            text.color = MessageTypeLookupTable[msgType];
            text.text = message;
            _MessageTexts.Add(text);
        }

        #endregion

    }

}
