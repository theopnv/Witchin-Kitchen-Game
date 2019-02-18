using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    public int m_ownerId;

    public void SetOwner(SpawnPlayersController playerSpawner)
    {
        GameObject owner = playerSpawner.GetPlayerByID(m_ownerId);
        if (owner == null)
        {
            Debug.LogError("Kitchen owner player " + m_ownerId + " not found!");
        }
        else
        {
            KitchenStation[] stations = GetComponentsInChildren<KitchenStation>();
            foreach (KitchenStation station in stations)
            {
                station.SetOwner(owner);
            }
        }
    }
}
