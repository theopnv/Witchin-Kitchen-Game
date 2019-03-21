using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaMagePunch : ASpell
{
    public float m_megaMagePunchDuration = 10.0f, m_megaMageMultiplier = 2.0f, m_sizeScaler = 1.2f;

    // Start is called before the first frame update

    void Start()
    {
        SetUpSpell();
    }

    public override IEnumerator SpellImplementation()
    {
        var player = m_mainGameManager.GetPlayerById(_TargetedPlayer.id);
        var playerPunch = player.GetComponentInChildren<PlayerPunch>();
        var playerTransform = player.gameObject.transform;

        playerPunch.ModulatePunchStrength(m_megaMageMultiplier);
        playerTransform.localScale *= m_sizeScaler;

        yield return new WaitForSeconds(m_megaMagePunchDuration);

        playerPunch.ModulatePunchStrength(1.0f / m_megaMageMultiplier);
        playerTransform.localScale /= m_sizeScaler;
    }

    public override Spells.SpellID GetSpellID()
    {
        return Spells.SpellID.mega_mage_punch;
    }
}
