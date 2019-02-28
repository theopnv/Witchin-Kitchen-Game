using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PunchWave : MonoBehaviour
{
    MeshRenderer Renderer;
    float Radius;

    [Range(0, 1)]
    public float Scale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Renderer = GetComponent<MeshRenderer>();
        Radius = Renderer.bounds.extents.x;

        // Disable shadow reception and casting
        Renderer.receiveShadows = false;
        Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    // Update is called once per frame
    void Update()
    {
        Renderer.sharedMaterial.SetFloat("_RampTexInterp", Scale);
        Renderer.sharedMaterial.SetFloat("_Radius", Radius * Scale);
    }
}
