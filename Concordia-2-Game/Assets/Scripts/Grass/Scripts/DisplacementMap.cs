using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplacementMap : MonoBehaviour
{
    static private DisplacementMap Instance;

    static public DisplacementMap Get()
    {
        return Instance;
    }


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
