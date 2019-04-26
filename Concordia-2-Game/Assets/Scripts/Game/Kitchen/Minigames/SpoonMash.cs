using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoonMash : MonoBehaviour
{
    public GameObject m_mashPlacement;
    Quaternion m_startOrientation, m_mashOrientation;
    public float m_speed = 3;
    private bool m_movingIntoPlace, m_mashing, m_resettingPos, m_gameIsOn;
    private float m_desiredStartAngle = 225;

    // Start is called before the first frame update
    void Start()
    {
        m_mashOrientation = m_mashPlacement.transform.localRotation;
        m_startOrientation = transform.localRotation;
        EndMash();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_movingIntoPlace)
        {
            if (Quaternion.Angle(transform.localRotation, m_mashOrientation) < 5)
            {
                m_movingIntoPlace = false;
            }
            else
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, m_mashOrientation, Time.deltaTime * m_speed);
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
            transform.Rotate(Vector3.up, m_speed, Space.World);
            if (m_mashing)
            {
                Vector3 pos = transform.position;
                pos.y -= Time.deltaTime * m_speed;
                transform.position = pos;

                if (pos.y <= 0.2f)
                    m_mashing = false;
            }
            else
            {
                Vector3 pos = transform.position;
                pos.y += Time.deltaTime * m_speed * 2;  

                if (pos.y <= 0.6f)
                    transform.position = pos;
            }
        }
    }

    public void StartMash()
    {
        m_gameIsOn = true;
        m_movingIntoPlace = true;
        m_resettingPos = false;
    }

    public void Mash()
    {
        m_mashing = true;
    }

    public void EndMash()
    {
        m_movingIntoPlace = false;
        m_mashing = false;
        m_resettingPos = true;
        m_gameIsOn = false;
    }

}

