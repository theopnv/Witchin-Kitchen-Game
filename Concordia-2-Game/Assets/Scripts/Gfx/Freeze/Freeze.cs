using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    public GameObject Target;

    public GameObject Ground;

    public Material FrozenMaterial;

    [Range(0.0f, 1.0f)]
    public float Thickness = 0.25f;

    public float AnimTime = 1.5f;

    public AnimationCurve ScaleAnimation;

    protected GameObject Clone;
    protected GameObject GroundClone;

    protected List<GameObject> Children = new List<GameObject>();
    protected List<Vector3> OriginalScales = new List<Vector3>();
    protected List<Vector3> TargetScales = new List<Vector3>();
    protected Vector3 GroundCloneOriginalPos;

    protected float StartTime;
    protected bool Freezing;
    protected bool Playing = false;
    protected float Playback = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Clone = Instantiate(Target);

        CollectChildren(Clone);

        foreach (var child in Children)
        {
            var childRenderer = child.GetComponent<Renderer>();
            childRenderer.sharedMaterial = FrozenMaterial;

            var curScale = child.transform.localScale;
            var extents = childRenderer.bounds.extents;

            var newScaleX = curScale.x * (extents.x + Thickness) / extents.x;
            var newScaleY = curScale.y * (extents.y + Thickness) / extents.y;
            var newScaleZ = curScale.z * (extents.z + Thickness) / extents.z;
            var newScale = new Vector3(newScaleX, newScaleY, newScaleZ);

            OriginalScales.Add(curScale);
            TargetScales.Add(newScale);
        }

        GroundClone = Instantiate(Ground);
        GroundClone.transform.SetParent(Clone.transform, false);
        GroundClone.GetComponent<Renderer>().enabled = false;
        var groundCloneBody = GroundClone.AddComponent<Rigidbody>();
        groundCloneBody.useGravity = false;
        groundCloneBody.isKinematic = true;
        GroundCloneOriginalPos = GroundClone.transform.position;

        // Hide at start
        Clone.SetActive(false);
    }

    void CollectChildren(GameObject obj)
    {
        for (var i = 0; i < obj.transform.childCount; ++i)
        {
            var child = obj.transform.GetChild(i);

            Children.Add(child.gameObject);
            CollectChildren(child.gameObject);
        }
    }

    public void PlayFreeze()
    {
        StartTime = Time.time;
        Playing = true;

        Freezing = true;
    }

    public void PlayThaw()
    {
        StartTime = Time.time;
        Playing = true;

        Freezing = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Playback
        if (Playing)
        {
            Clone.SetActive(true);

            var curTime = Time.time;
            var elapsed = curTime - StartTime;

            if (elapsed > AnimTime) // Done animation
            {
                Playing = false;

                if (!Freezing)
                {
                    Clone.SetActive(false);
                }
            }

            if (Freezing)
                Playback = elapsed / AnimTime;
            else
                Playback = 1.0f - elapsed / AnimTime;

            Playback = Mathf.Clamp01(Playback);


            var scaleAnim = ScaleAnimation.Evaluate(Playback);

            for (var i = 0; i < Children.Count; ++i)
            {
                var child = Children[i];
                var originalScale = OriginalScales[i];
                var targetScale = TargetScales[i];

                child.transform.localScale = originalScale + (targetScale - originalScale) * scaleAnim;
            }

            var pos = GroundClone.transform.position;
            pos.y = GroundCloneOriginalPos.y + Thickness * scaleAnim;
            GroundClone.transform.position = pos;
        }
    }
}
