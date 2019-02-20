using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampWithinArenaBounds : MonoBehaviour
{
    static float arenaBaseX, arenaBaseZ;
    static bool m_roundArena = true;
    private Rigidbody m_rb;

    // Start is called before the first frame update
    void Start()
    {
        if (arenaBaseX == 0 || arenaBaseZ == 0)
        {
            MeshCollider arenaBase = GameObject.FindGameObjectWithTag(Tags.ARENA_BASE).GetComponent<MeshCollider>();
            arenaBaseX = arenaBase.bounds.size.x / 2.2f;    //Add the .2 so the walls are slightly in from the visible edge
            arenaBaseZ = arenaBase.bounds.size.z / 2.2f;
            m_roundArena = arenaBaseX - arenaBaseZ < 1;
        }
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_roundArena)
        {
            Vector3 position = transform.position;
            if (position.magnitude > arenaBaseZ)
            {
                transform.position = Vector3.ClampMagnitude(position, arenaBaseX);
                m_rb.velocity *= -3;
            }
        }
        else
        {
            if (Math.Abs(transform.position.x) > arenaBaseX
                || Math.Abs(transform.position.z) > arenaBaseZ)
            {
                m_rb.velocity = Vector3.zero;
                transform.position = new Vector3(
    Mathf.Clamp(transform.position.x, -arenaBaseX, arenaBaseX),
    transform.position.y,
    Mathf.Clamp(transform.position.z, -arenaBaseZ, arenaBaseZ));
            }
        }
    }
}
