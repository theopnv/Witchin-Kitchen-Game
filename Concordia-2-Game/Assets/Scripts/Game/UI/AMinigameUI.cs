using UnityEngine;
using System.Collections;

public abstract class AMinigameUI : MonoBehaviour
{
    protected bool m_makingProgress = false;
    protected float m_progressTimestamp;

    public void MakingProgress()
    {
        m_makingProgress = true;
        m_progressTimestamp = Time.time;
    }

    private void Update()
    {
        if (Time.time - m_progressTimestamp >= 0.5f)
        {
            m_makingProgress = false;
        }

        if (m_makingProgress)
        {

        }
    }
}
