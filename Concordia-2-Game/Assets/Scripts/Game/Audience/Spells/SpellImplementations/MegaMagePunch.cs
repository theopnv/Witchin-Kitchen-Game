﻿using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MegaMagePunch : ASpell
{
    public float m_megaMagePunchDuration = 10.0f, m_megaMageMultiplier = 2.0f, m_sizeScaler = 1.2f;

    public AnimationCurve ScaleAnim;
    protected Vector3 OriginalScale;
    protected float StartTime;
    protected bool Playing = false;
    public GameObject promptPrefab;
    private GameObject instantiatedPrompt;
    private PlayerPickUpDropObject m_ppdo;

    // Start is called before the first frame update

    void Start()
    {
        SetUpSpell();
    }

    public override IEnumerator SpellImplementation()
    {
        if (!m_inProgress)
        {
            m_inProgress = true;
            var player = m_mainManager.GetPlayerById(_TargetedPlayer.id);
            var playerPunch = player.GetComponentInChildren<PlayerPunch>();
            var playerMovement = player.GetComponentInChildren<PlayerMovement>();
            m_ppdo = player.GetComponentInChildren<PlayerPickUpDropObject>();
            var hitbox = playerPunch.gameObject.transform;
            var hitboxScale = hitbox.localScale;
            var charModel = player.GetComponentInChildren<Animator>().gameObject.transform;

            playerPunch.ModulatePunchStrength(m_megaMageMultiplier);
            playerPunch.ModulatePunchCooldown(1.0f / m_megaMageMultiplier);
            var originalShake = playerPunch.ShakeIntensity;
            playerPunch.ShakeIntensity = CamShakeMgr.Intensity.MEDIUM;
            playerMovement.ModulateMovementSpeed(m_sizeScaler);
            playerMovement.ModulateMaxMovementSpeed(m_sizeScaler);
            playerMovement.SetImmunity(true);
            hitbox.localScale = new Vector3(hitboxScale.x * m_sizeScaler, hitboxScale.y, hitboxScale.z * m_sizeScaler);

            Playing = true;
            StartTime = Time.time;
            OriginalScale = charModel.localScale;

            instantiatedPrompt = Instantiate(promptPrefab);

            yield return new WaitForSeconds(m_megaMagePunchDuration);
            m_inProgress = false;

            playerPunch.ModulatePunchStrength(1.0f / m_megaMageMultiplier);
            playerPunch.ModulatePunchCooldown(m_megaMageMultiplier);
            playerPunch.ShakeIntensity = originalShake;
            playerMovement.ModulateMovementSpeed(1.0f / m_sizeScaler);
            playerMovement.ModulateMaxMovementSpeed(1.0f / m_sizeScaler);
            playerMovement.SetImmunity(false);
            hitbox.localScale = new Vector3(hitboxScale.x, hitboxScale.y, hitboxScale.z);

            Destroy(instantiatedPrompt.gameObject);

            Playing = false;
            charModel.localScale = OriginalScale;
            m_ppdo = null;
        }
    }

    public override Spells.SpellID GetSpellID()
    {
        return Spells.SpellID.mega_mage_punch;
    }

    protected void Update()
    {
        if (Playing)
        {
            var elapsed = Time.time - StartTime;
            var progress = Mathf.Clamp01(elapsed / m_megaMagePunchDuration);

            var player = m_mainManager.GetPlayerById(_TargetedPlayer.id);
            var charModel = player.GetComponentInChildren<Animator>().gameObject.transform;

            var scale = ScaleAnim.Evaluate(progress);
            charModel.localScale = OriginalScale * scale;

            var ingredient = m_ppdo.GetHeldObject();
            if (ingredient)
            {
                ingredient.localScale = Vector3.one / scale;
            }

            instantiatedPrompt.transform.localScale = scale/2.0f * Vector3.one;
            instantiatedPrompt.transform.position = player.transform.position + new Vector3(1.5f, 5, 0);
        }
    }
}
