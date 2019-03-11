using UnityEngine;
using System.Collections;
using con2.game;

public class FreezingRainEvent : AbstractAudienceEvent
{
    public float m_freezingRainDuration = 10.0f;
    public float m_dragFraction = 0.01f, m_movementModulator = 1.5f;
    public Freeze m_freeze;

    private PlayerMovement[] m_playerMovementControllers;

    void Start()
    {
        SetUpEvent();
    }

    public override void EventStart()
    {}

    public override IEnumerator EventImplementation()
    {
        var players = Players.Dic;

        m_playerMovementControllers = new PlayerMovement[players.Count];
        for (int i = 0; i < players.Count; i++)
        {
            m_playerMovementControllers[i] = players[i].GetComponent<PlayerMovement>();
        }

        _MessageFeedManager.AddMessageToFeed("The audience made the floor slippery!", MessageFeedManager.MessageType.arena_event);

        foreach (PlayerMovement player in m_playerMovementControllers)
        {
            player.ModulateMovementSpeed(m_movementModulator);
            player.ModulateRotationSpeed(m_movementModulator);
        }

        var frictionControllers = FindObjectsOfType<FloorFriction>();
        foreach (FloorFriction controller in frictionControllers)
        {
            controller.ModulateFriction(-m_dragFraction);
        }

        m_freeze.PlayFreeze();

        yield return new WaitForSeconds(m_freezingRainDuration);

        foreach (PlayerMovement player in m_playerMovementControllers)
        {
            player.ModulateMovementSpeed(1.0f / m_movementModulator);
            player.ModulateRotationSpeed(1.0f / m_movementModulator);
        }

        foreach (FloorFriction controller in frictionControllers)
        {
            controller.ModulateFriction(1.0f / -m_dragFraction);
        }

        m_freeze.PlayThaw();
    }

    public override Events.EventID GetEventID()
    {
        return Events.EventID.freezing_rain;
    }
}
