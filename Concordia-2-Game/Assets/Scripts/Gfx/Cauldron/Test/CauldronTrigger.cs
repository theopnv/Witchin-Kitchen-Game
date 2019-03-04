using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronTrigger : MonoBehaviour
{
    public GameObject Target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var c = Target.GetComponent<Cauldron>();

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            c.StartCooking();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            c.StopCooking();
        }
    }
}
