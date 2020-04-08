using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using UniRx;

public class NailDetectionTest : MonoBehaviour
{
    public int inputWidth = 513;
    public int inputHeight = 513;
    public int outputWidth = 513;
    public int outputHeight = 513;
    [SerializeField] string fileName = "frozen_inference_graph.tflite";
    [SerializeField] ComputeShader compute = null;
    // [SerializeField] RawImage cameraView = null;
    [SerializeField] public RawImage texView;

    // [System.NonSerialized] float duration;

    protected Interpreter interpreter;
    protected TextureToTensor tex2tensor;
    protected TextureToTensor.ResizeOptions resizeOptions;

    protected byte[,,] inputs;
    protected System.Int64[,] outputs1;
    protected ComputeBuffer inputBuffer;
    protected Texture2D tex2Texture;

    public virtual void Start()
    {
        var options = new Interpreter.Options()
        {
            threads = 2,
            gpuDelegate = null,//CreateGpuDelegate(),
        };
        // fileName = "palm_detection.tflite";
        interpreter = new Interpreter(FileUtil.LoadFile(fileName), options);
        interpreter.LogIOInfo();
        InitInputs();

        inputs = new byte[inputWidth, inputHeight, 3];
        inputBuffer = new ComputeBuffer(inputWidth * inputHeight * 3, sizeof(byte));
        outputs1 = new System.Int64[outputWidth, outputHeight];

        tex2tensor = new TextureToTensor();
        resizeOptions = new TextureToTensor.ResizeOptions()
        {
            aspectMode = TextureToTensor.AspectMode.Fit,
            rotationDegree = 0,
            flipX = false,
            flipY = true,
            width = inputWidth,
            height = inputHeight,
            offsetX = 0,
            offsetY = 0,
            scaleX = 1,
            scaleY = 1,
        };

        tex2Texture = new Texture2D(
            inputWidth,
            inputHeight,
            TextureFormat.RGBA32,
            false);
        tex2Texture.filterMode = FilterMode.Point;
        texView.texture = tex2Texture;
    }

    void OnDestroy()
    {
        interpreter?.Dispose();
        inputBuffer?.Dispose();
    }

    public void Invoke(Texture webcam, WebCamDevice device)
    {
        float OFFSET = (float)inputWidth / 2;
        float SCALE = 1f / OFFSET;
        ToTensor(webcam, inputs, OFFSET, SCALE);

        interpreter.SetInputTensorData(0, inputs);
        interpreter.Invoke();
        interpreter.GetOutputTensorData(0, outputs1);

        // 結果
        var res = 0;
        for (int i = 0; i < outputs1.GetLength(0); i++) {
            for (int j = 0; j < outputs1.GetLength(1); j++) {
                if (outputs1[i, j] > 0) {
                    res++;
                }
            }
        }
        Debug.Log(res);
    }

    public void DebugTexture()
    {
        // テスト用にテクスチャを表示
        var buffer = tex2Texture.GetPixels();
        for (int i = 0; i < buffer.Length; i++) {
            var nx = i % inputWidth;
            var ny = i / inputHeight;
            buffer[i].r = (float)inputs[ny, nx, 0] / 255f;
            buffer[i].g = (float)inputs[ny, nx, 1] / 255f;
            buffer[i].b = (float)inputs[ny, nx, 2] / 255f;
        }
		tex2Texture.SetPixels(buffer);
		tex2Texture.Apply();
    }

    protected void ToTensor(Texture inputTex, float[,,] inputs, float offset, float scale)
    {
        RenderTexture tex = tex2tensor.Resize(inputTex, resizeOptions);
        tex2tensor.ToTensor(tex, inputs, offset, scale);
    }

    protected void ToTensor(Texture inputTex, byte[,,] inputs, float offset, float scale)
    {
        RenderTexture tex = tex2tensor.Resize(inputTex, resizeOptions);
        tex2tensor.ToTensor(tex, inputs, offset, scale);
    }

    private void InitInputs()
    {
        var idim0 = interpreter.GetInputTensorInfo(0).shape;
        var height = idim0[1];
        var width = idim0[2];
        var channels = idim0[3];
        // inputs = new float[height, width, channels];

        int inputCount = interpreter.GetInputTensorCount();
        for (int i = 0; i < inputCount; i++)
        {
            int[] dim = interpreter.GetInputTensorInfo(i).shape;
            interpreter.ResizeInputTensor(i, dim);
        }
        interpreter.AllocateTensors();
    }

#pragma warning disable CS0162 // Unreachable code detected 
    static IGpuDelegate CreateGpuDelegate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return new GpuDelegate();
#elif UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        return new MetalDelegate(new MetalDelegate.Options()
        {
            allowPrecisionLoss = false,
            waitType = MetalDelegate.WaitType.Passive,
        });
#endif
        UnityEngine.Debug.LogWarning("GPU Delegate is not supported on this platform");
        return null;
    }
#pragma warning restore CS0162 // Unreachable code detected 
}
