using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSurface : MonoBehaviour
{
    private void Awake()
    {
        gameObject.layer = Grass.GRASS_SURFACE_LAYER_MASK;
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
