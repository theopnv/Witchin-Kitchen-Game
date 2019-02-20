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
        f_menuContext = new Func<int, List<IInputConsumer>>(GetMenuContext);
        f_gameContext = new Func<int, List<IInputConsumer>>(GetGameContext);

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
        List<IInputConsumer> inputConsumers = new List<IInputConsumer>();
        return inputConsumers;
    }

    private static List<IInputConsumer> GetGameContext(int playerIndex)
    {
        List<IInputConsumer> inputConsumers = new List<IInputConsumer>();

        GameObject managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
        MainGameManager mgm = managers.GetComponentInChildren<MainGameManager>();
        inputConsumers.Add(mgm);

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

        SpawnPlayersController playerSpawner = managers.GetComponentInChildren<SpawnPlayersController>();
        GameObject player = playerSpawner.GetPlayerByID(playerIndex);
        inputConsumers.Add(player.GetComponent<PlayerInputController>());
        
        return inputConsumers;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
