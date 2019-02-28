using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : HitAllInRange
{
    [SerializeField] private float m_speed = 6.0f;
    [SerializeField] private float m_flyTime = 2.0f;
    private float m_launchTime;
    public GameObject m_launcher;

    protected override void OnStart()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * m_speed;
        m_launchTime = Time.time;
        StartCoroutine(ProjectileDespawn());
    }

    private IEnumerator ProjectileDespawn()
    {
        yield return new WaitForSeconds(m_flyTime);
        Hit();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != m_launcher)
        {
            Hit();
        }
    }

    protected override Vector3 ModulateHitVector(Vector3 hitVector)
    {
        return hitVector;
    }

    protected override void AfterHitting()
    {
        StartCoroutine(Explode());
    }
    
    private IEnumerator Explode()
    {
        MeshRenderer[] meshes = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.enabled = true;
        }
        yield return new WaitForSeconds(0.1f);
        GameObject.Destroy(this.gameObject);
    }
}
