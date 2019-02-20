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

        private GameObject[] m_players;
        private Vector3 m_initialDistanceFromCenter;

        // Start is called before the first frame update
        void Start()
        {
            // Snap at start to avoid interpolation
            m_initialDistanceFromCenter = transform.position;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetPlayers(GameObject[] players)
        {
            m_players = players;
        }

        void LateUpdate()
        {
            camPos = getNewCamTargetPos();
            transform.position = Vector3.SmoothDamp(transform.position, camPos, ref velocity, smoothTime);
        }

        private Vector3 getNewCamTargetPos()
        {
            Vector3 middle = Vector3.zero;
            Vector3 furthestFromMiddle = Vector3.zero;

            for (int i = 0; i < m_players.Length; ++i)
            {
                Vector3 playerPosition = m_players[i].transform.position;
                middle += playerPosition;
                if (playerPosition.magnitude > furthestFromMiddle.magnitude)
                    furthestFromMiddle = playerPosition;
            }

            //Average the positions with the x3 to compensate for the middle
            middle /= (3*m_players.Length);
            middle += m_zoomFactor*m_initialDistanceFromCenter.normalized*furthestFromMiddle.magnitude;  //zoom
            middle += m_initialDistanceFromCenter;  //Keeps camera above the arena, pointed at 'middle'
            return middle;
        }

    }

}
