using UnityEngine;
using System.Collections;

namespace con2.game
{

    public class ExplosiveItem : AHitAllInRange
    {
        private bool m_explodeOnContact = false;
        [HideInInspector] public GameObject m_launcher;
        private bool m_exploded = false;

        public void ExplodeByTime(float delaySec)
        {
            StartCoroutine(Explode(delaySec));
        }

        public void ExplodeOnContact()
        {
            m_explodeOnContact = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (m_explodeOnContact)
            {
                if (collision.gameObject != m_launcher)
                {
                    StartCoroutine(Explode(0));
                }
            }
        }

        protected override void AfterHitting()
        {

        }

        protected override Vector3 ModulateHitVector(Vector3 hitVector)
        {
            return hitVector;
        }

        protected override void OnStart()
        {
            
        }

        // Update is called once per frame
        private IEnumerator Explode(float delaySec)
        {
            yield return new WaitForSeconds(delaySec);
            if (!m_exploded)
            {
                m_exploded = true;
                Hit();

                Rigidbody body = GetComponent<Rigidbody>();
                body.constraints = RigidbodyConstraints.FreezeAll;

                Collider[] colliders = transform.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                {
                    collider.enabled = false;
                }

                Renderer[] meshes = transform.GetComponentsInChildren<Renderer>();
                foreach (Renderer mesh in meshes)
                {
                    mesh.enabled = false;
                }

                var blastwave = gameObject.GetComponentInChildren<Blastwave>();
                blastwave.GetComponent<Renderer>().enabled = true;
                blastwave.Play();

                var kaboom = gameObject.GetComponentInChildren<Kaboom>();
                StartCoroutine(kaboom.Activate());
            }
        }
    }
}
