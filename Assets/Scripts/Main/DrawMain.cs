#define NAIL_EDIT

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

#if !NAIL_EDIT
using TensorFlowLite;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
#endif

public class DrawMain : MonoBehaviour
{
    public bool useWebCamera;
    public RectTransform canvasRect;
    [SerializeField] Text outputTextView = null;
    [SerializeField] Button camButton = null;
    [SerializeField] Button recButton = null;
    [SerializeField] RectTransform cameraRectTransform = null;
    [SerializeField] RawImage cameraView = null;
    [SerializeField] RawImage texView = null;

#if !NAIL_EDIT
    [SerializeField] PalmDetection palmDetection = null;
    [SerializeField] HandLandmark handLandmark = null;
#endif
    [SerializeField] public NailDetection nailDetection = null;
#if !NAIL_EDIT
    [SerializeField] public NailDetectionTest nailDetectionTest = null;
#endif

    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    int webcamIndex = 0;
    List<int> deviceIndexes = new List<int>();
    List<WebCamTexture> webcamTextures = new List<WebCamTexture>();

    bool isProcessing = false;

#if !NAIL_EDIT
    IClock clock;
    IMediaRecorder recorder;
    CameraInput cameraInput;
    AudioInput audioInput;
#endif

    void Start()
    {
        // Init camera
        if (useWebCamera) {
            for (var i = 0; i < WebCamTexture.devices.Length; i++) {
                var device = WebCamTexture.devices[i];
                if (device.kind == WebCamKind.WideAngle) {
                    var webcamTexture = new WebCamTexture(device.name);
                    Debug.Log(device.name + ": " + webcamTexture.width + "," + webcamTexture.height);
                    // webcamTexture = new WebCamTexture(device.name, webcamTexture.width / 2, webcamTexture.height / 2, 30);
                    webcamTextures.Add(webcamTexture);
                    deviceIndexes.Add(i);
                }
            }
        }
        if (webcamTextures.Count > 0) {
            webcamIndex = webcamTextures.Count - 1;
        }
        ChangeCamera();

        // ボタン押し
        if (camButton) {
            camButton.OnClickAsObservable()
                .Subscribe(_ => ChangeCamera())
                .AddTo(gameObject);
        }

#if !NAIL_EDIT
        if (recButton) {
            recButton.OnClickAsObservable()
                .SubscribeOnMainThread()
                .Subscribe(_ => {
                    if (clock == null) {
                        StartRecording();
                    } else {
                        StopRecording();
                    }
                })
                .AddTo(gameObject);
        }
#endif

        // // 0.2秒ごとに処理
        // var time0 = System.TimeSpan.FromMilliseconds(0);
        // var time1 = System.TimeSpan.FromMilliseconds(200);
        // Observable.Timer(time0, time1)
        //     .Subscribe(_ => Invoke())
        //     .AddTo(gameObject);

#if !UNITY_EDITOR
        SROptions.Current.DispTextView
            .Subscribe(flag => outputTextView.enabled = flag);
        SROptions.Current.DispHandModel
            .Subscribe(flag => handLandmark.dotPoints2.pointFolder.parent.gameObject.SetActive(flag));
        SROptions.Current.DispOrgImage
            .Subscribe(flag => texView.enabled = flag);
        SROptions.Current.DispPalmImage
            .Subscribe(flag => palmDetection.texView.enabled = flag);
        SROptions.Current.DispHandImage
            .Subscribe(flag => handLandmark.texView.enabled = flag);
#endif
    }

    void Update()
    {
        var w = 1280f; // カメラを使わない場合用の初期値（エディタなど）
        var h = 960f;
        var angle = 0;
        if (webcamTextures.Count > 0) {
            var webcam = webcamTextures[webcamIndex];
            var device = WebCamTexture.devices[deviceIndexes[webcamIndex]];

            w = (float)webcam.width;
            h = (float)webcam.height;
            angle = webcam.videoRotationAngle;
            // angle = 90; // テスト用

            cameraRectTransform.localScale = new Vector3(
                device.isFrontFacing ? -1 : 1,
                webcam.videoVerticallyMirrored ? -1 : 1,
                1);
            cameraRectTransform.rotation = Quaternion.Euler(
                0,
                0,
                (device.isFrontFacing ? angle : -angle) + SROptions.Current.CameraDegreeOffset);
        } else {
            cameraRectTransform.localScale = new Vector3(
                -1,
                1,
                1);
        }
        var scale = SROptions.Current.CameraDispSizeScale + 1;
        var sizeCam = new Vector3(
            w,
            h,
            0);
        var sizeCvs = new Vector3(
            canvasRect.sizeDelta.x,
            canvasRect.sizeDelta.y,
            0);
        // sizeCam = Quaternion.Euler(0, 0, webcam.videoRotationAngle) * sizeCam;
        // sizeCam.x = Mathf.Abs(sizeCam.x);
        // sizeCam.y = Mathf.Abs(sizeCam.y);
        sizeCvs = Quaternion.Euler(0, 0, angle + SROptions.Current.CameraDegreeOffset) * sizeCvs;
        sizeCvs.x = Mathf.Abs(sizeCvs.x);
        sizeCvs.y = Mathf.Abs(sizeCvs.y);
        var aspect = sizeCam.y / sizeCam.x;
        // 480,640->1.33 1242,2688->2.1
        // 640,480->0.75 2688,1242->0.46
        // Debug.Log("aspect: " + sizeCam.x + "," + sizeCam.y + " -> " + aspect + " -> " + (canvasRect.sizeDelta.y / canvasRect.sizeDelta.x));
        if (useWebCamera) {
            if (aspect > sizeCvs.y / sizeCvs.x) {
                cameraRectTransform.sizeDelta = new Vector2(
                    sizeCvs.x,
                    sizeCvs.x * aspect) * scale;
            } else {
                cameraRectTransform.sizeDelta = new Vector2(
                    sizeCvs.y / aspect,
                    sizeCvs.y) * scale;
            }
        } else {
            if (aspect < sizeCvs.y / sizeCvs.x) {
                cameraRectTransform.sizeDelta = new Vector2(
                    sizeCvs.x,
                    sizeCvs.x / aspect) * scale;
            } else {
                cameraRectTransform.sizeDelta = new Vector2(
                    sizeCvs.y / aspect,
                    sizeCvs.y) * scale;
            }
        }
        // Debug.Log("Size: " + Screen.width + "," + cameraRectTransform.sizeDelta.x);
        // Debug.Log("Size: " + Screen.width + "," + Screen.height + " -> " + canvasRect.sizeDelta);
        // cameraRectTransform.sizeDelta = new Vector2(
        //     Screen.width,
        //     Screen.height);

        // カメラの射影サイズを変更して3D表示の大きさをUIと一致させる
        Camera.main.orthographicSize = 0.5f * sizeCvs.y / cameraRectTransform.sizeDelta.y;

        Invoke();
        // Observable.Start(() => {}) // 別スレッドで開始
        //     .ObserveOn(Scheduler.ThreadPool) // ここからはスレッドプールで処理
        //     .Subscribe(_ => Invoke())
        //     .AddTo(this);
    }

    void OnDestroy()
    {
        foreach (var webcamTexture in webcamTextures) {
            webcamTexture.Stop();
        }
    }

    void Invoke()
    {
        if (!isProcessing) {
            isProcessing = true;
            float startTime = Time.realtimeSinceStartup;

            if (webcamTextures.Count > 0) {
                var webcam = webcamTextures[webcamIndex];
                var device = WebCamTexture.devices[deviceIndexes[webcamIndex]];
                // palmDetection.Invoke(webcam, device);
                // handLandmark.Resize(webcam, palmDetection);
                // handLandmark.Invoke(webcam, device);
#if !NAIL_EDIT
                nailDetectionTest.Invoke(texView.texture, device, () => {
                    isProcessing = false;
                });
#endif
            }

            if (useWebCamera) {
#if !NAIL_EDIT
                nailDetection.Invoke(texView.texture, nailDetectionTest.dot2Area);
#endif
            } else {
                nailDetection.Invoke2(texView.texture);
            }

            if (webcamTextures.Count > 0) {
                // palmDetection.DebugTexture();
                // handLandmark.DebugTexture();
#if !NAIL_EDIT
                nailDetectionTest.DebugTexture();
#endif
            }
            DebugDisp();
            if (webcamTextures.Count == 0) {
                isProcessing = false;
            }
        }
    }

    void ChangeCamera()
    {
        DebugPhoto.Instance.AddIndex();
        cameraView.texture = Resources.Load(DebugPhoto.Instance.PhotoFileName) as Texture2D;
        if (webcamTextures.Count == 0) {
            return;
        }
        if (webcamTextures[webcamIndex].isPlaying) {
            webcamTextures[webcamIndex].Stop();
        }
        webcamIndex++;
        webcamIndex %= webcamTextures.Count;
        cameraView.texture = webcamTextures[webcamIndex];
        webcamTextures[webcamIndex].Play();
        Debug.Log("Change: " + webcamTextures[webcamIndex].name + " (" + webcamIndex + ") -> " + cameraView.texture.width + "," + cameraView.texture.height);
        texView.texture = cameraView.texture;
    }

    void DebugDisp()
    {
        if (webcamTextures.Count == 0) {
            return;
        }

        var webcam = webcamTextures[webcamIndex];
        var device = WebCamTexture.devices[deviceIndexes[webcamIndex]];

        sb.Clear();
        // sb.AppendLine($"Process time: {duration: 0.00000} sec");
        sb.AppendLine("---");

        // for (int i = 0; i < outputs.GetLength(1); i++)
        // {
        //     sb.AppendLine($"{i}: {outputs[index, i]: 0.00}");
        // }

        Vector2? autoFocusPoint = webcam.autoFocusPoint;
        sb.Append("size:").AppendLine(webcam.width + "," + webcam.height);
        sb.Append("autoFocusPoint:").AppendLine((autoFocusPoint==null ? Vector2.zero.ToString() : autoFocusPoint.ToString()));
        sb.Append("deviceName:").AppendLine(webcam.deviceName.ToString());
        sb.Append("didUpdateThisFrame:").AppendLine(webcam.didUpdateThisFrame.ToString());
        sb.Append("isDepth:").AppendLine(webcam.isDepth.ToString());
        sb.Append("isPlaying:").AppendLine(webcam.isPlaying.ToString());
        sb.Append("requestedFPS:").AppendLine(webcam.requestedFPS.ToString());
        sb.Append("requestedHeight:").AppendLine(webcam.requestedHeight.ToString());
        sb.Append("requestedWidth:").AppendLine(webcam.requestedWidth.ToString());
        sb.Append("videoRotationAngle:").AppendLine(webcam.videoRotationAngle.ToString());
        sb.Append("videoVerticallyMirrored:").AppendLine(webcam.videoVerticallyMirrored.ToString());
        sb.AppendLine("");

        sb.Append("isFrontFacing:").AppendLine(device.isFrontFacing.ToString());
        sb.AppendLine("");

        sb.Append("canvas:").AppendLine(canvasRect.sizeDelta.ToString());
        sb.Append("canvas2:").AppendLine(cameraRectTransform.sizeDelta.ToString());
        sb.AppendLine("");

        // sb.AppendLine($"index = {index}, {maxValue}");
        // for (int i = 0; i < 4; i++)
        // {
        //     sb.AppendLine($"{i}: {outputs[index, i]: 0.00}");
        // }

        if (outputTextView) {
            outputTextView.text = sb.ToString();
        }
    }

#if !NAIL_EDIT
    void StartRecording()
    {
        Debug.Log("StartRecording");
        // Create a recording clock
        clock = new RealtimeClock();
        // Start recording
        recorder = new MP4Recorder(640, 480, 2);
        // Create a camera input to record the main camera
        cameraInput = new CameraInput(recorder, clock, Camera.main);
        // Create an audio input to record the scene's AudioListener
        // audioInput = new AudioInput(recorder, clock, Camera.main.GetComponent<AudioListener>());
        audioInput = null;
    }

    async void StopRecording()
    {
        Debug.Log("StopRecording");
        clock = null;
        // Destroy the recording inputs
        cameraInput.Dispose();
        // audioInput.Dispose();
        // Stop recording
        // recorder.FinishWriting();
        var path = await recorder.FinishWriting();
        // Playback recording
        Debug.Log($"Saved recording to: {path}");
        recorder = null;
    	new NativeShare().AddFile(path).SetSubject("Subject goes here").SetText("Hello world!").Share();
    }
#endif

    void OpenMenu()
    {
        Debug.Log("OpenMenu");
    }
}
