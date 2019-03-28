using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public class MiniTransition : MonoBehaviour
    {
        static protected MiniTransition Instance;

        static public MiniTransition Get()
        {
            return Instance;
        }


        public bool HideAtStart = true;
        public bool PlayOutAtStart = true;

        public float Duration = 0.3f;

        [ColorUsageAttribute(false)]
        public Color FillColor;

        public AnimationCurve AlphaAnimIn;
        public AnimationCurve AlphaAnimOut;

        protected UnityEngine.UI.Image Target;
        protected TMPro.TextMeshProUGUI Target2;

        protected bool Playing = false;
        protected float StartTime;
        protected bool PlayingIn;

        void Awake()
        {
            Instance = this;

            Target = GetComponentInChildren<UnityEngine.UI.Image>();
            Target2 = GetComponentInChildren<TMPro.TextMeshProUGUI>();

            if (HideAtStart)
            {
                var col = Target.color;
                col.a = 0.0f;
                Target.color = col;

                Target2.alpha = 0.0f;
            }

            if (PlayOutAtStart)
            {
                StartCoroutine(PlayOut());
            }
        }

        public IEnumerator SequenceOut(IEnumerator pre = null, IEnumerator post = null)
        {
            var sequence = new List<IEnumerator>();

            if (pre != null)
                sequence.Add(pre);

            sequence.Add(PlayOut());

            if (post != null)
                sequence.Add(post);

            return sequence.GetEnumerator();
        }

        public IEnumerator PlayIn()
        {
            Playing = true;
            StartTime = Time.time;
            PlayingIn = true;

            yield return new WaitForSeconds(Duration);
        }

        public IEnumerator PlayOut()
        {
            Playing = true;
            StartTime = Time.time;
            PlayingIn = false;

            yield return new WaitForSeconds(Duration);
        }

        // Update is called once per frame
        void Update()
        {
            if (Playing)
            {
                var curve = AlphaAnimOut;

                if (PlayingIn)
                    curve = AlphaAnimIn;

                var elapsed = Time.time - StartTime;
                var progess = Mathf.Clamp01(elapsed / Duration);
                if (progess >= 1.0f)
                    Playing = false;

                var alpha = curve.Evaluate(progess);

                var col = Target.color;
                col.a = alpha;
                col.r = FillColor.r;
                col.g = FillColor.g;
                col.b = FillColor.b;
                Target.color = col;
                Target2.alpha = alpha;
            }
        }
    }
}
