using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawInstancedScript : MonoBehaviour
{
    // Batch stuff
    const float BATCH_MAX_FLOAT = 1023f;
    const int BATCH_MAX = 1023;

    public GameObject prefab;
    public Material meshMaterial;
    
    public int width;
    public int depth;

    public float spacing;
    public float threshold;

    private MeshFilter mMeshFilter;
    private MeshRenderer mMeshRenderer;
    private MaterialPropertyBlock propertyBlock;

    private class BatchBucket
    {
        public int NumInstances;
        public Vector4[] ColorArray;
        public Vector4[] PositionArray;
        public Matrix4x4[] MatrixArray;

        public BatchBucket()
        {
            ColorArray = new Vector4[BATCH_MAX];
            PositionArray = new Vector4[BATCH_MAX];
            MatrixArray = new Matrix4x4[BATCH_MAX];
        }

        public bool IsFull()
        {
            return NumInstances == BATCH_MAX;
        }
    }

    private List<BatchBucket> Buckets = new List<BatchBucket>();


    // Wind anim stuff
    private float CurTime = 0.0f;
    public float WindStrength = 2.0f;
    public float WindScrollSpeed = 0.1f;
    public Vector3 WindDirection = new Vector3();
    public float WindDirectionModulationTimeScale = 0.1f;
    public float WindDirectionModulationStrength = 0.5f;
    public float RollingWindPositionScale = 0.001f;
    private Vector3 WindDirectionModulated = new Vector3();
    private Texture2D RollingWindTex;
    private Vector2 RollingWindOffset = new Vector2();
    private float MeshHeight = 0.0f;
    

    void Start()
    {
        mMeshFilter = prefab.GetComponent<MeshFilter>();
        mMeshRenderer = prefab.GetComponent<MeshRenderer>();

        RollingWindTex = GetComponent<PerlinTexture>().getTex();

        Buckets.Add(new BatchBucket());
        InitData();
    }

    private void InitData()
    {
        Vector3 pos = new Vector3();
        Vector3 scale = new Vector3(1, 1, 1);

        int count = width * depth;
        propertyBlock = new MaterialPropertyBlock();
        
        Color[] colors = {
                            HexToColor(0x03A9F4),
                            HexToColor(0xE1F5FE),
                            HexToColor(0xB3E5FC),
                            HexToColor(0x81D4FA),
                            HexToColor(0x4FC3F7),
                            HexToColor(0x29B6F6),
                            HexToColor(0x03A9F4),
                            HexToColor(0x039BE5),
                            HexToColor(0x0288D1),
                            HexToColor(0x0277BD),
                            HexToColor(0x01579B),
                            HexToColor(0x80D8FF),
                            HexToColor(0x40C4FF),
                            HexToColor(0x00B0FF),
                            HexToColor(0x0091EA)
                        };

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < depth; ++j)
            {
                BatchBucket currentBucket = Buckets[Buckets.Count - 1];
                if (currentBucket.IsFull())
                {
                    currentBucket = new BatchBucket();
                    Buckets.Add(currentBucket);
                }

                int idx = i * depth + j;

                currentBucket.MatrixArray[currentBucket.NumInstances] = Matrix4x4.identity;

                pos.x = i * spacing;
                pos.y = 0.0f;
                pos.z = j * spacing;

                float curNoise = Mathf.PerlinNoise(pos.x / (float)width, pos.z / (float)depth);

                if (curNoise >= threshold)
                {
                    currentBucket.MatrixArray[currentBucket.NumInstances].SetTRS(pos, Quaternion.identity, scale);
                }
                
                currentBucket.ColorArray[currentBucket.NumInstances] = colors[idx % colors.Length];
                currentBucket.PositionArray[currentBucket.NumInstances] = pos;

                currentBucket.NumInstances++;
            }
        }
    }

    private Color HexToColor(uint hex)
    {
        float r = (hex >> 16) & 0xff;
        float g = (hex >> 8) & 0xff;
        float b = (hex) & 0xff;
        Color newColor = new Color(r / 255f, g / 255f, b / 255f);

        return newColor;
    }

    void Update()
    {
        MeshHeight = mMeshFilter.sharedMesh.bounds.size.y;

        WindDirection.Normalize();
        var perpendicularWindDirection = new Vector3(WindDirection.z, 0.0f, -WindDirection.x);
        var modulation = WindDirectionModulationStrength * Mathf.Sin(Time.time * WindDirectionModulationTimeScale);
        WindDirectionModulated = WindDirection + perpendicularWindDirection * modulation;
        WindDirectionModulated.Normalize();

        CurTime += Time.deltaTime;
        RollingWindOffset.x -= WindDirectionModulated.x * (WindScrollSpeed * Time.deltaTime);
        RollingWindOffset.y -= WindDirectionModulated.z * (WindScrollSpeed * Time.deltaTime);

        int total = width * depth;
        int batches = (int)Mathf.Ceil(total / BATCH_MAX_FLOAT);

        for (int i = 0; i < Buckets.Count; ++i)
        {
            BatchBucket bucket = Buckets[i];

            propertyBlock.SetVectorArray("_Color", bucket.ColorArray);
            propertyBlock.SetVectorArray("_InstancePosition", bucket.PositionArray);

            propertyBlock.SetFloat("_CurTime", CurTime);
            propertyBlock.SetFloat("_WindStrength", WindStrength);
            propertyBlock.SetVector("_WindDirection", WindDirectionModulated);
            propertyBlock.SetFloat("_RollingWindPositionScale", RollingWindPositionScale);
            propertyBlock.SetTexture("_RollingWindTex", RollingWindTex);
            propertyBlock.SetVector("_RollingWindOffset", RollingWindOffset);
            propertyBlock.SetFloat("_MeshHeight", MeshHeight);

            Graphics.DrawMeshInstanced(mMeshFilter.sharedMesh, 0, meshMaterial, bucket.MatrixArray, bucket.NumInstances, propertyBlock);
        }
    }
}
