using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PunchWave : MonoBehaviour
{
    private MeshRenderer Renderer;
    private float Radius;
    private float StartTime;
    private bool Playing = false;

    [Range(0, 1)]
    public float Playback = 0.0f;

    public float Duration = 0.5f;

    public AnimationCurve Scale;

    public AnimationCurve Alpha;

    // Start is called before the first frame update
    void Start()
    {
        Renderer = GetComponent<MeshRenderer>();
        Radius = Renderer.bounds.extents.x;

        // Disable shadow reception and casting
        Renderer.receiveShadows = false;
        Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public void Play()
    {
        StartTime = Time.time;
        Playing = true;
    }

    public bool IsPlaying()
    {
        return Playing;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Playing)
        {
            Renderer.sharedMaterial.SetFloat("_Radius", 0.0f);
            return;
        }

        // Playback
        var curTime = Time.time;
        var elapsed = curTime - StartTime;

        if (elapsed > Duration) // Done animation
            Playing = false;
        
        Playback = elapsed / Duration;
        Playback = Mathf.Clamp01(Playback);


        var scale = Scale.Evaluate(Playback);
        var alpha = Alpha.Evaluate(Playback);

        Renderer.sharedMaterial.SetFloat("_RampTexInterp", scale);
        Renderer.sharedMaterial.SetFloat("_Radius", Radius * scale);
        Renderer.sharedMaterial.SetFloat("_Alpha", alpha);
    }
}
