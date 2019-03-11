using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Displacer : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float Intensity = 1.0f;

    public float MaxRaycastDistance = 20.0f;

    public float HeightFalloffScale = 0.5f;

    public AnimationCurve HeightFalloff;

    private Renderer Renderer;
    private MeshFilter Mesh;

    // Start is called before the first frame update
    void Start()
    {
        Renderer = GetComponent<Renderer>();
        Mesh = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.right);

        Renderer.sharedMaterial.SetFloat("_Radius", Mesh.sharedMesh.bounds.extents.x);

        // Height falloff
        var internalIntensity = 0.0f;
        RaycastHit hitInfo;
        int layerMask = ~Grass.GRASS_SURFACE_LAYER_MASK; // Ignore all but grass surfaces
        var hit = Physics.Raycast(transform.position, Vector3.down, out hitInfo, MaxRaycastDistance, layerMask);

        if (hit)
        {
            var falloffProgress = Mathf.Clamp01(hitInfo.distance / HeightFalloffScale);
            var falloff = HeightFalloff.Evaluate(falloffProgress);
            internalIntensity = falloff * Intensity;
        }

        Renderer.sharedMaterial.SetFloat("_Intensity", internalIntensity);
    }
}
