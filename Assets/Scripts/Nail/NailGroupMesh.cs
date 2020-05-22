using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NailGroupMesh : MonoBehaviour
{
    public Mesh mesh = null;

    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector3> normals;
    private List<Vector2> uv;
    private List<Color> colors;

    private float edgePer1 = 0.8f;
    private float edgePer2 = 1.0f;
    private Color edgeColor = Color.white;

    // メッシュ作成
    public void CreateMesh(NailGroup g, int[] data)
    {
        // メッシュの初期化
        // if (mesh == null) {
            mesh = new Mesh();
        //     meshFilter.mesh = mesh;
        // }

        vertices = new List<Vector3>();
        triangles = new List<int>();
        normals = new List<Vector3>();
        uv = new List<Vector2>();
        colors = new List<Color>();

        vertices.Add(Vector3.zero);
        normals.Add(new Vector3(0, 0, -1));
        colors.Add(Color.white);
        uv.Add(Vector2.one / 2);

        // float化
        var data2 = data.Select(v => (float)v).ToArray();
        data2 = SmoothVertex(data2);
        // data2 = SmoothVertex(data2);

        CreateFan(g, data2);
        CreateEdge(g, data2);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        mesh.colors = colors.ToArray();
    }

    // 中心の円を作る
    private void CreateFan(NailGroup g, float[] data)
    {
        // 座標への倍率
        var mx = edgePer1 / (float)g.orgTexWidth;
        var my = edgePer1 / (float)g.orgTexHeight;

        for (int i = 0; i < data.Length; i += 2) {
            var n = vertices.Count;
            var n1 = i;
            var n2 = (i + 2) % data.Length;
            var fx1 = (data[n1 + 0] - g.cx) * mx;
            var fy1 = (data[n1 + 1] - g.cy) * my;
            var fz1 = 0f;
            var fx2 = (data[n2 + 0] - g.cx) * mx;
            var fy2 = (data[n2 + 1] - g.cy) * my;
            var fz2 = 0f;

            // 頂点座標
            // vertices.Add(Vector3.zero);
            vertices.Add(new Vector3(fx1 * g.aspect, -fy1, fz1));
            vertices.Add(new Vector3(fx2 * g.aspect, -fy2, fz2));

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
                (data[n1 + 0] - (float)g.minX) / (float)g.texWidth * edgePer1,
                (1 - (data[n1 + 1] - (float)g.minY) / (float)g.texHeight) * edgePer1));
            uv.Add(new Vector2(
                (data[n2 + 0] - (float)g.minX) / (float)g.texWidth * edgePer1,
                (1 - (data[n2 + 1] - (float)g.minY) / (float)g.texHeight) * edgePer1));

            // 追加
            triangles.Add(0);
            triangles.Add(n + 0);
            triangles.Add(n + 1);
            triangles.Add(0);
            triangles.Add(n + 1);
            triangles.Add(n + 0);
        }
    }

    // 中心の端の消える部分を作成する
    private void CreateEdge(NailGroup g, float[] data)
    {
        // 座標への倍率
        var mx1 = edgePer1 / (float)g.orgTexWidth;
        var my1 = edgePer1 / (float)g.orgTexHeight;
        var mx2 = edgePer2 / (float)g.orgTexWidth;
        var my2 = edgePer2 / (float)g.orgTexHeight;

        for (int i = 0; i < data.Length; i += 2) {
            var n = vertices.Count;
            var n1 = i;
            var n2 = (i + 2) % data.Length;
            var fx1 = (data[n1 + 0] - g.cx) * mx1;
            var fy1 = (data[n1 + 1] - g.cy) * my1;
            var fz1 = 0f;
            var fx2 = (data[n2 + 0] - g.cx) * mx1;
            var fy2 = (data[n2 + 1] - g.cy) * my1;
            var fz2 = 0f;

            var fx3 = (data[n1 + 0] - g.cx) * mx2;
            var fy3 = (data[n1 + 1] - g.cy) * my2;
            var fz3 = 0f;
            var fx4 = (data[n2 + 0] - g.cx) * mx2;
            var fy4 = (data[n2 + 1] - g.cy) * my2;
            var fz4 = 0f;

            // 頂点座標
            // vertices.Add(Vector3.zero);
            vertices.Add(new Vector3(fx1 * g.aspect, -fy1, fz1));
            vertices.Add(new Vector3(fx2 * g.aspect, -fy2, fz2));
            vertices.Add(new Vector3(fx3 * g.aspect, -fy3, fz3));
            vertices.Add(new Vector3(fx4 * g.aspect, -fy4, fz4));

            // 法線
            var nm1 = (vertices[n + 0] - vertices[0]).normalized;
            var nm2 = (vertices[n + 1] - vertices[0]).normalized;
            var nm3 = (vertices[n + 2] - vertices[0]).normalized;
            var nm4 = (vertices[n + 3] - vertices[0]).normalized;
            // nm1.x *= 5;
            // nm2.x *= 5;
            var r = 45f / 180f * Mathf.PI;
            nm1.x *= 5 * Mathf.Cos(r);
            nm1.y *= 5 * Mathf.Sin(r);
            nm2.x *= 5 * Mathf.Cos(r);
            nm2.y *= 5 * Mathf.Sin(r);
            nm1.z = -1;
            nm2.z = -1;
            nm3.x *= 5 * Mathf.Cos(r);
            nm3.y *= 5 * Mathf.Sin(r);
            nm4.x *= 5 * Mathf.Cos(r);
            nm4.y *= 5 * Mathf.Sin(r);
            nm3.z = -1;
            nm4.z = -1;
            normals.Add(nm1.normalized);
            normals.Add(nm2.normalized);
            normals.Add(nm3.normalized);
            normals.Add(nm4.normalized);

            // 頂点カラー
            for (int j = 0; j < 4; j++) {
                colors.Add(j < 2 ? Color.white : edgeColor);
            }

            // テクスチャのUV
            uv.Add(new Vector2(
                (data[n1 + 0] - (float)g.minX) / (float)g.texWidth * edgePer1,
                (1 - (data[n1 + 1] - (float)g.minY) / (float)g.texHeight) * edgePer1));
            uv.Add(new Vector2(
                (data[n2 + 0] - (float)g.minX) / (float)g.texWidth * edgePer1,
                (1 - (data[n2 + 1] - (float)g.minY) / (float)g.texHeight) * edgePer1));
            uv.Add(new Vector2(
                (data[n1 + 0] - (float)g.minX) / (float)g.texWidth * edgePer2,
                (1 - (data[n1 + 1] - (float)g.minY) / (float)g.texHeight) * edgePer2));
            uv.Add(new Vector2(
                (data[n2 + 0] - (float)g.minX) / (float)g.texWidth * edgePer2,
                (1 - (data[n2 + 1] - (float)g.minY) / (float)g.texHeight) * edgePer2));

            // 追加
            triangles.Add(n + 0);
            triangles.Add(n + 1);
            triangles.Add(n + 2);
            triangles.Add(n + 0);
            triangles.Add(n + 2);
            triangles.Add(n + 1);

            triangles.Add(n + 1);
            triangles.Add(n + 2);
            triangles.Add(n + 3);
            triangles.Add(n + 1);
            triangles.Add(n + 3);
            triangles.Add(n + 2);
        }
    }

    // 滑らかにする
    private float[] SmoothVertex(float[] data)
    {
        // 個数が足りない、あるいは個数が奇数個
        if (data.Length < 3 * 2 || data.Length % 2 > 0) {
            return data;
        }

        var res = new float[data.Length];
        for (int i = 0; i < data.Length; i += 2) {
            var n0 = (i + data.Length - 2) % data.Length;
            var n1 = i;
            var n2 = (i + 2) % data.Length;

            for (int j = 0; j < 2; j++) {
                // 前後の中点を出す
                var c = (data[n2 + j] + data[n0 + j]) / 2;
                // 中点と現在位置の中間を取得
                res[n1 + j] = (data[n1 + j] + c) / 2;
            }
        }
        return res;
    }
}
