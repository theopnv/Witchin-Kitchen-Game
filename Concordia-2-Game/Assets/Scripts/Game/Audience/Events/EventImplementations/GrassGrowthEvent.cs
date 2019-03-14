using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace con2.game
{
    public class GrassGrowthEvent : AbstractAudienceEvent
    {
        public float m_grassGrowthEventLength = 20.0f;
        public Grass m_gazon;

        public AnimationCurve ScaleAnim;

        [Range(0.0f, 100.0f)]
        public float TargetScale;

        public AnimationCurve FlexibilityAnim;

        [Range(0.0f, 1.0f)]
        public float TargetFlexibility;

        protected bool Playing;
        protected float StartTime;
        protected float StartScale;
        protected float StartFlexibility;

        void Start()
        {
            SetUpEvent();
        }

        public override void EventStart()
        { }

        public override IEnumerator EventImplementation()
        {

            _MessageFeedManager.AddMessageToFeed("The grass is growing by magic!", MessageFeedManager.MessageType.arena_event);

            StartTime = Time.time;
            StartScale = m_gazon.Scale.y;
            StartFlexibility = m_gazon.Flexibility;
            Playing = true;

            yield return new WaitForSeconds(m_grassGrowthEventLength);
        }

        public override Events.EventID GetEventID()
        {
            return Events.EventID.grass_growth;
        }

        public void Update()
        {
            if (Playing)
            {
                var curTime = Time.time;
                var elapsed = curTime - StartTime;

                if (elapsed >= m_grassGrowthEventLength)
                    Playing = false;

                var playback = Mathf.Clamp01(elapsed / m_grassGrowthEventLength);

                var scaleAnim = ScaleAnim.Evaluate(playback);
                m_gazon.Scale.y = StartScale + (TargetScale - StartScale) * scaleAnim;

                var flexibilityAnim = FlexibilityAnim.Evaluate(playback);
                m_gazon.Flexibility = StartFlexibility + (TargetFlexibility - StartFlexibility) * flexibilityAnim;
            }
        }
    }
}
