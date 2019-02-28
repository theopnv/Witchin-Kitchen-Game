using UnityEngine;

public class DrawInstancedScript : MonoBehaviour
{
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
    private Vector4[] colorArray;
    private Vector4[] instancePositionArray;
    private Matrix4x4[] matrices;
    private MaterialPropertyBlock propertyBlock;

    private float CurTime = 0.0f;
    public float WindSpeed = 1.0f;
    public Vector3 WindDirection = new Vector3();
    public Vector3 WindDirectionModulated = new Vector3();
    private Texture2D RollingWindTex;
    private Vector2 RollingWindOffset = new Vector2();
    private float MeshHeight = 0.0f;
    
    void Start()
    {
        mMeshFilter = prefab.GetComponent<MeshFilter>();
        mMeshRenderer = prefab.GetComponent<MeshRenderer>();

        RollingWindTex = GetComponent<PerlinTexture>().getTex();

        InitData();
    }

    private void InitData()
    {
        Vector3 pos = new Vector3();
        Vector3 scale = new Vector3(1, 1, 1);

        int count = width * depth;
        matrices = new Matrix4x4[count];
        colorArray = new Vector4[count];
        instancePositionArray = new Vector4[count];
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
                int idx = i * depth + j;

                matrices[idx] = Matrix4x4.identity;

                pos.x = i * spacing;
                pos.y = 0.0f;
                pos.z = j * spacing;

                float curNoise = Mathf.PerlinNoise(pos.x / (float)width, pos.z / (float)depth);

                if (curNoise >= threshold)
                {
                    matrices[idx].SetTRS(pos, Quaternion.identity, scale);
                }

                colorArray[idx] = colors[idx % colors.Length];
                instancePositionArray[idx] = pos;
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
        WindDirectionModulated = WindDirection;
        WindDirectionModulated.z += 0.5f * Mathf.Sin(Time.time * 0.1f);
        print(WindDirectionModulated);
        WindDirectionModulated.Normalize();

        CurTime += Time.deltaTime;
        RollingWindOffset.x -= WindDirectionModulated.x * (WindSpeed * Time.deltaTime);
        RollingWindOffset.y -= WindDirectionModulated.z * (WindSpeed * Time.deltaTime);

        int total = width * depth;
        int batches = (int)Mathf.Ceil(total / BATCH_MAX_FLOAT);

        for (int i = 0; i < batches; ++i)
        {
            int batchCount = Mathf.Min(BATCH_MAX, total - (BATCH_MAX * i));

            int start = Mathf.Max(0, (i - 1) * BATCH_MAX);

            Matrix4x4[] batchedMatrices = GetBatchedMatrices(start, batchCount);
            Vector4[] batchedColorArray = GetBatchedArray(start, batchCount);
            Vector4[] batchedInstancePositionArray = GetBatchedInstancePositionArray(start, batchCount);

            propertyBlock.SetVectorArray("_Color", batchedColorArray);
            propertyBlock.SetVectorArray("_InstancePosition", batchedInstancePositionArray);

            propertyBlock.SetFloat("_CurTime", CurTime);
            propertyBlock.SetVector("_WindDirection", WindDirectionModulated);
            propertyBlock.SetTexture("_RollingWindTex", RollingWindTex);
            propertyBlock.SetVector("_RollingWindOffset", RollingWindOffset);
            propertyBlock.SetFloat("_MeshHeight", MeshHeight);

            Graphics.DrawMeshInstanced(mMeshFilter.sharedMesh, 0, meshMaterial, batchedMatrices, batchCount, propertyBlock);
        }
    }

    private Vector4[] GetBatchedArray(int offset, int batchCount)
    {
        Vector4[] batchedArray = new Vector4[batchCount];

        for (int i = 0; i < batchCount; ++i)
        {
            batchedArray[i] = colorArray[i + offset];
        }

        return batchedArray;
    }

    private Vector4[] GetBatchedInstancePositionArray(int offset, int batchCount)
    {
        Vector4[] batchedArray = new Vector4[batchCount];

        for (int i = 0; i < batchCount; ++i)
        {
            batchedArray[i] = instancePositionArray[i + offset];
        }

        return batchedArray;
    }

    private Matrix4x4[] GetBatchedMatrices(int offset, int batchCount)
    {
        Matrix4x4[] batchedMatrices = new Matrix4x4[batchCount];

        for (int i = 0; i < batchCount; ++i)
        {
            batchedMatrices[i] = matrices[i + offset];
        }

        return batchedMatrices;
    }
}
