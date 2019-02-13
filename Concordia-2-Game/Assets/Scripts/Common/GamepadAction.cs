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




        // Internal use only
        public void SetNew(bool newJustPressed, bool newPressed, bool newJustReleased)
        {
            justPressed = newJustPressed;
            pressed = newPressed;
            justReleased = newJustReleased;
        }

        public Gamepad.InputID defaultInputID;
        public Gamepad.InputID currentInputID;
    }
}
