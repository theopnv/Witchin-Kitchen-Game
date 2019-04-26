using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaboomTrigger : MonoBehaviour
{
    public GameObject Target;

    [Range(0.0f, 10.0f)]
    public float RepeatWaitDuration = 1.0f;

    private Kaboom kaboom;

    // Start is called before the first frame update
    void Start()
    {
        kaboom = Target.GetComponent<Kaboom>();

        StartCoroutine(WaitThenPunch(RepeatWaitDuration));
    }

    IEnumerator WaitThenPunch(float time)
    {
        yield return new WaitForSeconds(time);
        kaboom.Play();

        StartCoroutine(WaitThenPunch(RepeatWaitDuration));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
