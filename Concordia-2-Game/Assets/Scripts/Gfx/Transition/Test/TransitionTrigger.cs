using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public class TransitionTrigger : MonoBehaviour
    {
        public Transition Target;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(Target.PlayIn());
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(Target.PlayOut());
            }
        }
    }
}
