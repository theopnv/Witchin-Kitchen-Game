using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputContextSwitcher : MonoBehaviour
{
    public void Start()
    {
        GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
        GamepadMgr gamepadManager = managers.GetComponentInChildren<GamepadMgr>();

        //Ask gpm for number of player, use that number to set contexts (with playcercontroller, and only allow menu input for p1)
        int x = GamepadMgr.NUM_PADS;
        gamepadManager.SwitchGamepadContext(GetMenuContext(), i);
    }

    private List<IInputConsumer> GetMenuContext()
    {
        List<IInputConsumer> inputConsumers = new List<IInputConsumer>();
        return inputConsumers;
    }

    private List<IInputConsumer> GetGameContext()
    {
        List<IInputConsumer> inputConsumers = new List<IInputConsumer>();
        GameObject[] kitchenParents = GameObject.FindGameObjectsWithTag(Tags.KITCHEN);
        List<CookingMinigame> kitchenStations = new List<CookingMinigame>();
        foreach (GameObject kitchen in kitchenParents)
        {
            CookingMinigame[] stations = kitchen.GetComponentsInChildren<CookingMinigame>();
            foreach (CookingMinigame station in stations)
            {
                kitchenStations.Add(station);
            }
        }    
          
        foreach (CookingMinigame station in kitchenStations)
        {
            inputConsumers.Add(station);
        }

        GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
        SpawnPlayersController playerSpawner = managers.GetComponentInChildren<SpawnPlayersController>();
        GameObject[] players = playerSpawner.GetPlayers();
        foreach (GameObject player in players)
        {
            PlayerInputController controller = player.GetComponent<PlayerInputController>();
            if (controller.GetPlayerIndex() == playerID)
            {
                m_player = player;
                inputConsumers.Add(controller);
            }
        }
        return inputConsumers;
    }
}
