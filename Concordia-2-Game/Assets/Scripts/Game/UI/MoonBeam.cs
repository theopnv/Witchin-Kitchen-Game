using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MoonBeam : MonoBehaviour
{
    public Vector3 WorldSpaceOffset;
    public Vector3 WorldSpaceRotation;

    protected SpriteRenderer Renderer;

    public AnimationCurve AlphaAnim;
    public float AlphaAnimDuration;

    protected bool Playing = false;
    protected bool PlayForward = true;
    protected float StartTime;

    // Start is called before the first frame update
    void Start()
    {
        Renderer = GetComponent<SpriteRenderer>();

        if (Application.isPlaying)
        {
            var col = Renderer.color;
            col.a = 0.0f;
            Renderer.color = col;
        }
    }

    public void Show()
    {
        Playing = true;
        StartTime = Time.time;
        PlayForward = true;
    }

    public void Hide()
    {
        Playing = true;
        StartTime = Time.time;
        PlayForward = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.transform.position + WorldSpaceOffset;
        transform.rotation = Quaternion.Euler(WorldSpaceRotation);

        if (Playing)
        {
            var elapsed = Time.time - StartTime;
            var progress = Mathf.Clamp01(elapsed / AlphaAnimDuration);

            if (progress >= 1.0f)
                Playing = false;

            if (!PlayForward)
                progress = 1.0f - progress;

            var alpha = AlphaAnim.Evaluate(progress);
            var col = Renderer.color;
            col.a = alpha;
            Renderer.color = col;
        }
    }
}
