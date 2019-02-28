using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private float Speed = 10f;

    public Transform GroundTarget = null;

    public void SetGroundTarget(Transform target)
    {
        GroundTarget = target;
        transform.LookAt(GroundTarget);
    }

    void Update()
    {
        if (GroundTarget != null)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * Speed);
        }
    }
}
