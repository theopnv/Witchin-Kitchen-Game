using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AlwaysFaceCamera : MonoBehaviour
{
    private Transform m_towardsCamera;

    private void Start()
    {
        m_towardsCamera = Camera.main.transform;
    }

    void FixedUpdate()
    {
        transform.LookAt(m_towardsCamera);
        var prompt = transform.Find("Canvas/Backdrop");
        prompt.transform.localEulerAngles = new Vector3(0, 180, 0);
    }
}

