using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadMgr : MonoBehaviour
{
    public static GamepadMgr instance;

    public const int NUM_PADS = 2;

    private List<Gamepad> gamepads = new List<Gamepad>();

    void Awake()
    {
        instance = this;

        for (int i = 0; i < NUM_PADS; ++i)
        {
            gamepads.Add(new Gamepad());
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < NUM_PADS; ++i)
        {
            gamepads[i].Poll(i);
        }
    }

    public static Gamepad Pad(int playerIdx)
    {
        return instance.gamepads[playerIdx];
    }
}
