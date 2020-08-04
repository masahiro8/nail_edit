using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using VoxelBusters.NativePlugins;

public class RenderScreenShot : MonoBehaviour {

    public Camera targetCamera;
    public Button shutterButton;
    public RawImage debugView;
    public AudioSource shutterSE;

    private RenderTexture renderTex;
    private Texture2D outputTex;

    private int count = -1;
    // private float timer = 0;

    // Use this for initialization
    void Start() {
        // int renderHeight = GlobalParam.GetMinimumPower2(Screen.height);
        renderTex = new RenderTexture(
                Screen.width,
                Screen.height,
                0);
        outputTex = new Texture2D(
                Screen.width,
                Screen.height,
                TextureFormat.RGBA32,
                false);
        if (targetCamera) {
            // targetCamera.enabled = true;
            targetCamera.targetTexture = renderTex;
        }
        debugView.texture = renderTex;

        if (shutterButton) {
            shutterButton.OnClickAsObservable()
                .Where(_ => !SROptions.Current.NailCheckMode)
                .Subscribe(_ => {
                    SavePhoto();
                })
                .AddTo(gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        if (count == 1) {
            UpdateTexture();
            targetCamera.enabled = false;
            SavePhotoImage();
            count = -1;
        }
        if (count >= 0) {
            count++;
        }
    }

    void UpdateTexture() {
        //Debug.Log (renderer);
        // RenderTexture currentActiveRT = RenderTexture.active;
        targetCamera.Render();
        RenderTexture.active = renderTex;
        outputTex.ReadPixels(new Rect(0.0f, 0.0f, renderTex.width, renderTex.height), 0, 0);
        outputTex.Apply();
        RenderTexture.active = null;
    }

    // 保存する
    public void SavePhoto()
    {
        count = 0;
        targetCamera.enabled = true;
        shutterSE.Play();
    }

    void SavePhotoImage()
    {
        // var tex = Resources.Load<Texture2D>("Test/nail_k1_66");
        var album = "Gallery/NailHolic";
        var filename = "NailHolicTest.png";
#if UNITY_EDITOR
        System.IO.File.WriteAllBytes(filename, outputTex.EncodeToPNG());
#else
        NativeGallery.SaveImageToGallery(
            //string existingMediaPath,
            outputTex,
            //string album,
            album,
            //string filename,
            filename);
#endif
        Debug.Log("SavePhotoImage: " + album + "/" + filename);
    }

    // シェアする
    public void SharePhoto()
    {
        // Create new instance and populate fields
        ShareSheet _shareSheet = new ShareSheet(); 
        // _shareSheet.Text = "Share message.";
        _shareSheet.AttachImage(outputTex);
        // _shareSheet.ExcludedShareOptions = ;
        // On iPad, popover view is used to show share sheet. So we need to set its position
        NPBinding.UI.SetPopoverPointAtLastTouchPosition();
        // Show composer
        NPBinding.Sharing.ShowView(_shareSheet, OnFinishedSharing);
    }

    private void OnFinishedSharing(eShareResult _result)
    {
        // Insert your code
        Debug.Log("OnFinishedSharing");
    }
}
