using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class JsonManager : MonoBehaviour
{   
    private UserDataClass userData;

    public void SaveData<T>(T data, string file = "userData.json"){
        string json = JsonUtility.ToJson(data, true);
        WriteToFile(file, json);
    }

    private void WriteToFile(string fileName, string json){
        string path = GetFilePath(fileName);
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using(StreamWriter writer = new StreamWriter(fileStream)){
            writer.Write(json);
        }
    }

    private string GetFilePath(string fileName){
        string path = Application.dataPath + "/UserData/" + fileName;   
#if UNITY_ANDROID
        path = Application.persistentDataPath + "/" + fileName;
#endif
#if UNITY_IOS // 해당 앱의 documents 폴더
        path = Application.persistentDataPath + "/" + fileName;
#endif
        return path;
    }

    public UserDataClass LoadData(string file = "userData.json"){ // 기존데이터 불러오기
        string json = ReadFromFile(file);
        if(json == ""){
            userData = null;
            Debug.Log("null");
        }
        else{
            userData = JsonUtility.FromJson<UserDataClass>(json);
            Debug.Log("data Load Success");
        }
        
        return userData;
    }


    private string ReadFromFile(string fileName){
        string path = GetFilePath(fileName);
        if(File.Exists(path)){
            Debug.Log("Open existing File");
            using(StreamReader reader = new StreamReader(path)){
            string json = reader.ReadToEnd();
            return json;
            }  
        }

        return ""; // 파일이 없는 경우
    }

//--------------------------------------------------------------
    
    public T LoadJson<T>(string fileName){ // fileName에 .txt 안들어감
        T tempData;
        fileName = "JsonData/"+fileName;
        TextAsset textData = Resources.Load<TextAsset>(fileName);
        tempData = JsonUtility.FromJson<T>(textData.ToString());
        return tempData;
    }

}
