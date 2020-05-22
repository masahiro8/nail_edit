using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailGroupTexture : MonoBehaviour
{
    public Texture2D lightTexture = null;

    // テクスチャを更新
    public void UpdateTexture(NailGroup g, int[] data, Texture orgTexture)
    {
        // テクスチャ作り直し
        if (lightTexture != null) {
            Object.Destroy(lightTexture);
        }

        // 元のテクスチャから範囲を切り出し
        var webCamTexture = orgTexture as WebCamTexture;
        if (webCamTexture) {
            // var pixels = webCamTexture.GetPixels(minX, minY, texWidth, texHeight);
            var x = g.minX * webCamTexture.width / g.orgTexWidth;
            var y = g.minY * webCamTexture.height / g.orgTexHeight;
            var w = g.texWidth * webCamTexture.width / g.orgTexWidth;
            var h = g.texHeight * webCamTexture.height / g.orgTexHeight;
            lightTexture = new Texture2D(w, h);
            var pixels = webCamTexture.GetPixels(x, y, w, h);
            lightTexture.SetPixels(pixels);
            lightTexture.Apply();
            // Debug.Log(minX + "," + minY + " -> " + x + "," + y);
            // Debug.Log(texWidth + "," + texHeight + " -> " + w + "," + h);
        }
        var renderTexture = orgTexture as RenderTexture;
        if (renderTexture) {
            var prevRT = RenderTexture.active;
            RenderTexture.active = renderTexture;

            lightTexture.ReadPixels(new Rect(g.minX, g.minY, g.texWidth, g.texHeight), 0, 0);
            lightTexture.Apply();

            RenderTexture.active = prevRT;
        }
        var texture2d = orgTexture as Texture2D;
        if (texture2d) {
            // var pixels = texture2d.GetPixels(minX, minY, texWidth, texHeight);
            // var pixels = texture2d.GetPixels(orgTexWidth - maxX, orgTexHeight - maxY, texWidth, texHeight);
            // Power of 2で1,024x1,024になるとうまくいかないので注意
            var pixels = texture2d.GetPixels(g.minX, g.orgTexHeight - 1 - g.maxY, g.texWidth, g.texHeight);
            lightTexture = new Texture2D(g.texWidth, g.texHeight);
            for (int i = 0; i < pixels.Length; i++) {
                float h, s, v;
                Color.RGBToHSV(pixels[i], out h, out s, out v);
                h = 0;
                s = 0;
                v = Mathf.Max(0, v * 10 - 9);
                // pixels[i] = Color.HSVToRGB(h, s, v);
                // 輝度部分だけを取得
                pixels[i] = new Color(1, 1, 1, v);
            }
            lightTexture.SetPixels(pixels);
            lightTexture.Apply();
        }
    }
}
