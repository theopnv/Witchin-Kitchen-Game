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

    public float Duration = 2.0f;

    private Renderer Renderer;
    private MeshFilter Mesh;
    private float StartTime;
    private bool Playing = false;

    // Start is called before the first frame update
    void Start()
    {
        Renderer = GetComponent<Renderer>();
        Mesh = GetComponent<MeshFilter>();
    }

    public void Play()
    {
        StartTime = Time.time;
        Playing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Playing)
        {
            Renderer.material.SetFloat("_Intensity", 0.0f);
            return;
        }


        // Playback
        var curTime = Time.time;
        var elapsed = curTime - StartTime;

        if (elapsed > Duration) // Done animation
            Playing = false;
        
        Playback = elapsed / Duration;
        Playback = Mathf.Clamp01(Playback);


        var min = MinRingRadius.Evaluate(Playback);
        var max = MaxRingRadius.Evaluate(Playback);
        var intensity = Intensity.Evaluate(Playback);
        var scale = Scale.Evaluate(Playback);

        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(scale, scale, scale);

        Renderer.material.SetFloat("_MinRingRadius", Mesh.sharedMesh.bounds.extents.x * min);
        Renderer.material.SetFloat("_MaxRingRadius", Mesh.sharedMesh.bounds.extents.x * max);
        Renderer.material.SetFloat("_Intensity", intensity);
    }

    private void OnDestroy()
    {
        Destroy(Renderer.material);
    }
}
