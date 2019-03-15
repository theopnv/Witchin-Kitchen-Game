using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class FireballEmberIndicator : MonoBehaviour
    {

        public GameObject m_uncharged, m_charged, m_player;
        // Start is called before the first frame update
        void Start()
        {
            m_charged.SetActive(true);
            m_uncharged.SetActive(false);
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
            if (charged)
            {
                m_charged.SetActive(true);
                m_uncharged.SetActive(false);
            }
            else
            {
                m_charged.SetActive(false);
                m_uncharged.SetActive(true);
            }
        }

        public void SetPlayer(GameObject player)
        {
            m_player = player;
        }
    }
}
