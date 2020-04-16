using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle
{
    public Vector3 center;
    public float radius;
    public int deltaAngle;
    public List<Vector3> vertices;
    public List<Vector2> uvs;
    public List<int> triangles;

    public Circle(Vector3 center, float radius, int deltaAngle)
    {
        this.center = center;
        this.radius = radius;
        this.deltaAngle = deltaAngle;
        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();
    }

    public void CreateCircle()
    {
        float val = Mathf.PI / 180f;
        vertices.Add(center);
        uvs.Add(new Vector2(0.5f, 0.5f));
        int triangleCount = 0;

        float x1 = center.x + radius * Mathf.Cos(0);
        float y1 = center.y;
        float z1 = center.z + radius * Mathf.Sin(0);
        Vector3 point1 = new Vector3(x1, y1, z1);
        vertices.Add(point1);
        uvs.Add(new Vector2((x1 + radius) / 2 * radius, (y1 + radius) / 2 * radius));

        for (int i = 0; i < 359; i = i + deltaAngle)
        {
            float x2 = center.x + radius * Mathf.Cos((i + deltaAngle) * val);
            float y2 = center.y;
            float z2 = center.z + radius * Mathf.Sin((i + deltaAngle) * val);
            Vector3 point2 = new Vector3(x2, y2, z2);
            vertices.Add(point2);
            uvs.Add(new Vector2((x2 + radius) / 2 * radius, (z2 + radius) / 2 * radius));
            triangles.Add(0);
            triangles.Add(triangleCount + 2);
            triangles.Add(triangleCount + 1);
            triangleCount++;
            point1 = point2;
        }
    }
}
