using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMashGame : MonoBehaviour
{
    public GameObject m_LTIcon, m_LpressIcon, m_RTIcon, m_RpressIcon;
    public float m_timePerState = 0.2f;

    private Vector3 m_movement = new Vector3(0, 0.26f, 0), m_LstartPos = Vector3.zero, m_RstartPos = Vector3.zero;

    private void OnEnable()
    {
        StartCoroutine(PressLeft());
    }

    private IEnumerator PressLeft()
    {
        if (m_LstartPos == Vector3.zero)
        {
            m_LstartPos = m_LTIcon.transform.position;
            m_RstartPos = m_RTIcon.transform.position;
        }

        m_LTIcon.transform.position -= m_movement;
        m_LpressIcon.SetActive(true);

        yield return new WaitForSeconds(m_timePerState);

        StartCoroutine(RaiseLeft());
    }

    private IEnumerator PressRight()
    {
        m_RTIcon.transform.position -= m_movement;
        m_RpressIcon.SetActive(true);

        yield return new WaitForSeconds(m_timePerState);

        StartCoroutine(RaiseRight());
    }

    private IEnumerator RaiseLeft()
    {
        m_LTIcon.transform.position += m_movement;
        m_LpressIcon.SetActive(false);

        yield return new WaitForSeconds(m_timePerState);

        StartCoroutine(PressRight());
    }

    private IEnumerator RaiseRight()
    {
        m_RTIcon.transform.position += m_movement;
        m_RpressIcon.SetActive(false);

        yield return new WaitForSeconds(m_timePerState);

        StartCoroutine(PressLeft());
    }

    private void OnDisable()
    {
        m_LTIcon.transform.position = m_LstartPos;
        m_RTIcon.transform.position = m_RstartPos;

        m_LstartPos = Vector3.zero;
        m_RstartPos = Vector3.zero;
    }
}
