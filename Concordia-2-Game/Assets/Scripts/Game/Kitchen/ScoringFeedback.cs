using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace con2.game
{
    public class ScoringFeedback : MonoBehaviour
    {
        //Public
        public float ScoredDuration = 3f;
        public float ProcessedDuration = 1f;

        // Private
        [SerializeField] private Image _Scored;
        private Vector3 _OriginalPosScored;
        private bool _ScoredIsActive;
        [SerializeField] private Image _ProcessedIngredient;
        private Vector3 _OriginalPosProcessedIng;
        private bool _ProcessedIngredientIsActive;

        void Start()
        {
            _OriginalPosScored = _Scored.transform.position;
            _OriginalPosProcessedIng = _ProcessedIngredient.transform.position;
        }

        public void ActivateScored()
        {
            _Scored.gameObject.SetActive(true);
            _Scored.transform.position = _OriginalPosScored;
            _ScoredIsActive = true;
            _Scored.CrossFadeAlpha(0, ScoredDuration, false);
            StartCoroutine(DeactivateScored());
        }

        private IEnumerator DeactivateScored()
        {
            yield return new WaitForSeconds(ScoredDuration);
            _Scored.gameObject.SetActive(false);
            _ScoredIsActive = false;
        }

        void Update()
        {
            if (_ScoredIsActive)
            {
                _Scored.transform.Translate((Vector3.up + Vector3.back) * Time.deltaTime);
            }

            if (_ProcessedIngredientIsActive)
            {
                _ProcessedIngredient.transform.Translate((Vector3.up + Vector3.back) * Time.deltaTime);
            }
        }

        public void ActivateProcessedIngredient()
        {
            _ProcessedIngredientIsActive = true;
            _ProcessedIngredient.transform.position = _OriginalPosProcessedIng;
            _ProcessedIngredient.gameObject.SetActive(true);
            _ProcessedIngredient.CrossFadeAlpha(0, ProcessedDuration, false);
            StartCoroutine(DeactivateProcessedIngredient());
        }

        private IEnumerator DeactivateProcessedIngredient()
        {
            yield return new WaitForSeconds(ProcessedDuration);
            _ProcessedIngredientIsActive = false;
            _ProcessedIngredient.gameObject.SetActive(false);
        }
    }
}