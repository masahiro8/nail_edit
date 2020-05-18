using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class NailMaterialEdit
{
    // [MenuItem("Tools/NailMaterial/Name To Type")]
    // private static void Delete()
    // {
    //     // AssetDatabase.Refresh();
    //     var path = "Assets/Resources/Data/NailMaterial";
    //     var suffix = ".asset";
    //     foreach (var f in Directory.GetFiles(path)) {
    //         if (f.Substring(f.Length - suffix.Length, suffix.Length) == suffix) {
    //             var d1 = AssetDatabase.LoadAssetAtPath<NailMaterialTable>(f);
    //             Debug.Log(f + ": " + d1.name);
    //             foreach (var d2 in d1.list) {
    //                 switch (d2.materialName) {
    //                     case "NailBase":
    //                         d2.materialType = NailMaterialType.Base;
    //                         break;
    //                     case "NailLame":
    //                         d2.materialType = NailMaterialType.Lame;
    //                         break;
    //                     default:
    //                         break;
    //                 }
    //             }
    //             EditorUtility.SetDirty(d1);
    //             // break; // 1ファイルだけのテスト
    //         }
    //     }
    // }
}
