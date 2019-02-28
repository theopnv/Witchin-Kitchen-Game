using con2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireball : MonoBehaviour, IInputConsumer
{
    [SerializeField] private GameObject m_fireballPrefab, m_parent;
    [SerializeField] private float m_fireballSpeed = 4.0f;
    [SerializeField] private float m_fireballFlyTime = 2.0f;
    [SerializeField] private float m_fireballReloadSeconds = 8.0f;

    private GameObject m_currentFireball;
    private bool m_canCastFireball = true;

    public void Start()
    {
        m_parent = GameObject.Find("Environment");
    }

    public bool ConsumeInput(GamepadAction input)
    {
        if (!m_canCastFireball)
            return false;

        if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.FIREBALL))
        {
            CastFireball();
            return true;
        }

        return false;
    }

    private void CastFireball()
    {
        m_currentFireball = Instantiate(m_fireballPrefab, m_parent.transform);
        m_currentFireball.transform.position = transform.position;
        m_currentFireball.transform.forward = transform.parent.forward;
        Rigidbody rb = m_currentFireball.GetComponent<Rigidbody>();
        rb.velocity = transform.parent.forward * m_fireballSpeed;
        m_canCastFireball = false;
        StartCoroutine(FireballCooldown());
        StartCoroutine(FireballDespawn());
    }

    private IEnumerator FireballDespawn()
    {
        yield return new WaitForSeconds(m_fireballFlyTime);
        GameObject.Destroy(m_currentFireball);
        m_currentFireball = null;
    }

    private IEnumerator FireballCooldown()
    {
        yield return new WaitForSeconds(m_fireballReloadSeconds);
        m_canCastFireball = true;
    }
}
