using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Displacer : MonoBehaviour
{
    public Material material;
    public MeshFilter quadMesh;

    [Range(0.0f, 360.0f)]
    public float angle = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    //public Material material;
    //private Mesh quadMesh;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    quadMesh = new Mesh();

    //    var vertices = new Vector3[4];

    //    vertices[0] = new Vector3(-0.5f, -0.5f, 0);
    //    vertices[1] = new Vector3(0.5f, -0.5f, 0);
    //    vertices[2] = new Vector3(-0.5f, 0.5f, 0);
    //    vertices[3] = new Vector3(0.5f, 0.5f, 0);

    //    quadMesh.vertices = vertices;

    //    var tri = new int[6];

    //    tri[0] = 0;
    //    tri[1] = 2;
    //    tri[2] = 1;

    //    tri[3] = 2;
    //    tri[4] = 3;
    //    tri[5] = 1;

    //    quadMesh.triangles = tri;

    //    var normals = new Vector3[4];

    //    normals[0] = -Vector3.forward;
    //    normals[1] = -Vector3.forward;
    //    normals[2] = -Vector3.forward;
    //    normals[3] = -Vector3.forward;

    //    quadMesh.normals = normals;
    //}

    // Update is called once per frame
    void Update()
    {
        var map = DisplacementMap.Get();
        var rt = map.rt;


        var pos = new Vector3();
        pos.x = (transform.position.x - map.targetBounds.center.x) * map.pixelsPerGameUnit;
        pos.y = (transform.position.z - map.targetBounds.center.z) * map.pixelsPerGameUnit;


        var scale = transform.localScale * map.pixelsPerGameUnit;


        // Start rendering to map
        GL.PushMatrix();
        GL.LoadPixelMatrix(-rt.width / 2.0f, rt.width / 2.0f, -rt.height / 2.0f, rt.height / 2.0f);
        material.SetPass(0);
        RenderTexture oldRT = RenderTexture.active;
        Graphics.SetRenderTarget(rt);
        GL.Clear(true, true, new Color(0.0f, 0.0f, 0.0f));

        var modelMatrix = new Matrix4x4();
        modelMatrix.SetTRS(pos, Quaternion.AngleAxis(angle, Vector3.right), scale);
        
        Graphics.DrawMeshNow(quadMesh.mesh, modelMatrix);
        //print("called");
        //Graphics.DrawMesh(quadMesh.mesh, modelMatrix, material, 0);

        // Stop rendering to map
        GL.PopMatrix();
        RenderTexture.active = oldRT;
    }
}
