using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplacementMap : MonoBehaviour
{
    public const int GRASS_DISPLACEMENT_LAYER = 9;


    static private DisplacementMap Instance;

    static public DisplacementMap Get()
    {
        return Instance;
    }


    public Camera cam;
    public RenderTexture rt;
    public GameObject target;
    public Bounds targetBounds;
    public int pixelsPerGameUnit = 10;

    private void Awake()
    {
        Instance = this;

        targetBounds = target.GetComponent<MeshRenderer>().bounds;
        var w = Mathf.RoundToInt(targetBounds.size.x * pixelsPerGameUnit);
        var h = Mathf.RoundToInt(targetBounds.size.z * pixelsPerGameUnit);

        rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
        rt.filterMode = FilterMode.Bilinear;
        rt.wrapModeU = TextureWrapMode.Clamp;
        rt.wrapModeV = TextureWrapMode.Clamp;
        rt.Create();
        
        cam.orthographic = true;
        cam.cullingMask = 1 << GRASS_DISPLACEMENT_LAYER;
        cam.targetTexture = rt;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        cam.transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.right);
        cam.transform.position = targetBounds.center + Vector3.up * 100.0f;
        cam.orthographicSize = targetBounds.extents.z;
        cam.aspect = targetBounds.size.x / targetBounds.size.z;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
