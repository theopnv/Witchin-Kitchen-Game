using UnityEngine;

namespace con2.game
{

    public class PickableObject : MonoBehaviour, IPunchable
    {
        protected Rigidbody m_rb;
        private bool m_isHeld = false;

        public float m_lerpSpeedz = 10.0f;

        private float m_dropTimestamp;

        void Start()
        {
            m_rb = GetComponent<Rigidbody>();
        }

        // Called by the carrying player's Update() to force the object to follow it
        public void UpdatePosition(Vector3 currentVel)
        {
            if (m_isHeld)
            {
                var parent = transform.parent;
                if (parent)
                {
                    transform.position = Vector3.Lerp(transform.position, parent.position, Time.deltaTime * m_lerpSpeedz);
                    m_rb.velocity = currentVel / m_rb.mass;
                }
                else
                {
                    m_isHeld = false;
                    m_dropTimestamp = Time.time;
                }
            }
        }

        // Get picked up
        virtual public bool PickUp(Transform newParent)
        {
            if (!m_isHeld)
            {
                m_isHeld = true;

                // Reset rotation
                transform.parent = newParent;

                if (m_rb == null)
                {
                    m_rb = GetComponent<Rigidbody>();
                }

                // Disable the use of gravity, remove the velocity, and freeze rotation (will all be driven by player movement)
                m_rb.useGravity = false;
                m_rb.velocity = Vector3.zero;
                m_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

                return true;
            }

            return false;
        }

        //Get dropped
        public void Drop(Vector3 throwVector)
        {
            if (m_rb == null)
            {
                m_rb = GetComponent<Rigidbody>();
            }

            m_isHeld = false;

            // Re-Enable the use of gravity on the object and remove all constraints
            m_rb.useGravity = true;
            m_rb.constraints = RigidbodyConstraints.None;

            // Get thrown forward
            m_rb.AddForce(throwVector, ForceMode.VelocityChange);

            // Unparent the object from the player
            transform.parent = null;

            m_dropTimestamp = Time.time;
        }

        public void Punch(Vector3 knockVelocity, float stunTime)
        {
            if (!m_isHeld && Time.time - m_dropTimestamp > 0.1f)
            {
                if (m_rb == null)
                {
                    m_rb = GetComponent<Rigidbody>();
                }
                m_rb.AddForce(knockVelocity, ForceMode.VelocityChange);
            }
        }

        public bool IsHeld()
        {
            return m_isHeld;
        }

        public float GetMaxSpeedFractionWhenHolding()
        {
            return GetComponentInChildren<ObjectWeight>().m_maxSpeedFractionWhenHolding;
        }

    }

}
