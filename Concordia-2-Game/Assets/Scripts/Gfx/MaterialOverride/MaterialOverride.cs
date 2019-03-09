using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOverride : MonoBehaviour
{
    [System.Serializable]
    public enum MaterialOverrideEntryType
    {
        Color,
        Texture,
    }

    [System.Serializable]
    public class MaterialOverrideEntry
    {
        public MaterialOverrideEntryType Type;
        public string Name;
        public Color Color;
        public Texture2D Texture;
    }
    
    public List<MaterialOverrideEntry> Entries = new List<MaterialOverrideEntry>();

    protected Renderer OurRenderer;
    protected Material SourceMaterial;
    protected Material ClonedMaterial;

    // Start is called before the first frame update
    void Start()
    {
        CloneMaterial();
        Apply();
    }

    protected void CloneMaterial()
    {
        OurRenderer = gameObject.GetComponent<Renderer>();
        SourceMaterial = OurRenderer.sharedMaterial;
        ClonedMaterial = new Material(SourceMaterial);
        OurRenderer.sharedMaterial = ClonedMaterial;
    }

    public void Apply()
    {
        foreach (var entry in Entries)
        {
            if (entry.Name == null)
            {
                throw new System.Exception("No property name set");
            }

            if (!ClonedMaterial.HasProperty(entry.Name))
            {
                throw new System.Exception("No material property with name: " + entry.Name);
            }

            if (entry.Type == MaterialOverrideEntryType.Color)
            {
                if (entry.Color == null)
                {
                    throw new System.Exception("No color set");
                }
                else
                {
                    ClonedMaterial.SetColor(entry.Name, entry.Color);
                }
            }
            else if (entry.Type == MaterialOverrideEntryType.Texture)
            {
                if (entry.Texture == null)
                {
                    throw new System.Exception("No texture set");
                }
                else
                {
                    ClonedMaterial.SetTexture(entry.Name, entry.Texture);
                }
            }
            else
            {
                throw new System.Exception("No type set on entry");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
