using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballForAll : ASpell
{
    public float m_FireballForAllDuration = 10.0f, m_rotationSpeedY = 0.1f;
    private float m_reloadModulator = 0.01f;
    private bool m_doneRotation = true;
    private Transform m_fireballLoc;

    // Start is called before the first frame update

    void Start()
    {
        SetUpSpell();
    }

    public override IEnumerator SpellImplementation()
    {
        GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
        GameObject player = managers.GetComponentInChildren<SpawnPlayersController>().GetPlayers()[_TargetedPlayer.id];
        PlayerFireball playerFireball = player.GetComponentInChildren<PlayerFireball>();
        m_fireballLoc = playerFireball.gameObject.transform;

        playerFireball.ModulateReloadTime(m_reloadModulator);
        m_doneRotation = false;
        StartCoroutine(Cast(playerFireball));
        StartCoroutine(RotateSpawner(playerFireball));

        yield return null;
    }

    public IEnumerator RotateSpawner(PlayerFireball playerFireball)
    {
        yield return new WaitForSeconds(m_FireballForAllDuration);

        m_doneRotation = true;
        m_fireballLoc.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        playerFireball.ModulateReloadTime(1.0f / m_reloadModulator);
        yield return null;
    }

    private void Update()
    {
        if (!m_doneRotation)
        {
            Quaternion rotation = m_fireballLoc.rotation;
            float CurRotationY = rotation.y + m_rotationSpeedY * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0.0f, CurRotationY, 0.0f);
        }
    }

    public IEnumerator Cast(PlayerFireball playerFireball)
    {
        while (!m_doneRotation)
        {
            playerFireball.CastFireball();
            yield return new WaitForSeconds(0.75f);
        }
    }

    public override Spells.SpellID GetSpellID()
    {
        return Spells.SpellID.fireball_for_all;
    }
}
