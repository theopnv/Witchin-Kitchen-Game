using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballForAll : ASpell
{
    public float m_FireballForAllDuration = 10.0f, m_reloadModulator = 0.01f;

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

        playerFireball.ModulateReloadTime(m_reloadModulator);

        yield return new WaitForSeconds(m_FireballForAllDuration);

        playerFireball.ModulateReloadTime(1.0f / m_reloadModulator);
    }

    public override Spells.SpellID GetSpellID()
    {
        return Spells.SpellID.fireball_for_all;
    }
}
