using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawInstancedScript : MonoBehaviour
{
    // Batch stuff
    const float BATCH_MAX_FLOAT = 1023f;
    const int BATCH_MAX = 1023;
    public const string GRASS_SURFACE_TAG = "GrassSurface";

    public GameObject target;
    public GameObject prefab;
    public Material meshMaterial;
    public Texture2D colorMap;

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
        
        propertyBlock = new MaterialPropertyBlock();

        var bounds = target.GetComponent<MeshRenderer>().bounds;
        var w = Mathf.RoundToInt(bounds.size.x - mMeshRenderer.bounds.size.x * 2.0f);
        var d = Mathf.RoundToInt(bounds.size.z - mMeshRenderer.bounds.size.z * 2.0f);
        var startX = bounds.min.x + mMeshRenderer.bounds.size.x - target.transform.position.x;
        var startZ = bounds.min.z + mMeshRenderer.bounds.size.z - target.transform.position.z;

        for (int i = 0; i < w / spacing; ++i)
        {
            for (int j = 0; j < d / spacing; ++j)
            {
                int idx = i * d + j;
                pos.x = startX + i * spacing;
                pos.y = 0.0f;
                pos.z = startZ + j * spacing;

                Vector3 origin = pos + new Vector3(0.0f, 100.0f, 0.0f);
                RaycastHit hitInfo;
                if (Physics.Raycast(origin, Vector3.down, out hitInfo))
                {
                    if (hitInfo.transform.tag == GRASS_SURFACE_TAG)
                    {
                        pos.y = hitInfo.point.y;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                BatchBucket currentBucket = Buckets[Buckets.Count - 1];
                if (currentBucket.IsFull())
                {
                    currentBucket = new BatchBucket();
                    Buckets.Add(currentBucket);
                }

                currentBucket.MatrixArray[currentBucket.NumInstances] = Matrix4x4.identity;

                float curNoise = Mathf.PerlinNoise(pos.x / (float)w, pos.z / (float)d);

                if (curNoise >= threshold)
                {
                    currentBucket.MatrixArray[currentBucket.NumInstances].SetTRS(pos, Quaternion.identity, scale);
                }

                var wProgress = (pos.x - startX) / w;
                var dProgress = (pos.z - startZ) / d;
                currentBucket.ColorArray[currentBucket.NumInstances] = colorMap.GetPixelBilinear(wProgress, dProgress);
                currentBucket.PositionArray[currentBucket.NumInstances] = pos;

                currentBucket.NumInstances++;
            }
        }
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
