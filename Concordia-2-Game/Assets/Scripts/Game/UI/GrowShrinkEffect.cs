using System.Collections;
using UnityEngine;

namespace con2.game
{

    public class GrowShrinkEffect : MonoBehaviour
    {
        public float _maxScale;

        public void Grow()
        {
            Debug.Log("gRRRROWWWW");
            StartCoroutine(Scale(1.0f, _maxScale));
        }

        public void Shrink()
        {
            Debug.Log("shhhRIIIIIIINNNKKK");
            StartCoroutine(Scale(_maxScale, 1.0f));
        }

        private IEnumerator Scale(float startScale, float endScale)
        {
            var startTime = Time.time;
            while (Time.time - startTime <= 0.5f)
            {
                float scaleComponent = Mathf.Lerp(startScale, endScale, (Time.time - startTime)/0.5f);
                Debug.Log(scaleComponent);
                transform.localScale = scaleComponent * Vector3.one;
                yield return new WaitForEndOfFrame();
            }
        }

    }

}
