using UnityEngine;
using System.Collections;
using con2.game;

public class FreezingRain : AbstractAudienceEvent
{
    public float m_freezingRainDuration = 10.0f;
    public float m_frictionFraction = 0.2f, m_movementModulator = 1.5f;

    private PlayerMovement[] m_playerMovementControllers;

    public override Events.EventID GetEventID()
    {
        return Events.EventID.freezing_rain;
    }

    public override IEnumerator EventImplementation()
    {
        GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
        GameObject[] players = managers.GetComponentInChildren<SpawnPlayersController>().GetPlayers();


        m_playerMovementControllers = new PlayerMovement[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            m_playerMovementControllers[i] = players[i].GetComponent<PlayerMovement>();
        }

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
}
