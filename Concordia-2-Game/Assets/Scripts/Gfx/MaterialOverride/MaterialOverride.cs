using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.gfx
{
    public class MaterialOverride : MonoBehaviour
    {
        [System.Serializable]
        public enum EntryType
        {
            Color,
            Texture,
        }

        [System.Serializable]
        public class Entry
        {
            public EntryType Type;
            public string Name;
            public Color Color;
            public Texture2D Texture;

            public Entry()
            {

            }

            public Entry(string Name, Color Color)
            {
                Type = EntryType.Color;
                this.Name = Name;
                this.Color = Color;
            }

            public Entry(string Name, Texture2D Texture)
            {
                Type = EntryType.Texture;
                this.Name = Name;
                this.Texture = Texture;
            }
        }

        public List<Entry> Entries = new List<Entry>();

        protected Renderer OurRenderer;
        protected Material SourceMaterial;
        protected Material ClonedMaterial;
        
        void Awake()
        {
            CloneMaterial();
            Apply();
        }

        protected void CloneMaterial()
        {
            OurRenderer = gameObject.GetComponent<Renderer>();

            if (OurRenderer == null)
            {
                throw new System.Exception("Found no Renderer component whose material to modify");
            }
            
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

                if (entry.Type == EntryType.Color)
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
                else if (entry.Type == EntryType.Texture)
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
}
