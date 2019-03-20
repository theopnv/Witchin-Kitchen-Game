using UnityEngine;

namespace con2.game
{
    public class KitchenManager : MonoBehaviour
    {
        public void SetOwner(PlayerManager owner)
        {
            var stations = GetComponentsInChildren<KitchenStation>();
            foreach (var station in stations)
            {
                station.SetOwner(owner);
            }
        }
    }
}
