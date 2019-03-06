using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    public Color OutlineColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    [Range(0.0f, 5.0f)]
    public float OutlineWidth = 0.15f;

    // We don't need to touch this unless we start seeing artifacts (incomplete outlines)
    [Range(0.0f, 180.0f)]
    private float SwitchShaderOnAngle = 89.0f;

    private Material OutlineMaterial;
    private string ShaderName = "Custom/Outline";
    private Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        OutlineMaterial = new Material(Shader.Find(ShaderName));
 
        mesh = GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        OutlineMaterial.SetColor("_OutlineColor", OutlineColor);
        OutlineMaterial.SetFloat("_OutlineWidth", OutlineWidth);
        OutlineMaterial.SetFloat("_Angle", SwitchShaderOnAngle);

        Graphics.DrawMesh(mesh, transform.localToWorldMatrix, OutlineMaterial, 0);
    }
}
