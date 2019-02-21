using con2;
using con2.game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudienceEventManager : MonoBehaviour
{
    public Text m_eventText;

    private Func<IEnumerator>[] m_eventFunctions;
    private PlayerMovement[] m_playerMovementControllers;
    private Gamepad[] m_playerGamepads;

    public void InitializeValues()
    {
        GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
        GameObject[] players = managers.GetComponentInChildren<SpawnPlayersController>().GetPlayers();


        m_playerMovementControllers = new PlayerMovement[players.Length];
        m_playerGamepads = new Gamepad[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            m_playerMovementControllers[i] = players[i].GetComponent<PlayerMovement>();
            m_playerGamepads[i] = GamepadMgr.Pad(i);
        }

        m_eventFunctions = new Func<IEnumerator>[] { FreezingRain, DiscoMania };
    }

    private void EventStart(int eventIndex)
    {
        if (eventIndex < m_eventFunctions.Length)
        {
            m_eventText.enabled = true;
            StartCoroutine(RunEvent(m_eventFunctions[eventIndex]));
        }
    }

    public IEnumerator RunEvent(Func<IEnumerator> currentEvent)
    {
        yield return StartCoroutine(currentEvent());
        EventEnd();
    }

    private void EventEnd()
    {
        m_eventText.text = "";
        m_eventText.enabled = false;
    }

    #region FreezingRain

    [Header("FreezingRain")]
    public float m_freezingRainDuration = 10.0f;
    public float m_frictionFraction = 0.2f, m_movementModulator = 1.5f;

    private IEnumerator FreezingRain()
    {
        m_eventText.text = "The audience made the floor slippery!";

        foreach (PlayerMovement player in m_playerMovementControllers)
        {
            player.ModulateMovementFriction(m_frictionFraction);
            player.ModulateMovementSpeed(m_movementModulator);
        }

        yield return new WaitForSeconds(m_freezingRainDuration);

        foreach (PlayerMovement player in m_playerMovementControllers)
        {
            player.ModulateMovementFriction(1.0f / m_frictionFraction);
            player.ModulateMovementSpeed(1.0f / m_movementModulator);
        }
    }

    #endregion

    #region DiscoMania

    [Header("DiscoMania")]
    public float m_discoManianDuration = 10.0f;

    private IEnumerator DiscoMania()
    {
        m_eventText.text = "The audience swapped your controls, it's DiscoMania!";

        foreach (Gamepad pad in m_playerGamepads)
        {
            pad.InvertMovement();
        }

        yield return new WaitForSeconds(m_discoManianDuration);

        foreach (Gamepad pad in m_playerGamepads)
        {
            pad.InvertMovement();
        }
    }

    #endregion
}
