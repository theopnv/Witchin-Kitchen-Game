using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetHeight : MonoBehaviour
{
    float GlowMeshHeight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var renderer = GetComponent<Renderer>();
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        GlowMeshHeight = mesh.bounds.size.y;
        renderer.sharedMaterial.SetFloat("_GlowHeight", GlowMeshHeight);
    }
}
