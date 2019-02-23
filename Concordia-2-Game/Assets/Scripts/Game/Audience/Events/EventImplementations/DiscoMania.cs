using UnityEngine;
using System.Collections;
using con2;
using con2.game;

public class DiscoMania : AbstractAudienceEvent
{
    public float m_discoManianDuration = 10.0f;

    private Gamepad[] m_playerGamepads;

    void Start()
    {
        SetUpEvent();
    }

    public override Events.EventID GetEventID()
    {
        return Events.EventID.disco_mania;
    }

    public override IEnumerator EventImplementation()
    {
        GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
        GameObject[] players = managers.GetComponentInChildren<SpawnPlayersController>().GetPlayers();

        m_playerGamepads = new Gamepad[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            m_playerGamepads[i] = GamepadMgr.Pad(i);
        }


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
}
