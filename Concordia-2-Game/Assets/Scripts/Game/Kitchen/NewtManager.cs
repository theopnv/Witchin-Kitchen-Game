using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class NewtManager : PickableObject, IPunchable
    {
        public GameObject m_eyeIngredientPrefab;
        private int m_eyeCount = 2;
        private GameObject[] m_eyes, m_eyesockets, m_eyepatches;
        private ItemSpawner m_itemSpawner;

        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            m_rb = GetComponent<Rigidbody>();
            m_itemSpawner = FindObjectOfType<ItemSpawner>();
            m_eyes = new[] { transform.Find("EyeBall2/Eye2").gameObject, transform.Find("EyeBall1/Eye1").gameObject };
            m_eyesockets = new[] { transform.Find("EyeBall2/EyeSocket2").gameObject, transform.Find("EyeBall1/EyeSocket1").gameObject };
            m_eyepatches = new[] { transform.Find("Eyepatch2").gameObject, transform.Find("Eyepatch1").gameObject };
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
                var playerPickup = newParent.parent.gameObject.GetComponent<PlayerPickUpDropObject>();
                playerPickup.ForcePickUpObject(eyePickable);

                //Swap out eye for an eyepatch
                m_eyes[m_eyeCount].SetActive(false);
                StartCoroutine(AddBandage());
                if (m_eyeCount <= 0)
                {
                    StartCoroutine(Despawn());
                }

                return true;
            }
            return false;
        }

        public new void Punch(Vector3 knockVelocity, float stunTime)
        {
            m_rb.AddForce(knockVelocity, ForceMode.VelocityChange);

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
                    StartCoroutine(Despawn());
                }
            }
        }

        private IEnumerator AddBandage()
        {
            yield return new WaitForSeconds(0.5f);
            m_eyesockets[m_eyeCount].SetActive(false);
            m_eyepatches[m_eyeCount].SetActive(true);
        }

        private IEnumerator Despawn()
        {
            yield return new WaitForSeconds(1.5f);
            --m_itemSpawner.SpawnedItemsCount[Ingredient.NEWT_EYE];
            m_itemSpawner.SpawnableItems[Ingredient.NEWT_EYE]?.AskToInstantiate();
            Destroy(gameObject);
        }
    }
}
