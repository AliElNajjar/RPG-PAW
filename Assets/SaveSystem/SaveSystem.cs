using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_SWITCH
using nn.fs;
using nn.account;
#endif

[Serializable]
public class DataContainer
{
    public Hashtable dataStored;

    public DataContainer()
    {
        dataStored = new Hashtable();
    }
}

[System.Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}]", x, y, z);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}

[System.Serializable]
public struct SpawnedEnemiesData
{
    public string sceneName;
    public bool[] enabledEnemies;

    public SpawnedEnemiesData(string sceneName, bool[] enabledEnemies)
    {
        this.sceneName = sceneName;
        this.enabledEnemies = enabledEnemies;
    }
}


public class SaveSystem
{
    /// <summary>
    /// Are UserPrefs Initialized?
    /// </summary>
    public static bool isInitialized = false;
    /// <summary>
    /// Will we be saving every time user prefs is called?
    /// </summary>
    public static bool constantSaving = false;
    /// <summary>
    /// We will be restoring our values from playerprefs
    /// </summary>
    static bool restore = false;

#if UNITY_STANDALONE
    public static DataContainer dataContainer = new DataContainer();
#else
    public static DataContainer dataContainer = new DataContainer();
#endif

#if UNITY_STANDALONE
    public static ISaveData saveData = new StandaloneSaveData();
#elif UNITY_SWITCH && !UNITY_EDITOR
    public static ISaveData saveData = new NintendoSwitchSaveData();
#else
    public static ISaveData saveData = new StandaloneSaveData();
#endif

    #region Events
    public delegate void UserPrefAction();
    public static event UserPrefAction OnReloadUserprefs;
    public static event UserPrefAction OnDeleteAll;
    public static event UserPrefAction OnSave;
    public static event UserPrefAction OnLoad;
    #endregion

    /// <summary>
    /// Initialize the userprefs
    /// </summary>
    /// <param name="constantSaving">True means saving the file on every call, false means only on Save()</param>
    /// <param name="restoreFromPP">do we check for playerprefs values before userprefs?</param>
    public static void Initialize(bool constantSaving = false, bool restoreFromPP = false)
    {
        isInitialized = true;

        SaveSystem.constantSaving = constantSaving;

#if UNITY_STANDALONE
        //saveData.Save(dataContainer);
        LoadContainer();
#else
        LoadContainer();
#endif

        restore = restoreFromPP;

        OnSave += HandleSave;
        OnLoad += HandleLoad;
        OnReloadUserprefs += HandleReload;
    }

#if UNITY_STANDALONE
    static DataContainer GetOrCreateDataContainer()
    {
        DataContainer dc = new DataContainer();
        try
        {
            if (saveData.SaveFileExists)
                dc = saveData.Load(dataContainer) as DataContainer;

            return dc;
        }
        catch (SerializationException)
        {
            Debug.LogError("Data might be corrupted, rebuilding...");
            saveData.Save(dc);
            dc = saveData.Load(dataContainer) as DataContainer ?? new DataContainer();
            return dc;
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogError("Save folder does not exist, creating...");
            saveData.CreateDirectory();
            saveData.Save(dc);
            dc = saveData.Load(dataContainer) as DataContainer ?? new DataContainer();
            return dc;
        }
        catch (System.IO.IsolatedStorage.IsolatedStorageException)
        {
            Debug.LogError("Isolated storage exception...");
            saveData.CreateDirectory();
            saveData.Save(dc);
            dc = saveData.Load(dataContainer) as DataContainer ?? new DataContainer();
            return dc;
        }
    }
#endif

    public static void SetString(string key, string value)
    {
        SaveContainer(key, value);
    }

    public static string GetString(string key, string defaultValue = "")
    {
        if (restore) RestoreValue(key, typeof(string));

        LoadContainer();

        if (dataContainer.dataStored.ContainsKey(key))
        {
            return dataContainer.dataStored[key].ToString();
        }
        else
        {
            SaveContainer(key, defaultValue);
        }
        return defaultValue;
    }

    public static void SetInt(string key, int value)
    {
        SaveContainer(key, value);
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        if(restore) RestoreValue(key, typeof(int));

        LoadContainer();

        if (dataContainer.dataStored.ContainsKey(key))
        {
            return Convert.ToInt32(dataContainer.dataStored[key]);
        }
        else
        {
            SaveContainer(key, defaultValue);
        }

        return defaultValue;
    }

    public static void SetFloat(string key, float value)
    {
        SaveContainer(key, value);
    }

    public static float GetFloat(string key, float defaultValue = 0)
    {
        if (restore) RestoreValue(key, typeof(float));

        LoadContainer();

        if (dataContainer.dataStored.ContainsKey(key))
        {
            return (float)dataContainer.dataStored[key];
        }
        else
        {
            SaveContainer(key, defaultValue);
        }
        return defaultValue;
    }

    public static void SetBool(string key, bool value)
    {
        SaveContainer(key, value);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        if (restore) RestoreValue(key, typeof(bool));

        LoadContainer();

        if (dataContainer.dataStored.ContainsKey(key))
        {
            return (bool)dataContainer.dataStored[key];
        }
        else if (!dataContainer.dataStored.ContainsKey(key))
        {            
            SaveContainer(key, defaultValue);
        }
        return defaultValue;
    }

    public static void SetVector3(string key, Vector3 value)
    {
        SerializableVector3 v = new SerializableVector3(value.x, value.y, value.z);

        SaveContainer(key, v);
    }

    public static Vector3 GetVector3(string key)
    {
        if (restore) RestoreValue(key, typeof(Vector3));

        LoadContainer();

        if (dataContainer.dataStored.ContainsKey(key))
        {
            return JsonUtility.FromJson<Vector3>(dataContainer.dataStored[key].ToString());
        }

        return Vector3.zero;
    }

    public static void SetObject<T>(string key, T value)
    {
        if (value.GetType().IsSerializable)
        {
            SaveContainer(key, value);
        }
        else
        {
            Debug.LogError("Object is not serializable!");
        }
    }

    public static T GetObject<T>(string key)
    {
        if (restore) RestoreValue(key, typeof(T));

        LoadContainer();

        if (dataContainer.dataStored.ContainsKey(key))
        {
            if (dataContainer.dataStored[key] is Newtonsoft.Json.Linq.JArray)
            {
                Newtonsoft.Json.Linq.JArray jArray = (Newtonsoft.Json.Linq.JArray)dataContainer.dataStored[key];

                jArray.ToObject<T>();

                T newArray = (T)jArray.ToObject<T>();

                return newArray;
            }
            else if (dataContainer.dataStored[key] is Newtonsoft.Json.Linq.JObject)
            {
                Newtonsoft.Json.Linq.JObject jObject = (Newtonsoft.Json.Linq.JObject)dataContainer.dataStored[key];

                jObject.ToObject<T>();

                T newObject = (T)jObject.ToObject<T>();

                return newObject;
            }

            return (T)dataContainer.dataStored[key];
        }

        return default;
    }

    public static Vector3 GetVector3(string key, Vector3 defaultValue)
    {
        if (restore) RestoreValue(key, typeof(Vector3));

        LoadContainer();

        if (dataContainer.dataStored.ContainsKey(key))
        {
            return (Vector3)dataContainer.dataStored[key];
        }
        else if (!dataContainer.dataStored.ContainsKey(key))
        {
            SaveContainer(key, defaultValue);
        }
        return defaultValue;
    }

    #region Other methods
    public static bool HasKey(string name)
    {
        if (dataContainer.dataStored.ContainsKey(name)) return true;
        return false;
    }

    public static void DeleteKey(string key)
    {
        if (dataContainer.dataStored.ContainsKey(key))
            dataContainer.dataStored.Remove(key);
    }

    public static void DeleteAll()
    {
        CheckInitialization();
        OnDeleteAll?.Invoke();
        dataContainer.dataStored.Clear();
        Save();
    }

    /// <summary>
    /// Reloads the UserPrefs
    /// </summary>
    public static void ReloadUserData()
    {
        OnReloadUserprefs();
        dataContainer = new DataContainer();
        LoadContainer();
    }

    /// <summary>
    /// Restore values from previous playerprefs backups
    /// </summary>
    /// <param name="key"></param>
    /// <param name="variableType"></param>
    private static void RestoreValue(string key, Type variableType)
    {
        if (PlayerPrefs.HasKey(key))
        {
            Debug.LogFormat("{0} value was found, restoring...", key);

            if (variableType == typeof(int)) SetInt(key, PlayerPrefs.GetInt(key));
            else if (variableType == typeof(float)) SetFloat(key, PlayerPrefs.GetFloat(key));
            else if (variableType == typeof(string)) SetString(key, PlayerPrefs.GetString(key));
            else if (variableType == typeof(bool)) SetBool(key, PlayerPrefs.GetInt(key) == 1);

            PlayerPrefs.DeleteKey(key);
        }
    }

    /// <summary>
    /// Returns the current data as a byte array
    /// </summary>
    /// <returns></returns>
    public static byte[] DataAsByteArray
    {
        get
        {
            return DataContainerAsByteArray(dataContainer);
        }
    }

    /// <summary>
    /// Returns the current data container as a byte array
    /// </summary>
    /// <returns></returns>
    private static byte[] DataContainerAsByteArray(DataContainer container){
        byte[] data;
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            bf.Serialize(stream, container);
            data = stream.ToArray();
            return data;
        }
    }

    /// <summary>
    /// Set the datacontainer to restore the values from a byte array
    /// </summary>
    /// <param name="array">Byte array to restore from.</param>
    public static void RestoreFromByteArray(byte[] array)
    {
        dataContainer = FromByteArray(array);
    }

    public static DataContainer FromByteArray(byte[] array)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(array))
        {
            return bf.Deserialize(stream) as DataContainer;
        }
    }
#endregion

#region Saving and Loading the DataContainer
    private static void SaveContainer<T>(string key, T value)
    {
        CheckInitialization();

        if (dataContainer.dataStored.ContainsKey(key))
        {
            dataContainer.dataStored[key] = value;
        }
        else
        {
            dataContainer.dataStored.Add(key, value);
        }

        if(constantSaving) Save();        
    }   

    /// <summary>
    /// Check if userprefs have been initialized
    /// </summary>
    static void CheckInitialization()
    {
#if UNITY_EDITOR
        if (!isInitialized)
        {
            Debug.Log("Userprefs were not initialized, going into default mode, with constant saving on");
            Initialize(true, false); //initialize to default, which saves a data container
            LoadContainer(); //then load it
        }
#else
        if (!isInitialized)
        {
            Debug.Log("Userprefs were not initialized, going into default mode, with constant saving off");
            Initialize(true, false); //initialize to default, which saves a data container
            LoadContainer(); //then load it
        }
        // if (!isInitialized) throw new Exception("UserPrefs are not initialized");
#endif
    }

    public static void Save()
    {
        OnSave?.Invoke();
    }

    private static void LoadContainer()
    {
        OnLoad?.Invoke();
    }
#endregion

#region EventHandlers
    private static void HandleSave()
    {
        CheckInitialization();
        saveData.Save(dataContainer);
    }

    private static void HandleReload()
    {

    }

    private static void HandleLoad()
    {
        CheckInitialization();
        
        if (saveData.SaveFileExists)
            dataContainer = saveData.Load(dataContainer) as DataContainer ?? new DataContainer();
    }
#endregion
}

public interface ISaveable
{
    void Save();
    void Load();
}

