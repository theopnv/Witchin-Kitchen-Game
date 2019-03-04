using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballForAll : ASpell
{
    public float m_FireballForAllDuration = 10.0f, m_rotationSpeedY = 0.1f;
    public GameObject m_fireballerPrefab;
    private GameObject m_fireballer;
    private PlayerManager m_player;
    private bool m_doneRotation = true;

    // Start is called before the first frame update

    void Start()
    {
        SetUpSpell();
    }

    public override IEnumerator SpellImplementation()
    {
        var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
        m_player = managers.GetComponentInChildren<SpawnPlayersController>().GetPlayerByID(_TargetedPlayer.id);
        var playerFireball = m_player.GetComponentInChildren<PlayerFireball>();
        playerFireball.SetCanCast(false);

        m_fireballer = Instantiate(m_fireballerPrefab); 

        m_doneRotation = false;
        StartCoroutine(Cast());
        StartCoroutine(RotateSpawner(playerFireball));

        yield return null;
    }

    public IEnumerator RotateSpawner(PlayerFireball playerFireball)
    {
        yield return new WaitForSeconds(m_FireballForAllDuration);

        m_doneRotation = true;
        playerFireball.SetCanCast(true);
        GameObject.Destroy(m_fireballer);
        yield return null;
    }

    private void Update()
    {
        if (!m_doneRotation)
        {
            m_fireballer.transform.position = m_player.transform.position;
            m_fireballer.transform.Rotate(new Vector3(0.0f, m_rotationSpeedY * Time.deltaTime, 0.0f), Space.Self);
        }
    }

    public IEnumerator Cast()
    {
        PlayerFireball fireballer = m_fireballer.GetComponent<PlayerFireball>();
        while (!m_doneRotation)
        {
            yield return new WaitForSeconds(0.2f);
            fireballer.FireballTurret(m_player.gameObject);
        }
    }

    public override Spells.SpellID GetSpellID()
    {
        return Spells.SpellID.fireball_for_all;
    }
}
