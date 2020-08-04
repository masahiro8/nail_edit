using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPhoto : MonoBehaviour
{
    static DebugPhoto _Instance = null;
    public static DebugPhoto Instance
    {
        get {
            if (_Instance == null) {
                _Instance = FindObjectOfType(typeof(DebugPhoto)) as DebugPhoto;
                if (_Instance == null) {
                    var obj = new GameObject("DebugPhoto");
                    _Instance = obj.AddComponent<DebugPhoto>();
                    _Instance.Setup();
                    DontDestroyOnLoad(obj);
                }
            }
            return _Instance;
        }
    }

    private int index = -1;
    private string folder = "Test/";
    private string[] imageFileName = {
        "nail_k2_20200226_1000064", // paa
        "nail_k2_20200226_1000058", // goo
        // "nail_k1_827", // paa
        // "bkm_20200130_1035", // goo
        // "513x513",
        // "bicycle513x513",
        // "bng_not_used_20200213_18", // goo
        // "bng_not_used_20200213_127", // goo
        // "nail_k1_66", // goo
        // "R0203318_HQ", // goo
        // "R0203340-Edit_HQ", // foot
        // "R0203340-Edit_HQ-2", // foot
        // "R0203340-Edit_HQ-3", // foot
    };
    private string[] jsonFileName = {
        "sample_result_pa",
        "sample_result",
        // "sample_result_pa",
        // "sample_result",
        // "sample_result_pa",
        // "sample_result",
        // "sample_result",
        // "sample_result",
        // "sample_result",
        // "sample_result",
        // "sample_result",
        // "sample_result",
        // "sample_result",
    };
    private int[,] rotateArray = {
        { 10, 10, 0, 10, 0 },
        { 90, 60, 50, 35, 125 },
    };
    private TextAsset[] jsonText;

    public string PhotoFileName {
        get {
            return folder + imageFileName[index];
        }
    }

    public string PhotoFileJson {
        get {
            return jsonText[index].text;
        }
    }

    public float GetRotate(int n)
    {
        return (float)rotateArray[index % rotateArray.GetLength(0), n % rotateArray.GetLength(1)];
    }

    public void AddIndex()
    {
        index = (index + 1) % imageFileName.Length;
    }

    public void Setup()
    {
        jsonText = new TextAsset[imageFileName.Length];
        for (var i = 0; i < imageFileName.Length; i++) {
            jsonText[i] = Resources.Load(folder + jsonFileName[i]) as TextAsset;
        }
    }
}
