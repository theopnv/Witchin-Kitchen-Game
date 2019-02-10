using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterPunch : MonoBehaviour
{
    private float timerLeft = 0.0f;

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

    private bool punched = false;

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

        // Check input
        if (timerLeft == 0.0f && Input.GetButtonDown("Fire1"))
        {
            timerLeft = GlobalFightState.get().PunchReloadSeconds;

            punched = true;
        }
        else
        {
            punched = false;
        }

        // Visualize the reload timer
        float lerp = GetReloadProgress();
        var renderer = GetComponent<MeshRenderer>();
        renderer.material.color = startColor * lerp + noPunchColor * (1.0f - lerp);
    }

    public float GetReloadProgress()
    {
        return 1.0f - (timerLeft / GlobalFightState.get().PunchReloadSeconds);
    }

    private void OnTriggerStay(Collider other)
    {
        // Ignore parent collision and only consider player capsule tag
        if (punched && other.gameObject != gameObject &&
            other.gameObject.tag == GlobalFightState.PLAYER_CAPSULE_TAG)
        {
            GlobalFightState.get().Punches.Add(new PunchEvent(gameObject, other.gameObject));
        }
    }
}
