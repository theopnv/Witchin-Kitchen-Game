using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheapInsurancePolicy : MonoBehaviour
{
    // DON'T FORGET TO CHANGE IF BOUNDS CHANGE FOR SOME REASON!
    // In world position (X=left/right, Y=bottom/top, Z=front/back):
    static public Vector3 Min = new Vector3(-22, -40, -16);
    static public Vector3 Max = new Vector3(22, 40, 16);

    // The teleport point. Should be above the camera's view but below the top bounding wall.
    static public Vector3 RespawnPoint = new Vector3(0.0f, 20.0f, 0.0f);

    protected Rigidbody Body;
    
    void Awake()
    {
        Body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;

        if (pos.x > Min.x && pos.y > Min.y && pos.z > Min.z && pos.x < Max.x && pos.y < Max.y && pos.z < Max.z)
        {
            // All good
        }
        else
        {
            // Insurance coverage kicks in

            transform.position = RespawnPoint;      // Teleport inside the arena
            Body.velocity = Vector3.zero;           // Cancel whatever caused this in the first place
        }
    }
}
