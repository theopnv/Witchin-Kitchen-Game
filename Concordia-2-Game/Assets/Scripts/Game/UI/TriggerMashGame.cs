using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMashGame : AMinigameUI
{
    public GameObject m_LTIcon, m_LpressIcon, m_RTIcon, m_RpressIcon;
    public float m_timePerState = 0.2f;

    private Vector3 m_movement = new Vector3(0, 0.2f, 0), m_LstartPos = Vector3.zero, m_RstartPos = Vector3.zero;

    private void OnEnable()
    {
        StartCoroutine(PressLeft());
    }

    private IEnumerator PressLeft()
    {
        if (m_LstartPos == Vector3.zero)
        {
            m_LstartPos = m_LTIcon.transform.localPosition;
            m_RstartPos = m_RTIcon.transform.localPosition;
        }

        m_LTIcon.transform.localPosition -= m_movement;
        m_LpressIcon.SetActive(true);

        yield return new WaitForSeconds(m_makingProgress ? 0.1f : m_timePerState);

        StartCoroutine(RaiseLeft());
    }

    private IEnumerator PressRight()
    {
        m_RTIcon.transform.localPosition -= m_movement;
        m_RpressIcon.SetActive(true);

        yield return new WaitForSeconds(m_makingProgress ? 0.1f : m_timePerState);

        StartCoroutine(RaiseRight());
    }

    private IEnumerator RaiseLeft()
    {
        m_LTIcon.transform.localPosition += m_movement;
        m_LpressIcon.SetActive(false);

        yield return new WaitForSeconds(m_makingProgress ? 0f : m_timePerState);

        StartCoroutine(PressRight());
    }

    private IEnumerator RaiseRight()
    {
        m_RTIcon.transform.localPosition += m_movement;
        m_RpressIcon.SetActive(false);

        yield return new WaitForSeconds(m_makingProgress ? 0f : m_timePerState);

        StartCoroutine(PressLeft());
    }

    private void OnDisable()
    {
        m_LTIcon.transform.localPosition = m_LstartPos;
        m_RTIcon.transform.localPosition = m_RstartPos;

        m_LstartPos = Vector3.zero;
        m_RstartPos = Vector3.zero;
    }
}
