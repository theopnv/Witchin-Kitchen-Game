using UnityEngine;
using System.Collections;
using con2;

public interface IInputConsumer
{
    bool ConsumeInput(GamepadAction input);
}
