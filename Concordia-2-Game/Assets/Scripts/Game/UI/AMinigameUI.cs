using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class AMinigameUI : MonoBehaviour
{
    protected bool m_makingProgress = false;
    protected float m_progressTimestamp;

    public GameObject m_backdrop;

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
            m_backdrop.GetComponent<Image>().color = Color.green;
        }
        else
        {
            m_backdrop.GetComponent<Image>().color = Color.white;
        }
    }
}
