using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBounceTrigger : MonoBehaviour
{
    public GameObject Target;

    [Range(0.0f, 10.0f)]
    public float RepeatWaitDuration = 1.0f;

    private SpawnBounce bounce;

    // Start is called before the first frame update
    void Start()
    {
        bounce = Target.GetComponent<SpawnBounce>();

        StartCoroutine(WaitThenBounce(RepeatWaitDuration));
    }

    IEnumerator WaitThenBounce(float time)
    {
        yield return new WaitForSeconds(time);
        bounce.Play();

        StartCoroutine(WaitThenShrink(RepeatWaitDuration));
    }

    IEnumerator WaitThenShrink(float time)
    {
        yield return new WaitForSeconds(time);
        bounce.ScaleObject.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

        StartCoroutine(WaitThenBounce(RepeatWaitDuration));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
