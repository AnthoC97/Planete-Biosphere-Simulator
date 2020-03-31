using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    Dictionary<Chunk, GameObject> debugSpheres = new Dictionary<Chunk, GameObject>();

    volatile Mesh mesh;
    //int resolution;
    Vector3 localUp;
    // The other axis
    Vector3 axisA;
    Vector3 axisB;

    Chunk parentChunk;
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();

    public TerrainFace(Mesh mesh/*, int resolution*/, Vector3 localUp)
    {
        this.mesh = mesh;
        //this.resolution = resolution;
        this.localUp = localUp;

        // randomly swap coordinate
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        // perpendiculare
        axisB = Vector3.Cross(localUp, axisA);
    }

    /*public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution-1)*(resolution-1)*6];
        int triIndex = 0;

        for(int y=0; y<resolution; ++y)
        {
            for(int x=0; x<resolution; ++x)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f)*2*axisA+(percent.y-0.5f)*2*axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                //vertices[i] = pointOnUnitCube;
                // spheriphy
                vertices[i] = pointOnUnitSphere;
                
                if(x!=resolution-1 && y!=resolution-1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex+1] = i+resolution+1;
                    triangles[triIndex+2] = i+resolution;

                    triangles[triIndex+3] = i;
                    triangles[triIndex+4] = i + 1;
                    triangles[triIndex+5] = i + resolution+1;
                    triIndex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }*/

    // Construct a quadtree of chunks
    public void ConstructTree()
    {
        // Reset the mesh
        vertices.Clear();
        triangles.Clear();

        // Generate chunks
        parentChunk = new Chunk(null, null, localUp * Planet.instance.radius, Planet.instance.radius, 0, localUp, axisA, axisB);
        parentChunk.GenerateChildren();

        // Get chunk mesh data
        int triangleOffset = 0;
        foreach(Chunk child in parentChunk.GetVisibleChildren())
        {
            (Vector3[], int[]) verticesAndTriangles = child.CalculateVerticesAndTriangles(triangleOffset);
            vertices.AddRange(verticesAndTriangles.Item1);
            triangles.AddRange(verticesAndTriangles.Item2);
            triangleOffset += verticesAndTriangles.Item1.Length;
        }

        mesh.Clear();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    public void UpdateTree()
    {
        // Reset the mesh
        vertices.Clear();
        triangles.Clear();

        // Update chunks
        parentChunk.UpdateChunk();
        parentChunk.GenerateChildren();

        // Get chunk mesh data
        int triangleOffset = 0;
        foreach (Chunk child in parentChunk.GetVisibleChildren())
        {
            (Vector3[], int[]) verticesAndTriangles = child.CalculateVerticesAndTriangles(triangleOffset);
            vertices.AddRange(verticesAndTriangles.Item1);
            triangles.AddRange(verticesAndTriangles.Item2);
            triangleOffset += verticesAndTriangles.Item1.Length;
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    public Chunk[] GetVisibleChunks()
    {
        List<Chunk> toBeRendered = new List<Chunk>();
        toBeRendered.Add(parentChunk);
        toBeRendered.AddRange(parentChunk.GetVisibleChildren());
        return toBeRendered.ToArray();
    }
}

public class Chunk
{
    public Chunk[] children;
    public Chunk parent;
    public Vector3 position;
    public float radius;
    public int detailLevel;
    public Vector3 localUp;
    public Vector3 axisA;
    public Vector3 axisB;

    public Chunk(Chunk[] children, Chunk parent, Vector3 position, float radius, int detailLevel, Vector3 localUp, Vector3 axisA, Vector3 axisB)
    {
        this.children = children;
        this.parent = parent;
        this.position = position;
        this.radius = radius;
        this.detailLevel = detailLevel;
        this.localUp = localUp;
        this.axisA = axisA;
        this.axisB = axisB;
    }

    public void GenerateChildren()
    {
        // If the detail level is between 0 and max.
        if(detailLevel >= 0 && detailLevel < Planet.instance.lods.Length)
        {
            if (Vector3.Distance(position.normalized * Planet.instance.radius, Planet.cameraTransform.position) <= Planet.instance.lods[detailLevel].range)
            {
                children = new Chunk[4];
                children[0] = new Chunk(new Chunk[0], this, position + axisA * radius *0.5f + axisB * radius * 0.5f, radius * 0.5f, detailLevel + 1, localUp, axisA, axisB);
                children[1] = new Chunk(new Chunk[0], this, position + axisA * radius * 0.5f - axisB * radius * 0.5f, radius * 0.5f, detailLevel + 1, localUp, axisA, axisB);
                children[2] = new Chunk(new Chunk[0], this, position - axisA * radius * 0.5f + axisB * radius * 0.5f, radius * 0.5f, detailLevel + 1, localUp, axisA, axisB);
                children[3] = new Chunk(new Chunk[0], this, position - axisA * radius * 0.5f - axisB * radius * 0.5f, radius * 0.5f, detailLevel + 1, localUp, axisA, axisB);
                
                foreach (Chunk child in children)
                {
                    child.GenerateChildren();
                }
            }
        }
    }

    // Returns the latest chunk in every branch, aka the ones to be rendered
    public Chunk[] GetVisibleChildren()
    {
        List<Chunk> toBeRendered = new List<Chunk>();
        if(children.Length > 0)
        {
            foreach(Chunk child in children)
            {
                toBeRendered.AddRange(child.GetVisibleChildren());
            }
        }
        else
        {
            toBeRendered.Add(this);
        }

        return toBeRendered.ToArray();
    }

    public (Vector3[], int[]) CalculateVerticesAndTriangles(int triangleOffset)
    {
        int resolution = 8;
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; ++y)
        {
            for (int x = 0; x < resolution; ++x)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                // The origin is the position variable, offset is scaled by the radius
                Vector3 pointOnUnitCube = position + ((percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB) * radius;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = pointOnUnitSphere * Planet.instance.radius;

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i+triangleOffset;
                    triangles[triIndex + 1] = i+resolution+1+triangleOffset;
                    triangles[triIndex + 2] = i + resolution+triangleOffset;

                    triangles[triIndex + 3] = i + triangleOffset;
                    triangles[triIndex + 4] = i + 1 + triangleOffset;
                    triangles[triIndex + 5] = i + resolution + 1 + triangleOffset;
                    triIndex += 6;
                }
            }
        }
        return (vertices, triangles);
    }

    public void UpdateChunk()
    {
        float distanceToPlayer = Vector3.Distance(position.normalized * Planet.instance.radius, Planet.cameraTransform.position);
        if (detailLevel < Planet.instance.lods.Length)
        {
            if(distanceToPlayer > Planet.instance.lods[detailLevel].range)
            {
                children = new Chunk[0];
            }
            else
            {
                if(children.Length > 0)
                {
                    foreach(Chunk child in children)
                    {
                        child.UpdateChunk();
                    }
                }
                else
                {
                    GenerateChildren();
                }
            }
        }
    }
}