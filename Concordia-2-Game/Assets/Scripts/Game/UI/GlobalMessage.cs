using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{

    public class GlobalMessage : MonoBehaviour
    {
        [SerializeField] private Text _MessageText;

        private string _Message;
        public string Message
        {
            get => _Message;
            set
            {
                _Message = value;
                _MessageText.text = _Message;
            }
        }

        private Color _Color;
        public Color Color
        {
            get => _Color;
            set
            {
                _Color = value;
                _MessageText.color = _Color;
            }
        }

        private MessageFeedManager.MessageType _MsgType;

        public MessageFeedManager.MessageType MsgType
        {
            get => _MsgType;
            set
            {
                _MsgType = value;
                Color = MessageTypeLookupTable[_MsgType];
            }
        }

        private float _TimeSinceInstantiation = 0;
        private bool _IsFadingOut = false;

        public Action<GlobalMessage> OnDestroy;

        private readonly Dictionary<MessageFeedManager.MessageType, Color> MessageTypeLookupTable = new Dictionary<MessageFeedManager.MessageType, Color>
        {
            { MessageFeedManager.MessageType.error, Color.red },
            { MessageFeedManager.MessageType.arena_event, Color.cyan },
            { MessageFeedManager.MessageType.generic, Color.white },
        };

        void Update()
        {
            _TimeSinceInstantiation += Time.deltaTime;
            if (!_IsFadingOut && _TimeSinceInstantiation >= 15)
            {
                _IsFadingOut = true;
                StartCoroutine(FadeOutAfterSeconds());
            }
        }

        IEnumerator FadeOutAfterSeconds()
        {
            const float fadeOutDuration = 1f;
            for (var t = 0.01f; t < fadeOutDuration; t += Time.deltaTime)
            {
                Color = Color.Lerp(Color, Color.clear, Mathf.Min(.25f, t / fadeOutDuration));

                yield return null;
            }

            OnDestroy?.Invoke(this);
        }
    }
}