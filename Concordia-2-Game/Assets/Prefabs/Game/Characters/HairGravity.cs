using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairGravity : MonoBehaviour
{
    protected Rigidbody Body;
    public float Gravity = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        Body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Body.AddForce(Vector3.down * Gravity, ForceMode.Acceleration);
    }
}
