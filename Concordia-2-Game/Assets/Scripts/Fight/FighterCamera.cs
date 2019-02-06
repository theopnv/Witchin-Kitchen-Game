using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterCamera : MonoBehaviour
{
    public Fighter Target;

    [Range(0.0f, 100.0f)] 
    public float Distance;

    [Range(0.0f, 1.0f)]
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;

    private Vector3 camPos;

    // Start is called before the first frame update
    void Start()
    {
        // Snap at start to avoid interpolation
        transform.position = getNewCamTargetPos();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        camPos = getNewCamTargetPos();
        transform.position = Vector3.SmoothDamp(transform.position, camPos, ref velocity, smoothTime);
    }

    private Vector3 getNewCamTargetPos()
    {
        var displacement = transform.forward * Distance;
        return Target.transform.position - displacement;
    }
}
