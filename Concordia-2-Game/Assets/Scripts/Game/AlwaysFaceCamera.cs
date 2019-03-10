using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AlwaysFaceCamera : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.LookAt(Camera.main.transform);
        var prompt = GetComponentInChildren<Text>();
        prompt.transform.localEulerAngles = new Vector3(0, 180, 0);
    }
}

