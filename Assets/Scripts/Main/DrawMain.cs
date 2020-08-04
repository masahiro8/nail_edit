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
    public bool useQuant;
    public bool useDelayCamera = true;
    public RectTransform canvasRect;
    // [SerializeField] Button camButton = null;
    // [SerializeField] Button shutterButton = null;
    // [SerializeField] Button flashButton = null;
    // [SerializeField] RectTransform cameraRectTransform = null;
    [SerializeField] RawImage cameraView = null;
    public RawImage delayView = null;
    public RawImage renderTexView = null;
    public AspectRatioFitter[] previewAspectFitters;

    public DrawDebug drawDebug = null;
    [SerializeField] public NailProcessing nailProcessing = null;
#if !NAIL_EDIT
    public ShapeDetection shapeDetection = null;
    public ObjectDetection objectDetection = null;
    public NailDetection nailDetection = null;
    public NailDetectionQuant nailDetectionQuant = null;
    public MainMiniCam mainMiniCam;
    public FaceDetection faceDetection;
#endif

    public RectTransform frameRect;
    // public Button colorSelectButton;
    public GameObject colorSelectView;
    public MainMenuList mainMenuList;
    public RenderScreenShot renderScreenShot;

    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    int webcamIndex = 0;
    List<int> deviceIndexes = new List<int>();
    List<WebCamTexture> webcamTextures = new List<WebCamTexture>();

    bool isProcessing = false;
    private Texture2D backupTexture = null;
    // private Texture2D backup2Texture = null;
    private bool nailCheckModeStop = false;

#if !NAIL_EDIT
    IClock clock;
    IMediaRecorder recorder;
    CameraInput cameraInput;
    AudioInput audioInput;
    private TextureToTensor tex2tensor = new TextureToTensor();
    private TextureToTensor.ResizeOptions resizeOptions = new TextureToTensor.ResizeOptions()
    {
        aspectMode = TextureToTensor.AspectMode.None,
        rotationDegree = 0,
        flipX = false,
        flipY = false,
        width = 1,
        height = 1,
        offsetX = 0,
        offsetY = 0,
        scaleX = 1,
        scaleY = 1,
    };
#endif

    void Start()
    {
        // Init camera
#if !NAIL_EDIT
        // NatDeviceと同時に起動させると実機で落ちる
        for (var i = 0; i < WebCamTexture.devices.Length; i++) {
            var device = WebCamTexture.devices[i];
            Debug.Log("WebCamTexture.devices " + device.name + ": " + device.kind + "," + device.isFrontFacing + "," + device.isAutoFocusPointSupported);
            if (!SROptions.Current.UseCameraNatDevice) {
                if (device.kind == WebCamKind.WideAngle) {
                    var webcamTexture = new WebCamTexture(device.name, 1024, 1024);
                    Debug.Log(device.name + ": " + webcamTexture.width + "," + webcamTexture.height);
                    // webcamTexture = new WebCamTexture(device.name, webcamTexture.width / 2, webcamTexture.height / 2, 30);
                    webcamTextures.Add(webcamTexture);
                    deviceIndexes.Add(i);
                }
            }
        }
#endif
        if (webcamTextures.Count > 0) {
            webcamIndex = webcamTextures.Count - 1;
        }
        ChangeCamera();

        // // 0.2秒ごとに処理
        // var time0 = System.TimeSpan.FromMilliseconds(0);
        // var time1 = System.TimeSpan.FromMilliseconds(200);
        // Observable.Timer(time0, time1)
        //     .Subscribe(_ => Invoke())
        //     .AddTo(gameObject);

        cameraView.gameObject.SetActive(true);
        delayView.gameObject.SetActive(useDelayCamera);
        delayView.material = new Material(delayView.material);
        renderTexView.material = delayView.material;
    }

    void Update()
    {
        if (DataTable.Param.useDummyImage) {
            // delayView.gameObject.SetActive(false);
            cameraView.texture = Resources.Load(DebugPhoto.Instance.PhotoFileName) as Texture2D;
            // renderTexView.texture = cameraView.texture;
            foreach (var previewAspectFitter in previewAspectFitters) {
                previewAspectFitter.aspectRatio = (float)cameraView.texture.width / (float)cameraView.texture.height;
            }
        } else if (webcamTextures.Count > 0) {
#if !NAIL_EDIT
            var webcam = webcamTextures[webcamIndex];
            var device = WebCamTexture.devices[deviceIndexes[webcamIndex]];
            var angle = webcam.videoRotationAngle + SROptions.Current.CameraDegreeOffset;
            var rflag = angle % 180 == 0;
            var w = rflag ? webcam.width : webcam.height;
            var h = rflag ? webcam.height : webcam.width;
            resizeOptions.width = w;
            resizeOptions.height = h;
            resizeOptions.rotationDegree = angle;
            // resizeOptions.flipX = device.isFrontFacing;
            // resizeOptions.flipY = webcam.videoVerticallyMirrored;
            resizeOptions.flipX = true;
            resizeOptions.flipY = device.isFrontFacing
                ? webcam.videoVerticallyMirrored
                : !webcam.videoVerticallyMirrored;
            cameraView.texture = tex2tensor.Resize(webcam, resizeOptions);
            foreach (var previewAspectFitter in previewAspectFitters) {
                previewAspectFitter.aspectRatio = (float)w / (float)h;
            }
#endif
        }

        // カメラの射影サイズを変更して3D表示の大きさをUIと一致させる
        // Camera.main.orthographicSize = 0.5f * sizeCvs.y / cameraRectTransform.sizeDelta.y;

#if !NAIL_EDIT
        // mainMiniCam.previewPanel.texture = new Texture2D(1080, 1920);
        drawDebug.texView1[0].texture = mainMiniCam.previewPanel.texture;
        Invoke();
        // Observable.Start(() => {}) // 別スレッドで開始
        //     .ObserveOn(Scheduler.ThreadPool) // ここからはスレッドプールで処理
        //     .Subscribe(_ => Invoke())
        //     .AddTo(this);
#endif

        var sd = Mathf.Min(
            delayView.rectTransform.rect.width,
            delayView.rectTransform.rect.height);
        // Debug.Log("sd: " + sd + ", (" + delayView.rectTransform.rect.width + ", " + delayView.rectTransform.rect.height + ")");
        frameRect.sizeDelta = Vector2.one * sd;
        frameRect.anchoredPosition = new Vector2(0, sd / 8f);
    }

    void OnDestroy()
    {
        foreach (var webcamTexture in webcamTextures) {
            webcamTexture.Stop();
        }
    }

#if !NAIL_EDIT
    void Invoke()
    {
        var doInvoke = !isProcessing;
        var setDelayImmediately = false;

        // ネイルチェックモードでは停止させる
        if (SROptions.Current.NailCheckMode) {
            if (delayView.gameObject.activeSelf != nailCheckModeStop) {
                delayView.gameObject.SetActive(nailCheckModeStop);
                setDelayImmediately = true;
                doInvoke = doInvoke && nailCheckModeStop;
                // nailDetectionTest1.dot2Area.result = new NailDotToArea.Data1[0];
                if (!nailCheckModeStop) {
                    nailDetection.ioDataAll.dot2Area.Clear();
                    nailProcessing.Invoke(nailDetection, null);
                }
            } else {
                doInvoke = false;
            }

            if (delayView.texture) {
                // nailDetection.Invoke(delayView.texture, nailDetectionTest1.dot2Area);
            }
            // doInvoke = doInvoke && !nailCheckModeStop;
        }

        if (DataTable.Param.useDummyDetection) {
            doInvoke = false;
        }

        if (doInvoke) {
            isProcessing = true;
            float startTime = Time.realtimeSinceStartup;

            var useTex = SROptions.Current.UseCameraNatDevice
                ? mainMiniCam.previewPanel.texture
                : cameraView.texture;
            if (useTex) {
                Texture2D tmpTexture = null;
                if (useDelayCamera) {
                    var camTex = useTex as Texture2D;
                    if (camTex) {
                        // Debug.Log("tex: " + camTex.width + "," + camTex.height);
                        tmpTexture = new Texture2D(camTex.width, camTex.height);
                        tmpTexture.SetPixels(camTex.GetPixels());
                        tmpTexture.Apply();
                    }
                    var renderTexture = useTex as RenderTexture;
                    if (renderTexture) {
                        var prevRT = RenderTexture.active;
                        RenderTexture.active = renderTexture;
                        if (tmpTexture != null) {
                            Destroy(tmpTexture);
                        }
                        tmpTexture = new Texture2D(renderTexture.width, renderTexture.height);
                        tmpTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                        tmpTexture.Apply();
                        RenderTexture.active = prevRT;
                    }
                }

                if (tmpTexture) {
                    if (setDelayImmediately) {
                        if (delayView.texture != null) {
                            Destroy(delayView.texture);
                        }
                        delayView.texture = tmpTexture;
                        delayView.material.SetTexture("MainTexture", tmpTexture);
                        renderTexView.texture = tmpTexture;
                        // backupTexture = tmpTexture;
                    }
                    // 物体検出
                    objectDetection.Invoke(tmpTexture, () => {
                        AsyncInvoke(tmpTexture, setDelayImmediately);
                    });
                } else {
                    isProcessing = false;
                }

                // if (camTex) {
                // }
// #endif
            } else {
                isProcessing = false;
            }

            DebugDisp();
            // if (webcamTextures.Count == 0) {
            //     isProcessing = false;
            // }
        }

        if (DataTable.Param.useDummyDetection) {
            nailProcessing.Invoke2(objectDetection, drawDebug.texView1[0].texture);
        }
    }

    // void AsyncInvoke(Texture2D tmpTexture, int index)
    void AsyncInvoke(Texture2D tmpTexture, bool setDelayImmediately)
    {
        // foreach (var v in nailDetectionTest2.dot2Area.result) {
        //     v.Range = tmpTexture.AdjustClipping(v.RangeOrg);
        // }

        var textures = objectDetection.line2Area.result
            .Select(v => tmpTexture.Clipping(v.Range))
            // .Select(v => tmpTexture.Clipping(new float[]{0, 0, 1, 1}))
            .ToArray();

        var ranges = objectDetection.line2Area.result
            .Select(v => v.Range)
            .ToArray();

        if (textures.Length == 0) {
            isProcessing = false;
            return;
        }

        shapeDetection.Invoke(tmpTexture, shapeType => {
            if (drawDebug.shapeTextView) {
                if (SROptions.Current.DispShapeTextView) {
                    // string[] strs = { "Goo", "Paa", "Foot", "Foot" };
                    // shapeTextView.text = "Shape: " + strs[shapeType] + shapeType;
                    // string[] strs = { "g", "p", ".", ".." };
                    // string[] strs = { "f", "fw", "g", "p", };
                    // string[] strs = { "fd", "fm", "gd", "gm", "pd", "pm" };
                    // drawDebug.shapeTextView.text = strs[shapeType];
                    drawDebug.shapeTextView.text = ShootModeType.Free.GetName(shapeType);
                } else {
                    drawDebug.shapeTextView.text = "";
                }
            }
            if (useQuant) {
                // 量子化
                for (var i = 0; i < nailDetectionQuant.ioData.Length; i++) {
                    drawDebug.texView3[i].texture = nailDetectionQuant.ioData[i].tex2Texture[0];
                    drawDebug.texView3[i].transform.GetChild(0).GetComponent<RawImage>().texture = nailDetectionQuant.ioData[i].tex2Texture[1];
                }

                nailDetectionQuant.Invoke(textures, ranges, () => {
                    foreach (var tex in textures) {
                        Destroy(tex);
                    }
                    if (!setDelayImmediately) {
                        Destroy(delayView.texture);
                    }
                    delayView.texture = tmpTexture;
                    renderTexView.texture = tmpTexture;
                    nailProcessing.Invoke3(nailDetectionQuant, tmpTexture);
                    isProcessing = false;
                });
            } else {
                // 通常
                nailDetection.ChangeFile(shapeType);
                for (var i = 0; i < nailDetection.ioData.Length; i++) {
                    drawDebug.texView3[i].texture = nailDetection.ioData[i].tex2Texture[0];
                    drawDebug.texView3[i].transform.GetChild(0).GetComponent<RawImage>().texture = nailDetection.ioData[i].tex2Texture[1];
                }

                nailDetection.Invoke(textures, ranges, () => {
                    foreach (var tex in textures) {
                        Destroy(tex);
                    }
                    if (!setDelayImmediately) {
                        Destroy(delayView.texture);
                    }
                    delayView.texture = tmpTexture;
                    delayView.material.SetTexture("MainTexture", tmpTexture);
                    renderTexView.texture = tmpTexture;
                    renderTexView.material.SetTexture("MainTexture", tmpTexture);
                    nailProcessing.Invoke(nailDetection, tmpTexture);
                    isProcessing = false;
                });
            }
        });
    }
#endif

    void ChangeCamera()
    {
#if !NAIL_EDIT
        if (SROptions.Current.UseCameraNatDevice) {
            mainMiniCam.SwitchCamera();
            return;
        }
#endif

        DebugPhoto.Instance.AddIndex();
        if (DataTable.Param.useDummyImage) {
            return;
        }
        if (webcamTextures[webcamIndex].isPlaying) {
            webcamTextures[webcamIndex].Stop();
        }
        webcamIndex++;
        webcamIndex %= webcamTextures.Count;
        cameraView.texture = webcamTextures[webcamIndex];
        webcamTextures[webcamIndex].Play();
        Debug.Log("Change: " + webcamTextures[webcamIndex].name + " (" + webcamIndex + ") -> " + webcamTextures[webcamIndex].width + "," + webcamTextures[webcamIndex].height);
        drawDebug.texView1[0].texture = webcamTextures[webcamIndex];
    }

    public void ChangeStopMode(bool flag)
    {
        if (!isProcessing) {
            nailCheckModeStop = flag;
        }
#if NAIL_EDIT
        nailProcessing.Invoke2(cameraView.texture);
#endif
    }

    public bool IsProcessing
    {
        get { return isProcessing; }
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
        // sb.Append("canvas2:").AppendLine(cameraRectTransform.sizeDelta.ToString());
        sb.AppendLine("");

        // sb.AppendLine($"index = {index}, {maxValue}");
        // for (int i = 0; i < 4; i++)
        // {
        //     sb.AppendLine($"{i}: {outputs[index, i]: 0.00}");
        // }

        if (drawDebug.outputTextView) {
            drawDebug.outputTextView.text = sb.ToString();
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

    public void AttachButtons(MainBaseView view)
    {
        if (view.shutterButton) {
            view.shutterButton.OnClickAsObservable()
                .SubscribeOnMainThread()
                .Subscribe(_ => {
                    ChangeStopMode(!nailCheckModeStop);
                })
                .AddTo(view.gameObject);
        }

        // ボタン押し
        if (view.changeCameraButton) {
// #if !NAIL_EDIT
            view.changeCameraButton.gameObject.SetActive(DataTable.Param.useDummyImage);
// #endif
            view.changeCameraButton.OnClickAsObservable()
                .Subscribe(_ => ChangeCamera())
                .AddTo(view.gameObject);
        }

        // ボタン押し
        if (view.flashButton) {
            view.flashButton.OnClickAsObservable()
                .Subscribe(_ => {
                    SaveName.NailMeshReverse.ToggleBool();
#if !NAIL_EDIT
                    mainMiniCam.ToggleFlashMode();
#endif
                })
                .AddTo(view.gameObject);
        }

        if (view.colorSelectButton) {
            view.colorSelectButton.OnClickAsObservable()
                .SubscribeOnMainThread()
                .Subscribe(_ => {
                    var colorSelect = Instantiate(colorSelectView, canvasRect).GetComponent<ColorSelectList>();
                    colorSelect.main = this;
                })
                .AddTo(view.gameObject);
        }

        if (view.menuButton) {
            view.menuButton.OnClickAsObservable()
                .SubscribeOnMainThread()
                .Subscribe(_ => {
                    mainMenuList.OpenMenu();
                })
                .AddTo(view.gameObject);
        }
    }
}
