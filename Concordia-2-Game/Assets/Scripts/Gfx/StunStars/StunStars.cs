using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunStars : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float Playback = 0.0f;

    public float AnimTime = 2.0f;

    public GameObject FollowTarget;

    public Vector3 FollowOffset = new Vector3(0.0f, 0.33f, 0.0f);
    
    public float Scale = 1.0f;
    
    public AnimationCurve ScaleAnim;

    public float RotationScaleX = 180.0f;
    private float RotationX = 0.0f;
    
    public AnimationCurve RotationSpeedX;

    public float CirclingSpeed = 20.0f;
    public AnimationCurve CirclingSpeedAnim;

    public float CirclingIntensity = 0.3f;
    public AnimationCurve CirclingIntensityAnim;

    private Renderer OwnRenderer;
    private Renderer TargetRenderer;
    private float StartTime;
    private float Duration;
    private bool Playing = false;

    // Start is called before the first frame update
    void Start()
    {
        OwnRenderer = GetComponent<Renderer>();
        TargetRenderer = FollowTarget.GetComponent<Renderer>();

        // Hide at start
        transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);

        // Prevent shadow casting and reception
        OwnRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        OwnRenderer.receiveShadows = false;
    }

    public void Play(float duration)
    {
        StartTime = Time.time;
        Duration = Mathf.Max(duration, AnimTime);
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
            return;

        // Playback
        var curTime = Time.time;
        var elapsed = curTime - StartTime;
        
        if (elapsed > Duration) // Done animation
            Playing = false;

        if (elapsed <= AnimTime / 2.0f)
            Playback = elapsed / AnimTime;
        else if (elapsed > Duration - AnimTime / 2.0f)
            Playback = (elapsed - (Duration - AnimTime / 2.0f)) / AnimTime + 0.5f;
        else
            Playback = 0.5f;

        Playback = Mathf.Clamp01(Playback);

        // Position
        var targetPos = FollowTarget.transform.position;
        targetPos.y += TargetRenderer.bounds.extents.y;
        targetPos += FollowOffset;

        transform.position = targetPos;


        // Scale
        var scale = ScaleAnim.Evaluate(Playback) * Scale;
        transform.localScale = new Vector3(scale, scale, scale);


        // Circling
        var cSpeed = CirclingSpeedAnim.Evaluate(Playback) * CirclingSpeed;
        var cIntensity = CirclingIntensityAnim.Evaluate(Playback) * CirclingIntensity;

        var cx = Mathf.Cos((Time.time % AnimTime) / AnimTime * cSpeed);
        var cz = Mathf.Sin((Time.time % AnimTime) / AnimTime * cSpeed);
        var rotationAxis = new Vector3(cx * cIntensity, 1.0f, cz * cIntensity);
        rotationAxis.Normalize();


        // Rotation
        var rotSpeedX = RotationSpeedX.Evaluate(Playback) * RotationScaleX;

        RotationX += rotSpeedX * Time.deltaTime;
        var orient = Quaternion.FromToRotation(Vector3.up, rotationAxis);
        transform.rotation = Quaternion.AngleAxis(RotationX, rotationAxis) * orient;
    }
}
