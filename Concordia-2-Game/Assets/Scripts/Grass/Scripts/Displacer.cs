using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Displacer : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float Intensity = 1.0f;

    public float MaxRaycastDistance = 20.0f;

    public float HeightFalloffScale = 0.5f;

    public AnimationCurve HeightFalloff;

    private Renderer Renderer;
    private MeshFilter Mesh;

    private Vector3 StartScale;

    // Start is called before the first frame update
    void Start()
    {
        Renderer = GetComponent<Renderer>();
        Mesh = GetComponent<MeshFilter>();
        StartScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.right);
        transform.localScale = StartScale * con2.game.GrassGrowthEvent.DisplacementStrengthMultiplier;

        Renderer.material.SetFloat("_Radius", Mesh.sharedMesh.bounds.extents.x);

        // Height falloff
        var internalIntensity = 0.0f;
        RaycastHit hitInfo;
        int layerMask = 1 << Grass.GRASS_SURFACE_LAYER_MASK; // Ignore all but grass surfaces
        var hit = Physics.Raycast(transform.position, Vector3.down, out hitInfo, MaxRaycastDistance, layerMask);

        if (hit)
        {
            var falloffProgress = Mathf.Clamp01(hitInfo.distance / HeightFalloffScale / con2.game.GrassGrowthEvent.DisplacementStrengthMultiplier);
            var falloff = HeightFalloff.Evaluate(falloffProgress);
            internalIntensity = falloff * Intensity;
        }

        Renderer.material.SetFloat("_Intensity", internalIntensity);
    }

    private void OnDestroy()
    {
        Destroy(Renderer.material);
    }
}
