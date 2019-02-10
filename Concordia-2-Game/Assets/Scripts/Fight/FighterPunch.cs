using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterPunch : MonoBehaviour
{
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
        
    }

    // Update is called once per frame
    void Update()
    {
        punched = Input.GetButtonDown("Fire1");
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
