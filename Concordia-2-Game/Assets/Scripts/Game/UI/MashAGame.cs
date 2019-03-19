using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MashAGame : AMinigameUI
{

    public GameObject m_AbuttonIcon, m_pressIcon;
    public float m_timePerState = 0.4f;

    private Vector3 m_movement = new Vector3(0, 0.125f, 0), m_startPos = Vector3.zero;

    private void OnEnable()
    {
        StartCoroutine(LowerButton());
    }

    private IEnumerator RaiseButton()
    {
        m_AbuttonIcon.transform.localPosition += m_movement;
        m_pressIcon.SetActive(false);

        yield return new WaitForSeconds(m_makingProgress ? 0.1f : m_timePerState);

        StartCoroutine(LowerButton());
    }

    private IEnumerator LowerButton()
    {
        if (m_startPos == Vector3.zero)
        {
            m_startPos = m_AbuttonIcon.transform.localPosition;
        }

        m_AbuttonIcon.transform.localPosition -= m_movement;
        m_pressIcon.SetActive(true);

        yield return new WaitForSeconds(m_makingProgress ? 0.1f : m_timePerState);

        StartCoroutine(RaiseButton());
    }

    private void OnDisable()
    {
        m_AbuttonIcon.transform.localPosition = m_startPos;
        m_startPos = Vector3.zero;
    }
}
