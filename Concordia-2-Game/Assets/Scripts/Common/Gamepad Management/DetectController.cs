using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace con2
{
    /// <summary>
    /// Helper to know which controllers are connected or disconnected.
    /// </summary>
    public class DetectController : MonoBehaviour
    {
        /// <summary>
        /// An array representing the state of the controllers: true = connected, false = disconnected
        /// </summary>
        [HideInInspector]
        public bool[] ControllersState;

        /// <summary>
        /// Called when a controller was connected.
        /// The parameter is the index of the controller.
        /// </summary>
        public Action<int> OnConnected;

        /// <summary>
        /// Called when a controller was disconnected
        /// The parameter is the index of the controller.
        /// </summary>
        public Action<int> OnDisconnected;

        /// <summary>
        /// Time it takes between 2 controllers state checks. 
        /// </summary>
        private const float _refreshFrequence = 1.5f;

        #region Unity API

        // Start is called before the first frame update
        private void Start()
        {
            ControllersState = new bool[4];
            InvokeRepeating("UpdateControllersState", 0, _refreshFrequence);
        }

        #endregion

        private void UpdateControllersState()
        {
            var temp = Input.GetJoystickNames();

            // Check whether array contains anything
            if (temp.Length > 0)
            {
                // Iterate over every element
                for (var i = 0; i < temp.Length; ++i)
                {
                    // Check if the string is empty or not
                    if (!string.IsNullOrEmpty(temp[i]))
                    {
                        // Not empty, controller temp[i] is connected
                        if (!ControllersState[i])
                        {
                            Debug.Log("Controller " + i + " was connected");
                            OnConnected?.Invoke(i);
                        }
                        ControllersState[i] = true;
                    }
                    else
                    {
                        // If it is empty, controller i is disconnected
                        // where i indicates the controller number
                        if (ControllersState[i])
                        {
                            Debug.Log("Controller " + i + " was disconnected");
                            OnDisconnected?.Invoke(i);
                        }
                        ControllersState[i] = false;
                    }
                }
            }
        }
    }
}
