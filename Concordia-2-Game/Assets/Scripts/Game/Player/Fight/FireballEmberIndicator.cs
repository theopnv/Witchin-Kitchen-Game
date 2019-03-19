using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class FireballEmberIndicator : MonoBehaviour
    {

        public GameObject m_charged, m_player;
        // Start is called before the first frame update
        void Start()
        {
            m_charged.SetActive(true);
            transform.Rotate(Vector3.up * Random.Range(0, 360), Space.Self);
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = m_player.transform.position;
            transform.Rotate(Vector3.up * 3.0f, Space.Self);
        }

        public void SetCharged(bool charged)
        {
            m_charged.SetActive(charged);
        }

        public void SetPlayer(GameObject player)
        {
            m_player = player;
        }
    }
}
