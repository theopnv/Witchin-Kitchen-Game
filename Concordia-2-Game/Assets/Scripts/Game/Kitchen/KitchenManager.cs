using UnityEngine;
using UnityEngine.SceneManagement;

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

                switch (SceneManager.GetActiveScene().name)
                {
                    case SceneNames.Lobby:
                        var rml = station.gameObject.AddComponent<RecipeManagerLobby>();
                        rml.Owner = owner;
                        break;
                    case SceneNames.Game:
                        var rmg = station.gameObject.AddComponent<RecipeManagerGame>();
                        rmg.Owner = owner;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
