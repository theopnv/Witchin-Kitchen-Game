using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var fighters = FindObjectsOfType<Fighter>();
        foreach (var fighter in fighters)
        {
            GlobalFightState.get().AddFighter(fighter);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
