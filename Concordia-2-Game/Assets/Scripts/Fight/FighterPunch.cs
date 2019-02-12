using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterPunch : MonoBehaviour
{
    private bool punchRequested = false;
    private bool punchOverlapping = false;
    private GameObject punchTarget;

    private Color startColor;
    private Color noPunchColor = new Color(1.0f, 1.0f, 1.0f);

    public class PunchEvent
    {
        public GameObject Source;
        public Vector3 SourcePosition;

        public GameObject Target;
        public Vector3 TargetPosition;

        public PunchEvent(GameObject source, GameObject target)
        {
            Source = source;
            Target = target;

            SourcePosition = Source.transform.position;
            TargetPosition = Target.transform.position;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startColor = GetComponent<MeshRenderer>().material.color;
    }
    
    public void SetReloadProgress(float progress)
    {
        // Visualize the reload timer
        float lerp = progress;
        var renderer = GetComponent<MeshRenderer>();
        renderer.material.color = startColor * lerp + noPunchColor * (1.0f - lerp);
    }

    public void RequestPunch()
    {
        punchRequested = true;
    }

    public void CancelPunch()
    {
        punchRequested = false;
    }

    private bool isTarget(Collider other)
    {
        // Ignore parent collision and only consider player capsule tag
        return other.gameObject != gameObject &&
               other.gameObject.tag == GlobalFightState.PLAYER_CAPSULE_TAG;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTarget(other))
        {
            punchOverlapping = true;
            punchTarget = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isTarget(other))
        {
            punchOverlapping = false;
            punchTarget = null;
        }
    }

    private void FixedUpdate()
    {
        if (punchOverlapping && punchRequested)
        {
            punchRequested = false;
            GlobalFightState.get().Punches.Add(new PunchEvent(gameObject, punchTarget));
        }
    }
}
