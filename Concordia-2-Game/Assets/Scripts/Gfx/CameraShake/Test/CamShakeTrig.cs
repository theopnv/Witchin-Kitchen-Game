using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShakeTrig : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float BigShakeAmount = 1.0f;

    [Range(0.0f, 10.0f)]
    public float BigShakeDuration = 1.0f;

    [Range(0.0f, 10.0f)]
    public float SmallShakeAmount = 1.0f;

    [Range(0.0f, 10.0f)]
    public float SmallShakeDuration = 1.0f;

    protected CamShake Shake;

    // Start is called before the first frame update
    void Start()
    {
        Shake = Camera.main.GetComponent<CamShake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Shake.ShakeCamera(BigShakeAmount, BigShakeDuration);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Shake.ShakeCamera(SmallShakeAmount, SmallShakeDuration);
        }
    }
}
