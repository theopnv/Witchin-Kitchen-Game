using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : AHitAllInRange
{
    [SerializeField] private float m_speed = 6.0f;
    [SerializeField] private float m_flyTime = 2.0f;
    private float m_launchTime;
    public GameObject m_launcher;
    private bool m_destroyed = false;

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
        if (!m_destroyed)
        {
            Hit();
        }
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
        m_destroyed = true;

        Rigidbody body = GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezeAll;

        Collider[] colliders = transform.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        Renderer[] meshes = transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer mesh in meshes)
        {
            mesh.enabled = false;
        }

        var kaboom = gameObject.GetComponentInChildren<Kaboom>();
        kaboom.GetComponent<Renderer>().enabled = true;
        kaboom.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
        kaboom.Play();

        yield return new WaitForSeconds(kaboom.AnimTime);
        GameObject.Destroy(this.gameObject);
    }
}
