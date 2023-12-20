using UnityEngine;
using UnityEngine.Splines;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class River : MonoBehaviour
{
    public static River Instance { get; private set; }
   
    [SerializeField]
    private Mesh2D shape2D;
    [SerializeField]
    private SplineContainer splineContainer;
    List<BezierKnot> knots;

    [Range(0, 1)]
    public float T;

    private float lastT;
   

    [SerializeField]
    [Range(0, 32)]
    private float edgeRingCount = 8;

    [SerializeField]
    [Range(0, 40)]
    private int planeSize = 40;

    Mesh mesh;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        splineContainer = GetComponent<SplineContainer>();
        knots = splineContainer.Spline.Knots.ToList();
        mesh = new Mesh();
        mesh.name = "River";
        GetComponent<MeshFilter>().mesh = mesh;
        GenerateMesh();
    }

    void Update()
    {
        if (lastT == T)
            return;
        BezierKnot second = knots[1];
        BezierKnot third = knots[2];

        second.Position = splineContainer.transform.InverseTransformPoint(Vector3.Lerp(new Vector3(-40, 0, 0), new Vector3(-40, 0, 10), T));
        third.Position = splineContainer.transform.InverseTransformPoint(Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 0, -10), T));

        splineContainer.Spline.SetKnot(1, second);
        splineContainer.Spline.SetKnot(2, third);
        lastT = T;
    }

    private void OnDrawGizmos()
    {
        //void DrawPoint(Vector2 point) => Gizmos.DrawSphere(LocalToWorld(point, t), 0.1f);

        Vector3[] vs = shape2D.vertices.Select(v => LocalToWorldPos(v.point, T)).ToArray();

        for (int i = 0; i < shape2D.lineIndices.Length; i += 2)
        {
            Vector3 a = vs[shape2D.lineIndices[i]];
            Vector3 b = vs[shape2D.lineIndices[i + 1]];
            Gizmos.DrawLine(a, b);
        }

         
    }

    private Vector3 LocalToWorldPos(Vector3 point, float t)
    {
        Vector3 worldPoint = splineContainer.Spline.EvaluatePosition(t);
        Vector3 forward = Vector3.Normalize(splineContainer.Spline.EvaluateTangent(t));
        Vector3 up =  splineContainer.Spline.EvaluateUpVector(t);
        Quaternion rotation = Quaternion.LookRotation(forward, up);

        return worldPoint + rotation * point;
    }

    private Vector3 LocalToWorldVector(Vector3 point, float t)
    {
        Vector3 forward = Vector3.Normalize(splineContainer.Spline.EvaluateTangent(t));
        Vector3 up = splineContainer.Spline.EvaluateUpVector(t);
        Quaternion rotation = Quaternion.LookRotation(forward, up);

        return rotation * point;
    }

    private void GenerateMesh()
    {
        mesh.Clear();

        //Vertices
        List<Vector3> verts = new List<Vector3>();
        //List<Vector3> normals = new List<Vector3>();
        for (int ring = 0; ring < edgeRingCount; ring++)
        {
            float t = ring / (edgeRingCount - 1f);
            for (int i = 0; i < shape2D.vertices.Length; i++)
            {
                verts.Add(LocalToWorldPos(shape2D.vertices[i].point, t));
                //normals.Add(LocalToWorldVector(shape2D.vertices[i].normal, t));
            }
        }

        int vertsAmount = verts.Count;

        for (int ring = 0; ring < edgeRingCount; ring++)
        {
            verts.Add(verts[(ring * 10) + 9]);
            verts.Add(new Vector3(verts[(ring * 10) + 9].x, 1, -planeSize));
        }

        for (int ring = 0; ring < edgeRingCount; ring++)
        {
            verts.Add(new Vector3(verts[(ring * 10)].x, 1, planeSize));
            verts.Add(verts[(ring * 10)]);
        }

        //Triangles
        List<int> triIndices = new List<int>();
        for (int ring = 0; ring < edgeRingCount - 1; ring++)
        {
            int rootIndex = ring * shape2D.vertices.Length;
            int rootNextIndex = (ring + 1) * shape2D.vertices.Length;

            for(int line = 0; line < shape2D.lineIndices.Length;line += 2)
            {
                int lineIndexA = shape2D.lineIndices[line];
                int lineIndexB = shape2D.lineIndices[line + 1];
                int currentA = rootIndex + lineIndexA;
                int currentB = rootIndex + lineIndexB;
                int nextA = rootNextIndex + lineIndexA;
                int nextB = rootNextIndex + lineIndexB;
                triIndices.Add(currentA);
                triIndices.Add(nextA);
                triIndices.Add(nextB);
                triIndices.Add(currentA);
                triIndices.Add(nextB);
                triIndices.Add(currentB);
            }
        }

        for (int ring = 0;ring < edgeRingCount * 2 - 1; ring++)
        {
            if(ring == edgeRingCount - 1) 
            { 
                continue; 
            }

            triIndices.Add(vertsAmount + ( ring * 2) + 0); //current A
            triIndices.Add(vertsAmount + (ring * 2) + 2); //next A
            triIndices.Add(vertsAmount + (ring * 2) + 3); //next B
            triIndices.Add(vertsAmount + (ring * 2) + 0); //current A
            triIndices.Add(vertsAmount + (ring * 2) + 3); //next B
            triIndices.Add(vertsAmount + (ring * 2) + 1); //current B
        }

        mesh.SetVertices(verts);
        //mesh.SetNormals(normals);
        mesh.SetTriangles(triIndices, 0);
    }

    private void DisableOthers()
    {
        for(int ring = 0; ring < edgeRingCount; ring++)
        {
            float t = ring / (edgeRingCount - 1f);
            for (int i = 1; i < shape2D.vertices.Length - 1; i++)
            {
                RaycastHit hit;
                Vector3 pos = LocalToWorldPos(shape2D.vertices[i].point, t);
                if (Physics.Raycast(pos, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
                {
                    hit.transform.gameObject.SetActive(false);
                }
            }
        }
    }

    void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
    }

    void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
    {
        GenerateMesh();
        DisableOthers();
    }
}
