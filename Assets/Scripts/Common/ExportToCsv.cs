using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
 
public class ExportToCsv : MonoBehaviour {

    static public ExportToCsv instance;
    void Awake (){
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad (gameObject);
        } else {
            Destroy (gameObject);
        }
    }

    public async void Export (string fileName , string list) {
        //ファイル保存先
        string path = Application.temporaryCachePath + "/" + fileName;
        Debug.Log("path " +path);
        StreamWriter sw = new StreamWriter(path,false, Encoding.GetEncoding("UTF-8"));
        await sw.WriteLineAsync(list);
        sw.Close();
    }

    public void ImportEachLine ( string fileName, System.Action<int, string[]> action ) {
        string path = Application.temporaryCachePath + "/" + fileName;
        // StreamReader reader = new StreamReader(path);
        using (StreamReader sr = new StreamReader(path)){
            string line;
            int index = 0;
            while ((line = sr.ReadLine()) != null){
                if( line != "") {
                    Debug.Log(">>>>>>>>>>>>>> "+ index +" - " +line);
                    action(index , line.Split(','));
                    index++;
                }
            }
        }
        // reader.Close();
    }
     
    // Update is called once per frame
    void Update () {
         
    }
}