using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MashAGame : MonoBehaviour
{

    public GameObject m_AbuttonIcon, m_pressIcon;
    public float m_timePerState = 0.4f;

    private Vector3 m_movement = new Vector3(0, 0, 0.175f), m_startPos = Vector3.zero;

    private void OnEnable()
    {
        StartCoroutine(LowerButton());
    }

    private IEnumerator RaiseButton()
    {
        m_AbuttonIcon.transform.position += m_movement;
        m_pressIcon.SetActive(false);

        yield return new WaitForSeconds(m_timePerState);

        StartCoroutine(LowerButton());
    }

    private IEnumerator LowerButton()
    {
        if (m_startPos == Vector3.zero)
        {
            m_startPos = m_AbuttonIcon.transform.position;
        }

        m_AbuttonIcon.transform.position -= m_movement;
        m_pressIcon.SetActive(true);

        yield return new WaitForSeconds(m_timePerState);

        StartCoroutine(RaiseButton());
    }

    private void OnDisable()
    {
        m_AbuttonIcon.transform.position = m_startPos;
        m_startPos = Vector3.zero;
    }
}
