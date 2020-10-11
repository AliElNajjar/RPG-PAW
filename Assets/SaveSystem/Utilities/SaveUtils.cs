using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Xml.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveUtils : MonoBehaviour
{
    /// <summary>
    /// Return the byte array of a given object
    /// </summary>
    /// <param name="save"></param>
    /// <returns>the data as a byte array</returns>
    public static byte[] ToByteArray(object save)
    {
        byte[] data;
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            bf.Serialize(stream, save);
            data = stream.ToArray();
            return data;
        }
    }

    /// <summary>
    /// Return the value of a byte array as an object
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static object FromByteArray(byte[] array)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(array))
        {
            return bf.Deserialize(stream);
        }
    }

    public static void WriteFile(object saveData, string filePath)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream file = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            bf.Serialize(file, saveData);
        }
    }

    public static object ReadFile(string filePath)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream file = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            return bf.Deserialize(file);
        }
    }

    public static ObjT LoadFromJson<ObjT>(string path, string name)
    {
        try
        {
            string file = path;
            if (File.Exists(file))
            {
                string json = File.ReadAllText(file);

                return JsonConvert.DeserializeObject<ObjT>(json);
            }
            else
            {
                return default(ObjT);
            }

        }
        catch
        {
            return default(ObjT);
        }
    }

    public static void SaveToJson<ObjT>(string path, ObjT obj, JsonSerializerSettings jsonSerializerSettings = null)
    {
        try
        {
            var jsonObj = JsonConvert.SerializeObject(obj, Formatting.Indented, jsonSerializerSettings);

            File.WriteAllText(path, jsonObj);
        }
        catch (Exception e)
        {
            Debug.LogError("Error serializing: " + e.ToString());
        }
    }

    public static void SaveToUnityJson<ObjT>(string path, ObjT obj)
    {
        string objJson = JsonUtility.ToJson(obj, true);

        string filePath = path;

        File.WriteAllText(filePath, objJson);
    }

    public static ObjT LoadFromUnityJson<ObjT>(string path, string name)
    {
        try
        {
            string file = path + "/" + name;
            if (File.Exists(file))
            {
                string json = File.ReadAllText(file);

                return JsonUtility.FromJson<ObjT>(file);
            }
            else
            {
                return default(ObjT);
            }

        }
        catch
        {
            return default(ObjT);
        }
    }

    public static void SaveToXML(string path, object obj)
    {
        var serializer = new XmlSerializer(obj.GetType());
        var stream = new FileStream(path, FileMode.Create);
        serializer.Serialize(stream, obj);
        stream.Close();
    }

    public static T LoadFromXML<T>(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StreamReader reader = new StreamReader(path);
        T deserialized = (T)serializer.Deserialize(reader.BaseStream);
        reader.Close();
        return deserialized;
    }


#if UNITY_EDITOR
    [MenuItem("Save Data/Open Save Directory")]
    public static void OpenSaveDirectory()
    {
        string itemPath = SaveSystem.saveData.SaveFileDirectory.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
    }

    [MenuItem("Save Data/Delete Save File")]
    public static void DeleteSaveFile()
    {
        if (SaveSystem.saveData.SaveFileExists)
        {
            File.Delete(StandaloneSaveData.filePath);
        }
    }
#endif
}
