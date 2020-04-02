using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NailGroup : MonoBehaviour
{
    public Mesh mesh = null;

    // メッシュ作成
    public void CreateMesh(int[] data)
    {
        // メッシュの初期化
        // if (mesh == null) {
            mesh = new Mesh();
        //     meshFilter.mesh = mesh;
        // }

        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var normals = new List<Vector3>();
        var uv = new List<Vector2>();
        var colors = new List<Color>();

        vertices.Add(Vector3.zero);
        normals.Add(new Vector3(0, 0, -1));
        colors.Add(Color.white);
        uv.Add(Vector2.one / 2);

        // Debug.Log("data.Length: " + data.Length);
        for (int i = 0; i < data.Length; i += 2) {
            var n = vertices.Count;
            var n1 = i;
            var n2 = (i + 2) % data.Length;
            var fx1 = ((float)data[n1 + 0] - cx) / (float)orgTexWidth;
            var fy1 = ((float)data[n1 + 1] - cy) / (float)orgTexHeight;
            var fz1 = 0f;
            var fx2 = ((float)data[n2 + 0] - cx) / (float)orgTexWidth;
            var fy2 = ((float)data[n2 + 1] - cy) / (float)orgTexHeight;
            var fz2 = 0f;

            // 頂点座標
            // vertices.Add(Vector3.zero);
            vertices.Add(new Vector3(fx1 * aspect, -fy1, fz1));
            vertices.Add(new Vector3(fx2 * aspect, -fy2, fz2));

            // 法線
            var nm1 = (vertices[n + 0] - vertices[0]).normalized;
            var nm2 = (vertices[n + 1] - vertices[0]).normalized;
            // nm1.x *= 5;
            // nm2.x *= 5;
            var r = 45f / 180f * Mathf.PI;
            nm1.x *= 5 * Mathf.Cos(r);
            nm1.y *= 5 * Mathf.Sin(r);
            nm2.x *= 5 * Mathf.Cos(r);
            nm2.y *= 5 * Mathf.Sin(r);
            nm1.z = -1;
            nm2.z = -1;
            normals.Add(nm1.normalized);
            normals.Add(nm2.normalized);

            // 頂点カラー
            for (int j = 0; j < 2; j++) {
                colors.Add(Color.white);
            }

            // テクスチャのUV
            uv.Add(new Vector2(
                (float)(data[n1 + 0] - minX) / (float)texWidth,
                1 - (float)(data[n1 + 1] - minY) / (float)texHeight));
            uv.Add(new Vector2(
                (float)(data[n2 + 0] - minX) / (float)texWidth,
                1 - (float)(data[n2 + 1] - minY) / (float)texHeight));

            // 追加
            triangles.Add(0);
            triangles.Add(n + 0);
            triangles.Add(n + 1);
            triangles.Add(0);
            triangles.Add(n + 1);
            triangles.Add(n + 0);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        // mesh.colors = colors.ToArray();

        // meshFilter.sharedMesh = mesh;
        // meshRenderer.sharedMaterial = material;
        // meshFilter.mesh = mesh;
    }
}
