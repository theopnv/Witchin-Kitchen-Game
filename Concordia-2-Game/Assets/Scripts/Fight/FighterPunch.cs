using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterPunch : MonoBehaviour
{
    private float timerLeft = 0.0f;

    private bool punchRequested = false;
    private bool punchAccepted = false;

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

    // Update is called once per frame
    void Update()
    {
        // Update reload timer
        timerLeft = Mathf.Max(0.0f, timerLeft - Time.deltaTime);

        // Visualize the reload timer
        float lerp = GetReloadProgress();
        var renderer = GetComponent<MeshRenderer>();
        renderer.material.color = startColor * lerp + noPunchColor * (1.0f - lerp);
    }

    private void LateUpdate()
    {
        // Check timer
        if (punchRequested && timerLeft == 0.0f)
        {
            timerLeft = GlobalFightState.get().PunchReloadSeconds;
            punchAccepted = true;
        }
        else
        {
            punchAccepted = false;
        }

        punchRequested = false;
    }

    public void RequestPunch()
    {
        punchRequested = true;
    }

    public float GetReloadProgress()
    {
        return 1.0f - (timerLeft / GlobalFightState.get().PunchReloadSeconds);
    }

    private void OnTriggerStay(Collider other)
    {
        // Ignore parent collision and only consider player capsule tag
        if (punchAccepted && other.gameObject != gameObject &&
            other.gameObject.tag == GlobalFightState.PLAYER_CAPSULE_TAG)
        {
            GlobalFightState.get().Punches.Add(new PunchEvent(gameObject, other.gameObject));
        }
    }
}
