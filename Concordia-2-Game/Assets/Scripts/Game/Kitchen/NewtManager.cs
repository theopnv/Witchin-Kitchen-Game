using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class NewtManager : PickableObject, IPunchable
    {
        Rigidbody m_rb;
        public GameObject m_eyeIngredientPrefab;
        private int m_eyeCount = 2;
        private GameObject[] m_eyes, m_eyesockets, m_eyepatches;

        void Start()
        {
            m_rb = GetComponent<Rigidbody>();
            m_eyes = new GameObject[] { transform.Find("EyeBall2/Eye2").gameObject, transform.Find("EyeBall1/Eye1").gameObject };
            m_eyesockets = new GameObject[] { transform.Find("EyeBall2/EyeSocket2").gameObject, transform.Find("EyeBall1/EyeSocket1").gameObject };
            m_eyepatches = new GameObject[] { transform.Find("Eyepatch2").gameObject, transform.Find("Eyepatch1").gameObject };
        }

        public override void PickUp(Transform newParent)
        {
            if (m_eyeCount > 0)
            {
                var dist = transform.position - newParent.position;
                m_rb.AddForce(dist.normalized * 5, ForceMode.VelocityChange);

                //Spawn an eye ingredient
                m_eyeCount--;
                var newEye = Instantiate(m_eyeIngredientPrefab, transform.position, Quaternion.identity);
                var eyePickable = newEye.GetComponent<PickableObject>();
                var playerPickup = newParent.parent.gameObject.GetComponent<PlayerPickUpDropObject>();
                playerPickup.ForcePickUpObject(eyePickable);

                //Swap out eye for an eyepatch
                m_eyes[m_eyeCount].SetActive(false);
                StartCoroutine(AddBandage());
            }
        }

        public void Punch(Vector3 knockVelocity, float stunTime)
        {
            m_rb.AddForce(knockVelocity, ForceMode.VelocityChange);


            if (m_eyeCount > 0)
            {
                //Spawn an eye ingredient
                m_eyeCount--;
                var newEye = Instantiate(m_eyeIngredientPrefab, transform.position, Quaternion.identity);
                var eyeRB = newEye.GetComponent<Rigidbody>();
                eyeRB.AddForce(-0.5f * knockVelocity, ForceMode.VelocityChange);

                //Swap out eye for an eyepatch
                m_eyes[m_eyeCount].SetActive(false);
                StartCoroutine(AddBandage());
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
