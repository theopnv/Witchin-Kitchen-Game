using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Displacer : MonoBehaviour
{
    MeshRenderer renderer;
    MeshFilter mesh;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        mesh = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        renderer.material.SetFloat("_Radius", mesh.mesh.bounds.extents.x);
    }
}
