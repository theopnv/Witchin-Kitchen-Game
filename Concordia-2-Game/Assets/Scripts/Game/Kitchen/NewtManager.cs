using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class NewtManager : PickableObject, IPunchable
    {
        public GameObject m_eyeIngredientPrefab;
        private int m_eyeCount = 2;
        public GameObject[] m_eyes, m_eyesockets, m_eyepatches;

        public Vector3 restLocation, exitLocation;
        public float m_movementSpeed;
        private Vector3 currentGoal;
        private AnimControlNewt anim;

        void Start()
        {
            Initialize();
            audioSource = GetComponent<AudioSource>();
            anim = GetComponent<AnimControlNewt>();

            currentGoal = restLocation;
        }

        private void Initialize()
        {
            m_rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            var dist = currentGoal - transform.position;
            if (Mathf.Abs(dist.magnitude) > 2.0f)
            {
                anim.SetWalking(true);
            }
            else if (currentGoal.Equals(exitLocation))
            {
                Destroy(this.gameObject);
            }
            else
            {
                anim.SetWalking(false);
            }
        }

        private void FixedUpdate()
        {
            var dist = currentGoal - transform.position;
            if (Mathf.Abs(dist.magnitude) > 2.0f)
            {
                m_rb.velocity = dist.normalized * m_movementSpeed;

                Quaternion facing = new Quaternion();
                facing.SetLookRotation(new Vector3(dist.x, 0, dist.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, facing, 2 * Time.deltaTime);
            }
        }

        public override bool PickUp(Transform newParent)
        {
            if (m_eyeCount > 0)
            {
                if (!m_rb)
                {
                    Initialize();
                }
                var dist = transform.position - newParent.position;
                m_rb.AddForce(dist.normalized * 5, ForceMode.Impulse);

                //Spawn an eye ingredient
                m_eyeCount--;
                var pos = transform.position;
                pos.x -= 1f; // Shift instantiation on the side a bit to avoid colliders overlapping and making the newt jump 20 feet high
                var newEye = Instantiate(m_eyeIngredientPrefab, pos, Quaternion.identity);
                var eyePickable = newEye.GetComponent<PickableObject>();
                var playerPickup = newParent.gameObject.GetComponentInParent<PlayerPickUpDropObject>();
                playerPickup.ForcePickUpObject(eyePickable);

                //Swap out eye for an eyepatch
                m_eyes[m_eyeCount].SetActive(false);
                StartCoroutine(AddBandage());
                if (m_eyeCount <= 0)
                {
                    currentGoal = exitLocation;
                }

                return true;
            }
            return false;
        }

        public new void Punch(Vector3 knockVelocity, float stunTime)
        {
            audioSource.Play();

            m_rb.AddForce(0.4f * knockVelocity, ForceMode.VelocityChange);  //The newt shouldn't fly too far when hit

            if (m_eyeCount > 0)
            {
                //Spawn an eye ingredient
                m_eyeCount--;
                var pos = transform.position;
                pos.x -= 1f; // Shift instantiation on the side a bit to avoid colliders overlapping and making the newt jump 20 feet high
                var newEye = Instantiate(m_eyeIngredientPrefab, pos, Quaternion.identity);
                var eyeRB = newEye.GetComponent<Rigidbody>();
                eyeRB.AddForce(-0.5f * knockVelocity, ForceMode.VelocityChange);

                //Swap out eye for an eyepatch
                m_eyes[m_eyeCount].SetActive(false);
                StartCoroutine(AddBandage());
                if (m_eyeCount <= 0)
                {
                    currentGoal = exitLocation;
                }
            }
        }

        private IEnumerator AddBandage()
        {
            yield return new WaitForSeconds(0.5f);
            m_eyesockets[m_eyeCount].SetActive(false);
            m_eyepatches[m_eyeCount].SetActive(true);
        }
    }
}
