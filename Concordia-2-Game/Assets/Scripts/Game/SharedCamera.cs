using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class SharedCamera : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        public float smoothTime = 0.3f;

        public float m_zoomFactor = 0.1f;

        private Vector3 velocity = Vector3.zero;

        private Vector3 camPos;

        private Vector3 m_initialDistanceFromCenter;

        private bool activateCamera = false;

        private AMainManager m_mainManager;

        void Awake()
        {
            m_mainManager = FindObjectOfType<AMainManager>();
        }

        void Start()
        {
            // Snap at start to avoid interpolation
            m_initialDistanceFromCenter = transform.position;
        }

        void LateUpdate()
        {
            camPos = getNewCamTargetPos();
            transform.position = Vector3.SmoothDamp(transform.position, camPos, ref velocity, smoothTime);
        }

        private Vector3 getNewCamTargetPos()
        {
            var middle = Vector3.zero;
            var furthestFromMiddle = Vector3.zero;

            if (m_mainManager.PlayersInstances.Count != 0)
            {
                foreach (var t in m_mainManager.PlayersInstances)
                {
                    if (t.Value != null)
                    {
                        var playerPosition = t.Value.gameObject.transform.position;
                        middle += playerPosition;
                        if (playerPosition.magnitude > furthestFromMiddle.magnitude)
                            furthestFromMiddle = playerPosition;
                    }
                }
                middle /= (3 * m_mainManager.PlayersInstances.Count);
            }
            else
            {
                middle = Vector3.zero;
            }

            //Average the positions with the x3 to compensate for the middle
            middle += m_zoomFactor * m_initialDistanceFromCenter.normalized * furthestFromMiddle.magnitude;  //zoom
            middle += m_initialDistanceFromCenter;  //Keeps camera above the arena, pointed at 'middle'
            return middle;
        }

    }

}
