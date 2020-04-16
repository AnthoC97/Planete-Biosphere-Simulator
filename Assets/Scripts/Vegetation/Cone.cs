using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cone
{
    public float radius;
    public int deltaAngle;
    public Vector3 center; //base center
    public float height;

    public List<Vector3> vertices;
    public List<Vector2> uvs;
    public List<int> triangles;

    public Cone(Vector3 center, float radius, float height, int deltaAngle)
    {
        this.center = center;
        this.radius = radius;
        this.height = height;
        this.deltaAngle = deltaAngle;

        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();
    }

    public void CreateCone()
    {
        //Create Base
        Circle coneBase = new Circle(center, radius, deltaAngle);
        coneBase.CreateCircle();
        //Create the top vertex
        Vector3 top = new Vector3(center.x, height, center.z);
        //rellier
        vertices.AddRange(coneBase.vertices);
        triangles.AddRange(coneBase.triangles);
        vertices.Add(new Vector3(center.x, height, center.z));

        int nbTriangles = triangles.Count / 3;
        for (int i = 0; i < nbTriangles; i++)
        {
            triangles.Add(vertices.Count - 1);
            triangles.Add(i + 2);
            triangles.Add(i + 1);
        }
    }

    
}
