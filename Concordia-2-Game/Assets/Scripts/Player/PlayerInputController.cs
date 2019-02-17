using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInputController : MonoBehaviour, IInputConsumer
{
    private int m_playerIndex;
    private List<IInputConsumer> m_InputConsumers;

    void Start()
    {
        m_InputConsumers = new List<IInputConsumer>();
        m_InputConsumers.Add(GetComponent<PlayerMovement>());
        m_InputConsumers.Add(GetComponent<PlayerPunch>());
        //m_InputConsumers.Add(GetComponent<LongRangeAttackControls>());
    }

    public bool ConsumeInput(GamepadAction input)
    {
        foreach (IInputConsumer consumer in m_InputConsumers)
        {
            if (consumer.ConsumeInput(input))
            {
                return true;
            }
        }
        return false;
    }

    public void SetPlayerIndex(int newIndex)
    {
        m_playerIndex = newIndex;
    }

    public int GetPlayerIndex()
    {
        return m_playerIndex;
    }
}
