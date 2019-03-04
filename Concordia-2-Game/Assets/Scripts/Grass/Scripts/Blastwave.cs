using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Blastwave : MonoBehaviour
{
    public AnimationCurve MinRingRadius;
    public AnimationCurve MaxRingRadius;
    public AnimationCurve Intensity;
    public AnimationCurve Scale;

    [Range(0.0f, 1.0f)]
    public float Playback = 0.0f;

    Renderer Renderer;
    MeshFilter Mesh;

    // Start is called before the first frame update
    void Start()
    {
        Renderer = GetComponent<Renderer>();
        Mesh = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        var min = MinRingRadius.Evaluate(Playback);
        var max = MaxRingRadius.Evaluate(Playback);
        var intensity = Intensity.Evaluate(Playback);
        var scale = Scale.Evaluate(Playback);

        transform.localScale = new Vector3(scale, scale, scale);

        Renderer.sharedMaterial.SetFloat("_MinRingRadius", Mesh.sharedMesh.bounds.extents.x * min);
        Renderer.sharedMaterial.SetFloat("_MaxRingRadius", Mesh.sharedMesh.bounds.extents.x * max);
        Renderer.sharedMaterial.SetFloat("_Intensity", intensity);
    }
}
