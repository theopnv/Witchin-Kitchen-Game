using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin2Win : MonoBehaviour 
{
    void Update()
    {
        transform.Rotate(new Vector3(0, 280, 0) * Time.deltaTime);
    }
}

