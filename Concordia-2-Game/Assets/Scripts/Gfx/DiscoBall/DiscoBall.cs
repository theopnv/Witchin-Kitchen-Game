using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoBall : MonoBehaviour
{
    public GameObject FollowTarget;

    public Vector3 FollowOffset = new Vector3(0.0f, 0.6f, 1.0f);

    [Range(0.0f, 1.0f)]
    public float FollowLerp = 0.1f;

    [Range(0.0f, 10.0f)]
    public float OscillationLoopSeconds = 2.0f;

    public AnimationCurve OscillationX;
    public AnimationCurve OscillationY;
    public AnimationCurve OscillationZ;

    public float RotationSpeedX;
    private float CurRotationX = 0.0f;
    public float RotationSpeedY;
    private float CurRotationY = 0.0f;

    [Range(0.0f, 10.0f)]
    public float OscillationScaleX = 1.0f;

    [Range(0.0f, 10.0f)]
    public float OscillationScaleY = 1.0f;

    [Range(0.0f, 10.0f)]
    public float OscillationScaleZ = 1.0f;

    private Renderer TargetRenderer;

    // Start is called before the first frame update
    void Start()
    {
        TargetRenderer = FollowTarget.GetComponent<Renderer>();

        // Disable shadow casting - it's distracting when it lands on the player
        TargetRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    // Update is called once per frame
    void Update()
    {
        var curveProgress = (Time.time % OscillationLoopSeconds) / OscillationLoopSeconds;
        var oscX = OscillationX.Evaluate(curveProgress) * OscillationScaleX;
        var oscY = OscillationY.Evaluate(curveProgress) * OscillationScaleY;
        var oscZ = OscillationZ.Evaluate(curveProgress) * OscillationScaleZ;

        var targetPos = FollowTarget.transform.position + FollowOffset;
        targetPos.y += TargetRenderer.bounds.extents.y;

        var curPos = transform.position;
        var newPos = curPos + (targetPos - curPos) * FollowLerp;
        newPos += new Vector3(oscX, oscY, oscZ);

        transform.position = newPos;


        CurRotationX += RotationSpeedX * Time.deltaTime;
        CurRotationY += RotationSpeedY * Time.deltaTime;
        transform.rotation = Quaternion.Euler(CurRotationX, CurRotationY, 0.0f);
    }
}
