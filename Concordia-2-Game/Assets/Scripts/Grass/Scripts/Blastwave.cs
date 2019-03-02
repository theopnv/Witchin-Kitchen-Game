using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blastwave : MonoBehaviour
{
    public AnimationCurve MinRingRadius;
    public AnimationCurve MaxRingRadius;
    public AnimationCurve Intensity;
    public AnimationCurve Scale;

    [Range(0.0f, 1.0f)]
    public float Playback = 0.0f;

    MeshRenderer renderer;
    MeshFilter mesh;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        mesh = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        var min = MinRingRadius.Evaluate(Playback);
        var max = MaxRingRadius.Evaluate(Playback);
        var intensity = Intensity.Evaluate(Playback);
        var scale = Scale.Evaluate(Playback);

        transform.localScale = new Vector3(scale, scale, scale);

        renderer.material.SetFloat("_MinRingRadius", mesh.mesh.bounds.extents.x * min);
        renderer.material.SetFloat("_MaxRingRadius", mesh.mesh.bounds.extents.x * max);
        renderer.material.SetFloat("_Intensity", intensity);
    }
}
