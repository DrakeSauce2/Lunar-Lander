using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public int numPoints = 100;
    public float amplitude = 1f;
    public float frequency = 1f;
    public float xOffset = 0f;
    public float yOffset = 0f;

    public float length;

    public GameObject prefab;

    void Start()
    {
        GenerateCosineWave();
    }

    void GenerateCosineWave()
    {
        int vertexCount = numPoints * 2;
        int triangleCount = (numPoints - 1) * 2;

        Vector3[] points = new Vector3[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];
        int[] triangles = new int[triangleCount * 3];

        for (int i = 0; i < numPoints; i++)
        {
            float t = (float)i / (numPoints - 1);
            float x = t * Mathf.PI * length;
            float y = Mathf.Sin(x * frequency) * amplitude;

            yOffset = Random.Range(-0.5f, 0.5f);

            y += yOffset;

            if (y < 0)
                y = 0;

            points[i] = new Vector3(transform.position.x + x + xOffset, transform.position.y + y + yOffset, 0f);

            if (prefab)
            {
                Instantiate(prefab, points[i], Quaternion.identity);
            }

            points[i * 2] = new Vector3(x + xOffset, y + yOffset, 0f);
            points[i * 2 + 1] = new Vector3(x + xOffset, 0f, 0f); // Bottom points
            uv[i * 2] = new Vector2(t, 1f);
            uv[i * 2 + 1] = new Vector2(t, 0f);

            if (i > 0)
            {
                int triIndex = (i - 1) * 6;
                int vertexIndex = (i - 1) * 2;
                triangles[triIndex] = vertexIndex;
                triangles[triIndex + 1] = vertexIndex + 1;
                triangles[triIndex + 2] = vertexIndex + 2;
                triangles[triIndex + 3] = vertexIndex + 1;
                triangles[triIndex + 4] = vertexIndex + 3;
                triangles[triIndex + 5] = vertexIndex + 2;
            }

            if (i > 0)
            {
                Debug.DrawLine(points[i - 1], points[i], Color.red, Mathf.Infinity);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = points;
        mesh.uv = uv;
        mesh.triangles = triangles;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // You can do something with the points array here (e.g., render a line).
    }
}
