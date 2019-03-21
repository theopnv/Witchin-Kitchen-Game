using UnityEngine;
using System.Collections;

namespace con2.game
{

    public class Gift : PickableObject
    {
        private GameObject m_contentsPrefab;
        private Color m_color;
        private Ingredient m_type;
        private bool m_contentsSpawned;

        private void Start()
        {
            var r = transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
            r.material.color = m_color;
        }

        public override bool PickUp(Transform newParent)
        {
            if (!m_contentsSpawned)
            {
                m_contentsSpawned = true;

                var gift = Instantiate(m_contentsPrefab, transform.position, new Quaternion(0, 0, 0, 0));
                var giftPickable = gift.GetComponent<PickableObject>();
                var playerPickup = newParent.parent.gameObject.GetComponent<PlayerPickUpDropObject>();
                playerPickup.ForcePickUpObject(giftPickable);

                if (m_type == Ingredient.NOT_AN_INGREDIENT)
                {
                    gift.GetComponent<ExplosiveItem>().ExplodeByTime(1.75f);
                }
                else
                {
                    var spawner = FindObjectOfType<ItemSpawner>();
                    ++spawner.SpawnedItemsCount[m_type];
                }

                StartCoroutine(DespawnThis());

                return true;
            }
            return false;
        }

        public void SetIngredientType(Ingredient type)
        {
            m_type = type;
        }

        public void SetContents(GameObject contents)
        {
            m_contentsPrefab = contents;
        }

        private IEnumerator DespawnThis()
        {
            var colliders = GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.isTrigger = true;
            }
            var rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;

            var visuals = transform.GetChild(0);
            var renderers = visuals.GetComponentsInChildren<Renderer>();
            while (renderers[0].material.color.a > 0.02f)
            {
                foreach (Renderer r in renderers)
                {
                    var color = r.material.color;
                    r.material.color = new Color(color.r, color.g, color.b, color.a - Time.deltaTime);
                }
                yield return new WaitForEndOfFrame();
            }
            Destroy(gameObject);
        }

        public void SetColor(Color c)
        {
            m_color = c;
        }
    }
}
