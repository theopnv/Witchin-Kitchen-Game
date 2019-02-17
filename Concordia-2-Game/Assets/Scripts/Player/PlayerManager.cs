using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private GameObject[] m_players;

    void Update()
    {
        if (m_players == null)
        {
            m_players = GameObject.FindGameObjectsWithTag("PlayerCapsule");
        }
    }


    public GameObject[] GetPlayers()
    {
        return m_players;
    }
}
