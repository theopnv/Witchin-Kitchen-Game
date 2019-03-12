using UnityEngine;
using System.Collections;

namespace con2.game
{

    public class ExplosiveItem : AHitAllInRange
    {
        protected override void AfterHitting()
        {
            GameObject.Destroy(gameObject);
        }

        protected override Vector3 ModulateHitVector(Vector3 hitVector)
        {
            return hitVector;
        }

        // Use this for initialization
        protected override void OnStart()
        {
            StartCoroutine(Explode());
        }

        // Update is called once per frame
        private IEnumerator Explode()
        {
            yield return new WaitForSeconds(1.5f);
            Hit();
        }
    }
}
