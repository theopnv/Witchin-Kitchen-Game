using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class KitchenSpin : MonoBehaviour
    {
        private bool m_isSpinning = false;
        private float m_spinSpeed;
        private Vector3 m_spinDir;
        private Dictionary<GameObject, GameObject> m_objectsInSpinZoneAndTheirOriginalParents = new Dictionary<GameObject, GameObject>();
        private GameObject m_floor;

        private static List<string> m_keyTags = new List<string> { Tags.PLAYER_TAG, Tags.INGREDIENT, Tags.PROJECTILE, Tags.KITCHEN };

        public void Run()
        {
            //Kitchens must spin
            var kitchens = GameObject.FindGameObjectsWithTag(Tags.KITCHEN);
            foreach (GameObject kitchen in kitchens)
            {
                m_objectsInSpinZoneAndTheirOriginalParents.Add(kitchen, kitchen.transform.parent.gameObject);
            }

            //The floor drives all spinning
            m_floor = transform.parent.gameObject;
        }
        private void FixedUpdate()
        {
            if (m_isSpinning)
            {
                m_floor.transform.Rotate(m_spinDir * Time.deltaTime * m_spinSpeed);
            }
        }

        public void ParentObjectsToBase()
        {
            foreach (GameObject rotatable in m_objectsInSpinZoneAndTheirOriginalParents.Keys)
            {
                if (rotatable == null)
                    continue;
                ParentToThis(rotatable);
            }
        }

        public void UnparentObjectsFromBase()
        {
            foreach (GameObject rotatable in m_objectsInSpinZoneAndTheirOriginalParents.Keys)
            {
                if (rotatable == null)
                    continue;
                Unparent(rotatable);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var collidingObject = other.gameObject;
            if (BelongsToKeyObject(collidingObject))
            {
                return; //Let all children of "key" objects (player, ingredient, etc.) keep their current parentage, they'll be rotated by their parent anyway
            }

            GameObject parentObject = null;
            var p = collidingObject.transform.parent;
            if (p)
                parentObject = p.transform.gameObject;

            if (!m_objectsInSpinZoneAndTheirOriginalParents.ContainsKey(collidingObject))
            {
                m_objectsInSpinZoneAndTheirOriginalParents.Add(collidingObject, parentObject);
            }


            if (m_isSpinning)
            {
                ParentToThis(collidingObject);
            }
        }

        private bool BelongsToKeyObject(GameObject obj)
        {
            Transform t = obj.transform;
            while (t.parent != null)
            {
                if (m_keyTags.Contains(t.parent.tag))
                {
                    return true;
                }
                t = t.parent.transform;
            }
            return false;
        }

        private void OnTriggerExit(Collider other)
        {
            var obj = other.gameObject;
            if (m_isSpinning)
            {
                Unparent(obj);
            }
            m_objectsInSpinZoneAndTheirOriginalParents.Remove(obj);
        }

        private void ParentToThis(GameObject obj)
        {
            obj.transform.parent = this.transform;
        }

        private void Unparent(GameObject obj)
        {
            if (m_objectsInSpinZoneAndTheirOriginalParents.ContainsKey(obj))
            {
                var originalParent = m_objectsInSpinZoneAndTheirOriginalParents[obj];

                Transform parentTransform = null;
                if (originalParent)
                    parentTransform = originalParent.transform;

                obj.transform.parent = parentTransform;
            }
        }

        public void SetSpinSpeed(float speed)
        {
            m_spinSpeed = speed;
        }

        public void SetIsSpinning(bool spin)
        {
            m_isSpinning = spin;
        }

        public void SetSpinDir(Vector3 dir)
        {
            m_spinDir = dir;
        }

    }
}
