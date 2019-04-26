using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float Speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            transform.position += Vector3.left * Speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.D))
            transform.position += Vector3.right * Speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
            transform.position += Vector3.forward * Speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.S))
            transform.position += Vector3.back * Speed * Time.deltaTime;
    }
}
