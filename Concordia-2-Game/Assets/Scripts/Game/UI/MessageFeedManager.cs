using System;
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

        private Queue<GameObject> _Messages;

        [SerializeField]
        private GameObject _GlobalMessagePrefab;


        // Public
        public float TimeBeforeFade = 15f;

        #region Unity API

        // Start is called before the first frame update
        void Start()
        {
            _Messages = new Queue<GameObject>();
        }

        #endregion

        #region Custom methods

        private void OnDestroyMessage(GlobalMessage message)
        {
            message.OnDestroy -= OnDestroyMessage;
            Destroy(_Messages.Dequeue());
        }

        /// <summary>
        /// Displays a message in the feed with a specific color depending on its type.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="msgType"></param>
        public void AddMessageToFeed(string message, MessageType msgType)
        {
            const int maxMessagesInFeed = 3;

            if (_Messages.Count >= maxMessagesInFeed)
            {
                Destroy(_Messages.Dequeue());
            }

            var msg = Instantiate(_GlobalMessagePrefab.transform, _MessageFeedPlaceholder.transform).GetComponent<GlobalMessage>();
            msg.Message = message;
            msg.MsgType = msgType;
            msg.OnDestroy += OnDestroyMessage;
            _Messages.Enqueue(msg.gameObject);
        }

        #endregion

    }

}
