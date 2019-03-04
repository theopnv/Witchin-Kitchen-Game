using con2;
using con2.game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputContextSwitcher : MonoBehaviour
{
    Func<int, List<IInputConsumer>> f_menuContext, f_gameContext;

     public void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        f_menuContext = GetMenuContext;
        f_gameContext = GetGameContext;

        //Initialize gamepads
        GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
        GamepadMgr gp = managers.GetComponentInChildren<GamepadMgr>();
        gp.InitializeGampads();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals(SceneNames.MainMenu))
        {
            SetToMenuContext();
        }
    }

    public void SetToGameContext()
    {
        SwitchContext(f_gameContext);
    }

    public void SetToMenuContext()
    {
        SwitchContext(f_menuContext);
    }

    private static void SwitchContext(Func<int, List<IInputConsumer>> contextFunction)
    {
        //Ask gpm for number of player, use that number to set contexts (with playercontroller, and only allow menu input for p1)
        for (int i = 0; i < GamepadMgr.NUM_PADS; i++)
        {
            GamepadMgr.Pad(i).SwitchGamepadContext(contextFunction(i), i);
        }
    }

    private static List<IInputConsumer> GetMenuContext(int playerIndex)
    {
        var inputConsumers = new List<IInputConsumer>();
        return inputConsumers;
    }

    private static List<IInputConsumer> GetGameContext(int playerIndex)
    {
        var inputConsumers = new List<IInputConsumer>();

        var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
        var mgm = managers.GetComponentInChildren<MainGameManager>();
        inputConsumers.Add(mgm);

        var kitchenParents = GameObject.FindGameObjectsWithTag(Tags.KITCHEN);
        var kitchenStations = new List<ACookingMinigame>();
        foreach (var kitchen in kitchenParents)
        {
            var stations = kitchen.GetComponentsInChildren<ACookingMinigame>();
            kitchenStations.AddRange(stations);
        }    
          
        foreach (ACookingMinigame station in kitchenStations)
        {
            inputConsumers.Add(station);
        }

        var playerSpawner = managers.GetComponentInChildren<SpawnPlayersController>();
        var player = playerSpawner.GetPlayerByID(playerIndex);
        inputConsumers.Add(player.GetComponent<PlayerInputController>());
        
        return inputConsumers;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
