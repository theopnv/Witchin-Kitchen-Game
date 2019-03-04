using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grass : MonoBehaviour
{
    // Batch stuff
    const float BATCH_MAX_FLOAT = 1023f;
    const int BATCH_MAX = 1023;
    public const int GRASS_SURFACE_LAYER_MASK = 10;

    public GameObject target;
    private Bounds targetBounds;
    public GameObject prefab;
    public float prefabScale = 1.0f;
    public Material meshMaterial;
    public Texture2D colorMap;
    public Texture2D instanceMap;

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
        Vector3 scale = new Vector3(prefabScale, prefabScale, prefabScale);
        
        propertyBlock = new MaterialPropertyBlock();

        targetBounds = target.GetComponent<MeshRenderer>().bounds;
        var w = targetBounds.size.x - mMeshRenderer.bounds.size.x * 2.0f;
        var d = targetBounds.size.z - mMeshRenderer.bounds.size.z * 2.0f;
        var startX = targetBounds.min.x + mMeshRenderer.bounds.size.x - target.transform.position.x;
        var startZ = targetBounds.min.z + mMeshRenderer.bounds.size.z - target.transform.position.z;

        for (int i = 0; i < instanceMap.width; ++i)
        {
            for (int j = 0; j < instanceMap.height; ++j)
            {
                if (instanceMap.GetPixel(i, j).r < 1.0)
                {
                    continue;
                }

                var progressX = i / (float)instanceMap.width;
                var progressZ = j / (float)instanceMap.height;

                pos.x = startX + progressX * w;
                pos.y = 0.0f;
                pos.z = startZ + progressZ * d;

                Vector3 origin = pos + new Vector3(0.0f, 100.0f, 0.0f);
                RaycastHit hitInfo;
                if (Physics.Raycast(origin, Vector3.down, out hitInfo))
                {
                    if (hitInfo.transform.gameObject != null &&
                        hitInfo.transform.gameObject.layer == GRASS_SURFACE_LAYER_MASK)
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

                currentBucket.PositionArray[currentBucket.NumInstances] = pos;
                currentBucket.MatrixArray[currentBucket.NumInstances] = Matrix4x4.identity;
                currentBucket.MatrixArray[currentBucket.NumInstances].SetTRS(pos, Quaternion.identity, scale);
                currentBucket.ColorArray[currentBucket.NumInstances] = colorMap.GetPixelBilinear(progressX, progressZ);

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
            propertyBlock.SetTexture("_Displacement", DisplacementMap.Get().rt);

            var grassBounds = new Vector4();
            grassBounds.x = targetBounds.center.x - targetBounds.extents.x;
            grassBounds.y = targetBounds.center.z - targetBounds.extents.z;
            grassBounds.z = targetBounds.size.x;
            grassBounds.w = targetBounds.size.z;
            propertyBlock.SetVector("_Bounds", grassBounds);

            Graphics.DrawMeshInstanced(
                mMeshFilter.sharedMesh,
                0,
                meshMaterial,
                bucket.MatrixArray,
                bucket.NumInstances,
                propertyBlock,
                UnityEngine.Rendering.ShadowCastingMode.On,
                true
            );
        }
    }
}
