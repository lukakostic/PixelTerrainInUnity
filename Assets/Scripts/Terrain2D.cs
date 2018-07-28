using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain2D : MonoBehaviour {
    public int x;
    public int y;
    public Texture2D texture;
    public SpriteRenderer sprite;
    public Terrain2DMain termain;
    public BoxCollider2D bc2d;
    public Transform backCollider;
    public MeshCollider mc;
    public float depth = 0.01f;
    public Color32[] m_Colors;
    private int m_Width;
    private int m_Height;
    public BoxCollider bc;
    int called = 0;

    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<Vector3> m_Normals = new List<Vector3>();


    private bool HasPixel(int aX, int aY)
    {
        return m_Colors[aX + aY * m_Width].a > 50;
    }
    void Start () {
        m_Colors = texture.GetPixels32();
        GenCollider();
        //mc.enabled = false;
        /*
        bc = gameObject.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size *= 5;
        */
    }

    private enum Edge { top, left, bottom, right };


    public void GenCollider ()
    {
        called++;
        GenerateMesh();
    }

    void Update ()
    {
    }

    public void Dig (Vector3 p)
    {
        Vector2 pos = p;

        pos = transform.InverseTransformPoint(pos);
        pos.x += 0.5f;
        pos.y += 0.5f;

        pos.x *= texture.width;
        pos.y *= texture.height;

        float i = (int)(pos.x) + (int)(pos.y);

        if (i >= 0 && i <= (texture.width * texture.height))
        {
            m_Colors[(int)i] = Color.clear;


            texture.SetPixel((int)pos.x, (int)pos.y, Color.clear);
            texture.Apply();
            GenCollider();
        }
    }

    void AddQuad(Vector3 aFirstEdgeP1, Vector3 aFirstEdgeP2, Vector3 aSecondRelative, Vector3 aNormal, Vector2 aUV1, Vector2 aUV2, bool aFlipUVs)
    {
        m_Vertices.Add(aFirstEdgeP1);
        m_Vertices.Add(aFirstEdgeP2);
        m_Vertices.Add(aFirstEdgeP2 + aSecondRelative);
        m_Vertices.Add(aFirstEdgeP1 + aSecondRelative);
        m_Normals.Add(aNormal);
        m_Normals.Add(aNormal);
        m_Normals.Add(aNormal);
        m_Normals.Add(aNormal);
    }

    void AddEdge(int aX, int aY, Edge aEdge)
    {
        Vector2 size = new Vector2(1.0f / m_Width, 1.0f / m_Height);
        Vector2 uv = new Vector3(aX * size.x, aY * size.y);
        Vector2 P = uv - Vector2.one * 0.5f;
        uv += size * 0.5f;
        Vector2 P2 = P;
        Vector3 normal;
        if (aEdge == Edge.top)
        {
            P += size;
            P2.y += size.y;
            normal = Vector3.up;
        }
        else if (aEdge == Edge.left)
        {
            P.y += size.y;
            normal = Vector3.left;
        }
        else if (aEdge == Edge.bottom)
        {
            P2.x += size.x;
            normal = Vector3.down;
        }
        else
        {
            P2 += size;
            P.x += size.x;
            normal = Vector3.right;
        }
        AddQuad(P, P2, Vector3.forward * depth, normal, uv, uv, false);
    }

    void GenerateMesh()
    {
        while(called>0){
        m_Vertices.Clear();
        m_Normals.Clear();




        m_Width = texture.width;
        m_Height = texture.height;


        //      first point                     , second point                    , relative 3. P, normal,          lower UV,     Upper UV,    flipUV
        // AddQuad(new Vector3(-0.5f, -0.5f, 0), new Vector3(-0.5f, 0.5f, 0), Vector3.right, Vector3.back, Vector2.zero, Vector2.one, false);
        // AddQuad(new Vector3(-0.5f, -0.5f, depth), new Vector3(0.5f, -0.5f, depth), Vector3.up, Vector3.forward, Vector2.zero, Vector2.one, true);

        for (int y = 0; y < m_Height; y++) // bottom to top
        {
            for (int x = 0; x < m_Width; x++) // left to right
            {
                if (HasPixel(x, y))
                {
                    if (x == 0 || !HasPixel(x - 1, y))
                        AddEdge(x, y, Edge.left);

                    if (x == m_Width - 1 || !HasPixel(x + 1, y))
                        AddEdge(x, y, Edge.right);

                    if (y == 0 || !HasPixel(x, y - 1))
                        AddEdge(x, y, Edge.bottom);

                    if (y == m_Height - 1 || !HasPixel(x, y + 1))
                        AddEdge(x, y, Edge.top);
                }
            }
        }

        var mesh = new Mesh();

        if (m_Vertices.Count < 65000)
        {

            mesh.vertices = m_Vertices.ToArray();
            mesh.normals = m_Normals.ToArray();

            int[] quads = new int[m_Vertices.Count];
            for (int i = 0; i < quads.Length; i++)
                quads[i] = i;
            mesh.SetIndices(quads, MeshTopology.Quads, 0);
            //GetComponent<MeshFilter>().sharedMesh = mesh;
            mesh.SetTriangles(mesh.GetTriangles(0), 0);

            mc.sharedMesh = mesh;

            }
            else
            {
                print("TOO MANY VERTS");
            }
        if (m_Vertices.Count < 3)
        {
            sprite.enabled = false;
        }
        else {
            sprite.enabled = true;
        }
        called--;
        }
        
    }

}
