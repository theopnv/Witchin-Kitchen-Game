using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    public float Duration = 1.5f;

    [Range(0.0f, 1.0f)]
    public float Playback = 0.0f;

    public AnimationCurve GlowIntensity;

    private const string CAULDRON_BODY_NAME = "Cauldron";
    private GameObject CauldronBody;
    private Renderer CauldronBodyRenderer;

    private float StartTime;
    private bool Growing;
    private bool Playing = false;

    // Start is called before the first frame update
    void Start()
    {
        CauldronBody = transform.Find(CAULDRON_BODY_NAME).gameObject;
        CauldronBodyRenderer = CauldronBody.GetComponent<Renderer>();

        Playback = 0.0f;
    }

    public void StartCooking()
    {
        StartTime = Time.time - Playback * Duration;

        Playing = true;
        Growing = true;
    }

    public void StopCooking()
    {
        StartTime = Time.time - (1.0f - Playback) * Duration;

        Playing = true;
        Growing = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Playback
        if (Playing)
        {
            var curTime = Time.time;
            var elapsed = curTime - StartTime;

            if (elapsed > Duration) // Done animation
                Playing = false;
            
            if (Growing)
                Playback = elapsed / Duration;
            else
                Playback = 1.0f - elapsed / Duration;

            Playback = Mathf.Clamp01(Playback);
        }


        var glowIntensity = GlowIntensity.Evaluate(Playback);
        CauldronBodyRenderer.material.SetFloat("_GlowIntensity", glowIntensity);
    }
}
