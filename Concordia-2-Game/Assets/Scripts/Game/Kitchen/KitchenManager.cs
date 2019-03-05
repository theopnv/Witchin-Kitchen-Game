using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game 
{
    public class KitchenManager : MonoBehaviour
    {
        public void SetOwner(int ownerId)
        {
            var owner = Players.GetPlayerByID(ownerId);
            if (owner == null)
            {
                Debug.LogError("Kitchen owner player " + ownerId + " not found!");
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
