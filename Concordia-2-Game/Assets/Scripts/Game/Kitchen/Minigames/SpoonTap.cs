using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoonTap : MonoBehaviour
{
    public GameObject m_LPlacement, m_RPlacement;
    Quaternion m_startOrientation, m_LOrientation, m_ROrientation;
    public float m_speed = 3;
    private bool m_movingIntoPlace, m_left, m_resettingPos, m_gameIsOn;

    // Start is called before the first frame update
    void Start()
    {
        m_LOrientation = m_LPlacement.transform.localRotation;
        m_ROrientation = m_RPlacement.transform.localRotation;
        m_startOrientation = transform.localRotation;
        EndTap();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_movingIntoPlace)
        {
            if (Quaternion.Angle(transform.localRotation, m_LOrientation) < 5)
            {
                m_movingIntoPlace = false;
            }
            else
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, m_LOrientation, Time.deltaTime * m_speed);
            }
        }
        else if (m_resettingPos)
        {
            if (Quaternion.Angle(transform.localRotation, m_startOrientation) < 5)
            {
                m_resettingPos = false;
            }
            else
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, m_startOrientation, Time.deltaTime * m_speed);
            }
        }
        else if (m_gameIsOn)
        {
            if (m_left)
            {
                //Magic numbers that just work? Clearly I don't get quaternions that well trololol
                if (Quaternion.Angle(transform.localRotation, m_LOrientation) > 5)
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, m_LOrientation, Time.deltaTime * m_speed);
            }
            else
            {
                if (Quaternion.Angle(transform.localRotation, m_ROrientation) > 5)
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, m_ROrientation, Time.deltaTime * m_speed);
            }
        }
    }

    public void StartTap()
    {
        m_gameIsOn = true;
        m_movingIntoPlace = true;
        m_resettingPos = false;
        m_left = true;
    }

    public void Aim(bool left)
    {
        m_left = left;
    }

    public void EndTap()
    {
        m_movingIntoPlace = false;
        m_left = false;
        m_resettingPos = true;
        m_gameIsOn = false;
    }

}

