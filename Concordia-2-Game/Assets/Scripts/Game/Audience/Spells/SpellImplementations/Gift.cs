using UnityEngine;
using System.Collections;

namespace con2.game
{

    public class Gift : PickableObject
    {
        private bool m_isBomb;
        private GameObject m_contentsPrefab;

        public override void PickUp(Transform newParent)
        {
            var gift = Instantiate(m_contentsPrefab, transform.position, new Quaternion(0, 0, 0, 0));
            var giftPickable = gift.GetComponent<PickableObject>();
            var playerPickup = newParent.parent.gameObject.GetComponent<PlayerPickUpDropObject>();
            playerPickup.ForcePickUpObject(giftPickable);

            if (m_isBomb)
            {
                gift.GetComponent<ExplosiveItem>().ExplodeByTime(1.75f);
            }

            StartCoroutine(DespawnThis());
        }

        public void SetIsBomb(bool isBomb)
        {
            m_isBomb = isBomb;
        }

        public void SetContents(GameObject contents)
        {
            m_contentsPrefab = contents;
        }

        private IEnumerator DespawnThis()
        {
            var renderers = GetComponentsInChildren<Renderer>();
            while (renderers[0].material.color.a > 0.02f)
            {
                foreach (Renderer r in renderers)
                {
                    var color = r.material.color;
                    r.material.color = new Color(color.r, color.g, color.b, color.a - Time.deltaTime/2.0f);
                }
                yield return new WaitForEndOfFrame();
            }
            GameObject.Destroy(gameObject);
        }
    }
}
