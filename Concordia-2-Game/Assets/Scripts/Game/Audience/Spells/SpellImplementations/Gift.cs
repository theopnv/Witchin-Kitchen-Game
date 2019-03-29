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

        public AnimationCurve DisappearScaleAnim;
        public float DisappearDuration;
        protected Vector3 InitialScale;
        protected float DisappearStartTime;
        protected bool DisappearPlaying = false;

        private void Start()
        {
            var r = transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
            r.material.color = m_color;

            InitialScale = transform.localScale;
        }

        public void SetFollowTarget(Transform toFollow)
        {
            var followController = GetComponent<FollowGift>();
            if (followController)
            {
                followController.SetFollowTarget(toFollow);
            }
        }

        public override bool PickUp(Transform newParent)
        {
            if (!m_contentsSpawned)
            {
                m_contentsSpawned = true;

                var gift = Instantiate(m_contentsPrefab, transform.position, new Quaternion(0, 0, 0, 0));
                var giftPickable = gift.GetComponent<PickableObject>();
                var playerPickup = newParent.gameObject.GetComponentInParent<PlayerPickUpDropObject>();
                playerPickup.ForcePickUpObject(giftPickable);

                if (m_type == Ingredient.NOT_AN_INGREDIENT)
                {
                    gift.GetComponent<ExplosiveItem>().ExplodeByTime(1.75f);
                }
                else
                {
                    var spawnBounce = gift.GetComponent<SpawnBounce>();
                    spawnBounce.CancelForGift();

                    var spawner = FindObjectOfType<ItemSpawner>();
                    ++spawner.SpawnedItemsCount[m_type];
                }

                DespawnThis();

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

        private void DespawnThis()
        {
            DisappearPlaying = true;
            DisappearStartTime = Time.time;

            var colliders = GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.isTrigger = true;
            }
            var rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }

        public void SetColor(Color c)
        {
            m_color = c;
        }

        public void Update()
        {
            if (DisappearPlaying)
            {
                var elapsed = Time.time - DisappearStartTime;
                var progress = Mathf.Clamp01(elapsed / DisappearDuration);

                var scale = DisappearScaleAnim.Evaluate(progress);
                transform.localScale = scale * InitialScale;

                if (progress >= 1.0f)
                    Destroy(gameObject);
            }
        }
    }
}
