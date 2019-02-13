using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// From https://answers.unity.com/questions/614343/how-to-implement-preupdate-function.html

namespace con2
{
    public class EarlyUpdate : MonoBehaviour
    {
        public static event System.Action EarlyUpdateEvent;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (EarlyUpdateEvent != null)
                EarlyUpdateEvent();
        }
    }
}
