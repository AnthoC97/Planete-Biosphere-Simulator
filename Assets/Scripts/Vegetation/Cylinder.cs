using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cylinder
{
    public float radius;
    public int deltaAngle;
    public Vector3 center; //base center
    public float height;

    public List<Vector3> vertices;
    public List<Vector2> uvs;
    public List<int> triangles;

    public Cylinder(Vector3 center, float radius, float height, int deltaAngle)
    {
        this.center = center;
        this.radius = radius;
        this.height = height;
        this.deltaAngle = deltaAngle;

        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();
    }

    public Vector3 CreateCylinder()
    {
        //Create Base and top
        Circle cylinderBase = new Circle(center, radius, deltaAngle);
        Circle cylinderTop = new Circle(new Vector3(center.x, center.y+height ,center.z), radius, deltaAngle);
        cylinderBase.CreateCircle();
        cylinderTop.CreateCircle();
        //rellier
        vertices.AddRange(cylinderBase.vertices);
        vertices.AddRange(cylinderTop.vertices);
        triangles.AddRange(cylinderBase.triangles);
        triangles.AddRange(cylinderTop.triangles);

        int nbTriangles = triangles.Count / 6;
        for (int i = 1; i < vertices.Count / 2 - 1; i++)
        {
            triangles.Add(i);
            triangles.Add(i + vertices.Count / 2);
            triangles.Add(i + 1);

            triangles.Add(i + vertices.Count / 2);
            triangles.Add(i + vertices.Count / 2 + 1);
            triangles.Add(i + 1);
        }
        return cylinderTop.center;
    }
}
