using con2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireball : MonoBehaviour, IInputConsumer
{
    [SerializeField] private GameObject m_fireballPrefab;
    [SerializeField] public float m_reloadSeconds, m_recoil;

    private GameObject m_spawnParent;
    private Rigidbody m_player;
    private Projectile m_currentFireball;
    private bool m_canCastFireball = true;

    public void Start()
    {
        m_spawnParent = GameObject.Find("Environment");
        m_player = transform.parent.gameObject.GetComponent<Rigidbody>();
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
        //The enviroment (terrain) is the parent of the fireball, but we position it initially based on the player
        GameObject newFireball = Instantiate(m_fireballPrefab, m_spawnParent.transform);
        newFireball.transform.position = transform.position;
        newFireball.transform.forward = transform.forward;
        newFireball.GetComponent<Projectile>().m_launcher = transform.parent.gameObject;

        Recoil();

        StartCoroutine(FireballCooldown());
    }

    private void Recoil()
    {
        m_player.AddForce(-transform.forward * m_recoil, ForceMode.VelocityChange);
    }

    private IEnumerator FireballCooldown()
    {
        m_canCastFireball = false;
        yield return new WaitForSeconds(m_reloadSeconds);
        m_canCastFireball = true;
    }
}
