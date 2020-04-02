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

    private int index;
    private string[] imageFileName = {
        "nail_k2_20200226_1000064",
        "nail_k2_20200226_1000058",
    };
    private string[] jsonFileName = {
        "sample_result_pa",
        "sample_result",
    };
    private TextAsset[] jsonText;

    public string PhotoFileName {
        get {
            return imageFileName[index];
        }
    }

    public string PhotoFileJson {
        get {
            return jsonText[index].text;
        }
    }

    public void AddIndex()
    {
        index = (index + 1) % 2;
    }

    public void Setup()
    {
        jsonText = new TextAsset[imageFileName.Length];
        for (var i = 0; i < imageFileName.Length; i++) {
            jsonText[i] = Resources.Load(jsonFileName[i]) as TextAsset;
        }
    }
}
