using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public class GamepadAction
    {
        public enum ID
        {
            PUNCH,
            INTERACT,
            MAX_ID,
        }

        public bool JustPressed {
            get { return justPressed; }
        }
        public bool Pressed {
            get { return pressed; }
        }
        public bool JustReleased {
            get { return justReleased; }
        }

        private bool justPressed;
        private bool pressed;
        private bool justReleased;

        public event System.Action JustPressedEvent;
        public event System.Action PressedEvent;
        public event System.Action JustReleasedEvent;




        // Internal use only
        public void SetNew(bool newJustPressed, bool newPressed, bool newJustReleased)
        {
            justPressed = newJustPressed;
            pressed = newPressed;
            justReleased = newJustReleased;

            if (justPressed && JustPressedEvent != null)
            {
                JustPressedEvent();
            }
            if (pressed && PressedEvent != null)
            {
                PressedEvent();
            }
            if (justReleased && JustReleasedEvent != null)
            {
                JustReleasedEvent();
            }
        }

        public Gamepad.InputID defaultInputID;
        public Gamepad.InputID currentInputID;
    }
}
