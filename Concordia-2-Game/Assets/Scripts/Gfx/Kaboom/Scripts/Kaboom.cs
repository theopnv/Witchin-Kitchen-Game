using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Kaboom : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float Playback = 0.0f;

    public float AnimTime = 1.0f;

    public AnimationCurve Scale;

    [Range(0.0f, 10.0f)]
    public float ScaleMultiplier = 1.0f;

    public AnimationCurve SliceAmount;

    public AnimationCurve SpinSpeed;

    public AnimationCurve BurnSize;

    public AnimationCurve EmissionAmount;

    public AnimationCurve HaloScale;

    [Range(0.0f, 5.0f)]
    public float EmissionScale;

    private Renderer OwnRenderer;
    private Renderer HaloRenderer;
    private GameObject Halo;

    private float StartTime;
    private bool Playing = false;

    // Start is called before the first frame update
    void Awake()
    {
        Halo = transform.GetChild(0).gameObject;

        OwnRenderer = GetComponent<Renderer>();
        OwnRenderer.sharedMaterial = new Material(OwnRenderer.sharedMaterial);
        HaloRenderer = Halo.GetComponent<Renderer>();

        // Hide at start
        transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        Halo.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);

        // Prevent shadow casting and reception
        OwnRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        OwnRenderer.receiveShadows = false;
        HaloRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        HaloRenderer.receiveShadows = false;
    }

    public IEnumerator Activate()
    {
        if (!Playing)
        {
            OwnRenderer.enabled = true;
            HaloRenderer.enabled = true;
            Play();

            yield return new WaitForSeconds(AnimTime);
            GameObject.Destroy(transform.parent.gameObject);
        }
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
            return;

        // Playback
        var curTime = Time.time;
        var elapsed = curTime - StartTime;

        Playback = elapsed / AnimTime;
        Playback = Mathf.Clamp01(Playback);


        var scale = Scale.Evaluate(Playback) * ScaleMultiplier;
        var sliceAmount = SliceAmount.Evaluate(Playback);
        var spinSpeed = SpinSpeed.Evaluate(Playback);
        var burnSize = BurnSize.Evaluate(Playback);
        var emissionAmount = EmissionAmount.Evaluate(Playback) * EmissionScale;

        transform.localScale = new Vector3(scale, scale, scale);
        OwnRenderer.sharedMaterial.SetFloat("_SliceAmount", sliceAmount);
        OwnRenderer.sharedMaterial.SetFloat("_SpinSpeed", spinSpeed);
        OwnRenderer.sharedMaterial.SetFloat("_BurnSize", burnSize);
        OwnRenderer.sharedMaterial.SetFloat("_EmissionAmount", emissionAmount);


        var haloScale = HaloScale.Evaluate(Playback);
        Halo.transform.localScale = new Vector3(haloScale, haloScale, haloScale);
    }
}
