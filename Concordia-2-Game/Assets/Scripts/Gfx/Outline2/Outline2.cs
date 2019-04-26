using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline2 : MonoBehaviour
{
    public Material OutlineMaterial;

    [Range(0.0f, 1.0f)]
    public float Thickness = 0.25f;

    // Start is called before the first frame update
    void Awake()
    {
        var children = GetComponentsInChildren<Outline2Marker>();
        foreach (var child in children)
        {
            var childRenderer = child.gameObject.GetComponent<Renderer>();

            var clone = Instantiate(childRenderer.gameObject);
            clone.transform.SetParent(gameObject.transform, false);

            var cloneRenderer = clone.GetComponent<Renderer>();
            cloneRenderer.sharedMaterial = OutlineMaterial;
            cloneRenderer.receiveShadows = false;
            cloneRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            var curScale = clone.transform.localScale;
            var extents = cloneRenderer.bounds.extents;

            var newScaleX = curScale.x * (extents.x + Thickness) / extents.x;
            var newScaleY = curScale.y * (extents.y + Thickness) / extents.y;
            var newScaleZ = curScale.z * (extents.z + Thickness) / extents.z;

            clone.transform.localScale = new Vector3(newScaleX, newScaleY, newScaleZ);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
