using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

[Serializable]
public struct LODConfig
{
    public float range;
    //public bool useTesselation;
}

[RequireComponent(typeof(EdgeFansPresets))]
public class Planet : MonoBehaviour
{
    public bool debug = false;

    public Material material;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    [HideInInspector]
    public PBSNoiseScript pbsNoiseScript;
    [HideInInspector]
    public PBSNoiseGenerator noiseGenerator;

    public float radius = 10;

    ////////////////////////// Water ////////////////////////
    public Material waterMaterial;
    public float waterPercent = 0.25f;
    private GameObject waterMesh = null;

    public Transform cameraTransform;
    public float cullingMinAngle = 45.0f;
    Vector3 lastCameraPosition;
    public float lodThreshold = 0;
    [Min(0.1f)]
    public float intervalUpdateLOD = 0.5f;
    public LODConfig[] lods;
    bool isUsingNoiseGenetic = false;

    private void Awake()
    {
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        lastCameraPosition = cameraTransform.position;
        pbsNoiseScript = GetComponent<PBSNoiseScript>();
        if (pbsNoiseScript)
            noiseGenerator = pbsNoiseScript.GetNoiseGenerator();
        else
            noiseGenerator = new PBSNoiseGenerator();
    }

    private void Start()
    {
        Initialize();
        GenerateMesh();
        GenerateWater();

        StartCoroutine(PlanetGenerationLoop());
    }

    void GenerateWater()
    {
        if (waterMesh) Destroy(waterMesh);
        if(waterPercent > 0.0f)
        {
            waterMesh = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            waterMesh.name = "WaterSphere";
            waterMesh.transform.SetParent(transform);
            waterMesh.transform.localScale = Vector3.one * (radius +radius*waterPercent) * 2;
            MeshRenderer mr = waterMesh.GetComponent<MeshRenderer>();
            mr.sharedMaterial = waterMaterial;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    IEnumerator PlanetGenerationLoop()
    {
        while(true)
        {
            if (!isUsingNoiseGenetic && Vector3.Distance(cameraTransform.position, lastCameraPosition) >= lodThreshold)
            {
                lastCameraPosition = cameraTransform.position;
                UpdateMesh();
            }
            yield return new WaitForSeconds(intervalUpdateLOD);
        }
    }

    private void Initialize()
    {
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
            
        }
        terrainFaces = new TerrainFace[6];
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        // Pour chaque direction crée un TerrainFace
        for (int i=0; i<6; ++i)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = material;
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, directions[i], this);
        }
    }

    void GenerateMesh()
    {
        foreach(TerrainFace face in terrainFaces)
        {
            face.ConstructTree();
        }
    }

    void UpdateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.UpdateTree();
        }
    }

    private void OnDrawGizmos()
    {
        if (!debug || terrainFaces == null) return;
        foreach (TerrainFace tf in terrainFaces)
        {
            foreach (Chunk chunk in tf.GetVisibleChunks())
            {
                Gizmos.color = Color.Lerp(Color.red, Color.green, (float)chunk.detailLevel / (lods.Length - 1));
                float elevation = noiseGenerator.GetNoise3D(chunk.position.normalized);
                Gizmos.DrawSphere(chunk.position.normalized * (1+elevation)* radius, Mathf.Lerp((lods.Length - 1)/2, 0.5f, (float)chunk.detailLevel / (lods.Length - 1)));

                Gizmos.color = Color.red;
                chunk.GetNeighbourLOD();
                foreach (byte b in chunk.neighbours)
                    if (b == 1) Gizmos.DrawWireCube(chunk.position.normalized * (1 + elevation) * radius, Vector3.one * Mathf.Lerp((lods.Length - 1) / 2, 0.5f, (float)chunk.detailLevel / (lods.Length - 1)));
            }
        }
    }

    public void UpdateNoiseGenerator(PBSNoiseGenerator noiseGenerator)
    {
        this.noiseGenerator = noiseGenerator;
        GenerateMesh();
    }

    public void SetIsUsingNoiseGenetic(bool isUsingNoiseGenetic)
    {
        this.isUsingNoiseGenetic = isUsingNoiseGenetic;
    }

    public bool GetIstUsingNoiseGenetic()
    {
        return isUsingNoiseGenetic;
    }
}
