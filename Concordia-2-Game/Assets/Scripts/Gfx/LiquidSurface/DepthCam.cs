using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DepthCam : MonoBehaviour
{
    void Awake()
    {
        var cam = Camera.main;
        SetupCam(cam);
    }

    // Start is called before the first frame update
    void Start()
    {
        var cam = Camera.main;
        SetupCam(cam);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetupCam(Camera cam)
    {
        cam.depthTextureMode = DepthTextureMode.Depth;
    }
}
