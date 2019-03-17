using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StirGame : MonoBehaviour
{
    public int m_spinDir;

    public GameObject m_arrow;
    public float m_spinSpeed = 10.0f;
    private bool m_justStarted = true;
    private Vector3 m_originalScale;

    private void OnEnable()
    {
        StartCoroutine(SpinArrow());
    }

    private IEnumerator SpinArrow()
    {
        if (m_justStarted)
        {
            m_justStarted = false;
            m_originalScale = m_arrow.transform.localScale;
            if (m_spinDir == 1)
                m_arrow.transform.localScale = new Vector3(-m_originalScale.x, m_originalScale.y, m_originalScale.z);
        }

        Transform arrow = m_arrow.transform;
        while (true)
        {
            arrow.Rotate(Vector3.back * m_spinDir, m_spinSpeed);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDisable()
    {
        m_justStarted = true;
        m_arrow.transform.localScale = m_originalScale;
    }
}
