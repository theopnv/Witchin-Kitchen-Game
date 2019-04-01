using System.Collections;
using UnityEngine;

namespace con2.game
{

    public class GrowShrinkEffect : MonoBehaviour
    {
        public float _maxScale;

        public void Grow()
        {
            StartCoroutine(Scale(_maxScale, 1.0f));
        }

        public void Shrink()
        {
            StartCoroutine(Scale(1.0f, _maxScale));
        }

        private IEnumerator Scale(float startScale, float endScale)
        {
            var startTime = Time.time;
            while (Time.time - startTime < 0.5f)
            {
                float scaleComponent = Mathf.Lerp(startScale, endScale, (Time.time - startTime)/0.5f);
                transform.localScale = scaleComponent * Vector3.one;
                yield return new WaitForEndOfFrame();
            }
        }

    }

}
