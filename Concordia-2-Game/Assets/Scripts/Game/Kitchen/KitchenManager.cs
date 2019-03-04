using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game 
{
    public class KitchenManager : MonoBehaviour
    {
        public int m_ownerId;

        public void SetOwner(SpawnPlayersController spawnController)
        {
            var owner = spawnController.GetPlayerByID(m_ownerId);
            if (owner == null)
            {
                Debug.LogError("Kitchen owner player " + m_ownerId + " not found!");
            }
            else
            {
                var stations = GetComponentsInChildren<KitchenStation>();
                foreach (var station in stations)
                {
                    station.SetOwner(owner);
                }
            }
        }
    }
}
