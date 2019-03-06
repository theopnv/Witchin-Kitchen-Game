using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaMagePunch : ASpell
{
    public float m_megaMagePunchDuration = 10.0f, m_megaMageMultiplier = 2.0f;

    // Start is called before the first frame update

    void Start()
    {
        SetUpSpell();
    }

    public override IEnumerator SpellImplementation()
    {
        var player = Players.GetPlayerByID(_TargetedPlayer.id);
        var playerPunch = player.GetComponentInChildren<PlayerPunch>();
        playerPunch.ModulatePunchStrength(m_megaMageMultiplier);

        yield return new WaitForSeconds(m_megaMagePunchDuration);

        playerPunch.ModulatePunchStrength(1.0f / m_megaMageMultiplier);
    }

    public override Spells.SpellID GetSpellID()
    {
        return Spells.SpellID.mega_mage_punch;
    }
}
