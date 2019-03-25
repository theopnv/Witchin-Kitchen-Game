using UnityEngine;
using System.Collections;
using con2.game;

public class FreezingRainEvent : AbstractAudienceEvent
{
    public float m_freezingRainDuration = 10.0f;
    public float m_dragFraction = 0.01f, m_movementModulator = 1.5f;
    public Freeze m_freeze;
    public GameObject m_floor;
    public PhysicMaterial m_freezePhysicsPlayer, m_originalPhysicsPlayer, m_frozenFloor, m_normalFloor, m_frozenItem, m_normalItem;

    private PlayerMovement[] m_playerMovementControllers;

    void Start()
    {
        SetUpEvent();
    }

    public override void EventStart()
    {
        _MessageFeedManager.AddMessageToFeed("The audience made the floor slippery!", MessageFeedManager.MessageType.arena_event);
    }

    public override IEnumerator EventImplementation()
    {
        var players = m_mainGameManager.PlayersInstances;

        m_playerMovementControllers = new PlayerMovement[players.Count];
        for (int i = 0; i < players.Count; i++)
        {
            m_playerMovementControllers[i] = players[i].GetComponent<PlayerMovement>();
        }
        
        foreach (PlayerMovement player in m_playerMovementControllers)
        {
            player.ModulateMaxMovementSpeed(m_movementModulator);
            player.ModulateMovementSpeed(1.0f / m_movementModulator);
            player.ModulateRotationSpeed(m_movementModulator);
            var collider = player.GetComponent<CapsuleCollider>();
            collider.material = m_freezePhysicsPlayer;
        }

        var items = FindObjectsOfType<PickableObject>();
        foreach (var item in items)
        {
            var colliders = item.GetComponentsInChildren<Collider>();
            foreach (var c in colliders)
            {
                if (!c.isTrigger)
                    c.material = m_frozenItem;
            }
        }

        var floor = m_floor.GetComponent<BoxCollider>();
        floor.material = m_frozenFloor;

        m_freeze.PlayFreeze();

        yield return new WaitForSeconds(m_freezingRainDuration);

        foreach (PlayerMovement player in m_playerMovementControllers)
        {
            player.ModulateMaxMovementSpeed(1.0f / m_movementModulator);
            player.ModulateMovementSpeed(m_movementModulator);
            player.ModulateRotationSpeed(1.0f / m_movementModulator);
            var collider = player.GetComponent<CapsuleCollider>();
            collider.material = m_originalPhysicsPlayer;
        }

        foreach (var item in items)
        {
            if (item != null)
            {
                var colliders = item.GetComponentsInChildren<Collider>();
                foreach (var c in colliders)
                {
                    if (!c.isTrigger)
                        c.material = m_normalItem;
                }
            }
        }

        floor.material = m_normalFloor;

        m_freeze.PlayThaw();
    }

    public override Events.EventID GetEventID()
    {
        return Events.EventID.freezing_rain;
    }
}
