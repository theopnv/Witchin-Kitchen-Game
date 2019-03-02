using UnityEngine;
using System.Collections;
using con2.game;

public class FreezingRainEvent : AbstractAudienceEvent
{
    public float m_freezingRainDuration = 10.0f;
    public float m_dragFraction = 0.01f, m_movementModulator = 1.5f;

    private PlayerMovement[] m_playerMovementControllers;

    void Start()
    {
        SetUpEvent();
    }

    public override Events.EventID GetEventID()
    {
        return Events.EventID.freezing_rain;
    }

    public override void EventStart()
    {}

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
            player.ModulateMovementSpeed(m_movementModulator);
        }

        FloorFriction[] frictionControllers = GameObject.FindObjectsOfType<FloorFriction>();
        foreach (FloorFriction controller in frictionControllers)
        {
            controller.ModulateFriction(-m_dragFraction);
        }

        yield return new WaitForSeconds(m_freezingRainDuration);

        foreach (PlayerMovement player in m_playerMovementControllers)
        {
            player.ModulateMovementSpeed(1.0f / m_movementModulator);
        }

        foreach (FloorFriction controller in frictionControllers)
        {
            controller.ModulateFriction(1.0f / -m_dragFraction);
        }
    }
}
