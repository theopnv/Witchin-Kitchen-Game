using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class Grass : MonoBehaviour
{
    // Batch stuff
    const float BATCH_MAX_FLOAT = 1023f;
    const int BATCH_MAX = 1023;
    public const int GRASS_SURFACE_LAYER_MASK = 10;
    public const int GRASS_GEOMETRY_LAYER_MASK = 11;

    public GameObject target;
    private Bounds targetBounds;
    public GameObject prefab;
    public float prefabScale = 1.0f;
    public Vector3 minScale = new Vector3(0.35f, 0.25f, 0.35f);
    public Material meshMaterial;
    public Material motionVectorsMaterial;
    public Texture2D colorMap;
    public Texture2D instanceMap;
    public Texture2D heightMap;

    private MeshFilter mMeshFilter;
    private MeshRenderer mMeshRenderer;
    private MaterialPropertyBlock propertyBlock;

    private class BatchBucket
    {
        public int NumInstances;
        public Vector4[] ColorArray;
        public Vector4[] PositionArray;
        public Matrix4x4[] MatrixArray;
        public MaterialPropertyBlock MotionVectorsPropertyBlock;
        public CommandBuffer MotionVectorsPass;

        public BatchBucket()
        {
            ColorArray = new Vector4[BATCH_MAX];
            PositionArray = new Vector4[BATCH_MAX];
            MatrixArray = new Matrix4x4[BATCH_MAX];
            MotionVectorsPropertyBlock = new MaterialPropertyBlock();
            MotionVectorsPass = new CommandBuffer();
        }

        public bool IsFull()
        {
            return NumInstances == BATCH_MAX;
        }
    }

    private List<BatchBucket> Buckets = new List<BatchBucket>();


    // Wind anim stuff
    public bool RenderMotionVectors = true;
    private float CurTime = 0.0f;
    public float DisplacementStrength = 1.0f;
    public float Flexibility = 1.0f;
    public float WindStrength = 2.0f;
    public float WindScrollSpeed = 0.1f;
    public Vector3 WindDirection = new Vector3();
    public float WindDirectionModulationTimeScale = 0.1f;
    public float WindDirectionModulationStrength = 0.5f;
    public float RollingWindPositionScale = 0.001f;
    public Vector3 Scale = new Vector3(1.0f, 1.0f, 1.0f);
    public Color FreezeColor;
    public float FreezeFactor = 0.0f;
    private Vector3 WindDirectionModulated = new Vector3();
    private Texture2D RollingWindTex;
    private Vector2 RollingWindOffset = new Vector2();
    private float MeshHeight = 0.0f;

    // Previous frame data for TAA
    private float PrevCurTime = 0.0f;
    private float PrevDisplacementStrength = 1.0f;
    private float PrevFlexibility = 1.0f;
    private float PrevWindStrength = 2.0f;
    private Vector3 PrevScale = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 PrevWindDirectionModulated = new Vector3();
    private Vector2 PrevRollingWindOffset = new Vector2();


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
        var w = targetBounds.size.x;
        var d = targetBounds.size.z;
        var startX = targetBounds.min.x - target.transform.position.x;
        var startZ = targetBounds.min.z - target.transform.position.z;

        //print(w);
        //print(d);
        //print(startX);
        //print(startZ);
        //print(instanceMap.width);
        //print(instanceMap.height);

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
                        (hitInfo.transform.gameObject.layer & GRASS_SURFACE_LAYER_MASK) > 0)
                    {
                        pos.y = hitInfo.point.y;
                    }
                    else
                    {
                        //if (hitInfo.transform.gameObject == null)
                        //{
                        //    print("noname");
                        //}
                        //else
                        //{
                        //    print(hitInfo.transform.gameObject.name);
                        //    print(hitInfo.transform.gameObject.layer);
                        //}

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
                
                var lerpFactor = 1.0f - heightMap.GetPixelBilinear(progressX, progressZ).r;
                var instanceScale = scale - lerpFactor * (scale - minScale);

                currentBucket.PositionArray[currentBucket.NumInstances] = pos;
                currentBucket.MatrixArray[currentBucket.NumInstances] = Matrix4x4.identity;
                currentBucket.MatrixArray[currentBucket.NumInstances].SetTRS(pos, Quaternion.identity, instanceScale);
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
            propertyBlock.SetFloat("_DisplacementStrength", DisplacementStrength);
            propertyBlock.SetFloat("_Flexibility", Flexibility);
            propertyBlock.SetFloat("_WindStrength", WindStrength);
            propertyBlock.SetVector("_WindDirection", WindDirectionModulated);
            propertyBlock.SetFloat("_RollingWindPositionScale", RollingWindPositionScale);
            propertyBlock.SetTexture("_RollingWindTex", RollingWindTex);
            propertyBlock.SetVector("_RollingWindOffset", RollingWindOffset);
            propertyBlock.SetFloat("_MeshHeight", MeshHeight);
            propertyBlock.SetVector("_Scale", Scale);
            propertyBlock.SetTexture("_Displacement", DisplacementMap.Get().rt);
            propertyBlock.SetColor("_FreezeColor", FreezeColor);
            propertyBlock.SetFloat("_FreezeFactor", FreezeFactor);

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
                true,
                GRASS_GEOMETRY_LAYER_MASK
            );
        }
    }

    void OnRenderObject()
    {
        var camera = Camera.current;

        if (RenderMotionVectors && (camera.depthTextureMode & DepthTextureMode.MotionVectors) != 0)
        {
            var nonJitteredVP = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true) * camera.worldToCameraMatrix;

            for (int i = 0; i < Buckets.Count; ++i)
            {
                BatchBucket bucket = Buckets[i];
                var props = bucket.MotionVectorsPropertyBlock;
                var pass = bucket.MotionVectorsPass;

                // Build and execute the motion vector rendering pass.
                pass.Clear();
                if (camera.allowMSAA && camera.actualRenderingPath == RenderingPath.Forward)
                    pass.SetRenderTarget(BuiltinRenderTextureType.MotionVectors);
                else
                    pass.SetRenderTarget(BuiltinRenderTextureType.MotionVectors, BuiltinRenderTextureType.CameraTarget);

                // Set the per-camera properties.
                props.SetMatrix("_PreviousVP", camera.previousViewProjectionMatrix);
                props.SetMatrix("_NonJitteredVP", nonJitteredVP);

                // The usual
                props.SetVectorArray("_InstancePosition", bucket.PositionArray);
                props.SetFloat("_CurTime", CurTime);
                props.SetFloat("_DisplacementStrength", DisplacementStrength);
                props.SetFloat("_Flexibility", Flexibility);
                props.SetFloat("_WindStrength", WindStrength);
                props.SetVector("_WindDirection", WindDirectionModulated);
                props.SetFloat("_RollingWindPositionScale", RollingWindPositionScale);
                props.SetTexture("_RollingWindTex", RollingWindTex);
                props.SetVector("_RollingWindOffset", RollingWindOffset);
                props.SetFloat("_MeshHeight", MeshHeight);
                props.SetVector("_Scale", Scale);
                props.SetTexture("_Displacement", DisplacementMap.Get().rt);

                var grassBounds = new Vector4();
                grassBounds.x = targetBounds.center.x - targetBounds.extents.x;
                grassBounds.y = targetBounds.center.z - targetBounds.extents.z;
                grassBounds.z = targetBounds.size.x;
                grassBounds.w = targetBounds.size.z;
                props.SetVector("_Bounds", grassBounds);

                // Previous frame for TAA
                props.SetTexture("_PrevDisplacement", DisplacementMap.Get().prevrt);
                props.SetFloat("_PrevCurTime", PrevCurTime);
                props.SetFloat("_PrevDisplacementStrength", PrevDisplacementStrength);
                props.SetFloat("_PrevFlexibility", PrevFlexibility);
                props.SetFloat("_PrevWindStrength", PrevWindStrength);
                props.SetVector("_PrevWindDirection", PrevWindDirectionModulated);
                props.SetVector("_PrevRollingWindOffset", PrevRollingWindOffset);
                props.SetVector("_PrevScale", PrevScale);

                // Update last frame data
                PrevCurTime = CurTime;
                PrevDisplacementStrength = DisplacementStrength;
                PrevFlexibility = Flexibility;
                PrevWindStrength = WindStrength;
                PrevWindDirectionModulated = WindDirectionModulated;
                PrevRollingWindOffset = RollingWindOffset;
                PrevScale = Scale;

                // Draw
                pass.DrawMeshInstanced(
                    mMeshFilter.sharedMesh,
                    0,
                    motionVectorsMaterial,
                    -1,
                    bucket.MatrixArray,
                    bucket.NumInstances,
                    props
                );
                Graphics.ExecuteCommandBuffer(pass);
            }
        }
    }
}
