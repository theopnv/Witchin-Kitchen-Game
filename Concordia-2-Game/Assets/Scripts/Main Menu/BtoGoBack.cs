using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.main_menu
{

    public class BtoGoBack : MonoBehaviour
    {

        public Button back;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown("joystick button 1"))
                back.onClick.Invoke();
        }
    }
}

