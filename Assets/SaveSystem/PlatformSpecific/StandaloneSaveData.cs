using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class StandaloneSaveData : ISaveData
{
    #region FILE VALUES
    /// <summary>
    /// The folder inside in which the file will be saved
    /// </summary>
    public static string directoryPath = string.Concat(Application.persistentDataPath, "/Saves");

    /// <summary>
    /// The name of the file
    /// </summary>
    public static string filename = "Save";
    /// <summary>
    /// The extension of the file
    /// </summary>
    public static string fileExtension = "sav";
    /// <summary>
    /// The final path of the file
    /// </summary>
    public static string filePath = string.Format("{0}/{1}.{2}", directoryPath, filename, fileExtension);
    /// <summary>
    /// The base file name "Save"
    /// </summary>
    public readonly string baseFileName = filename;
    #endregion

    #region PROPERTIES
    public bool DirectoryExists
    {
        get { return Directory.Exists(directoryPath); }
    }

    public bool SaveFileExists
    {
        get { return File.Exists(filePath); }
    }

    public string SaveFileDirectory
    {
        get { return directoryPath; }
    }
    #endregion

    public StandaloneSaveData()
    {
        Initialize();
    }

    public void Initialize()
    {
        SetFilePath(directoryPath, filename, fileExtension);
        if (SaveFileExists) SaveSystem.dataContainer = Load<DataContainer>(SaveSystem.dataContainer);
        SaveSystem.Initialize(true);
    }

    /// <summary>
    /// Set the directory, file name, extension and if we are using events
    /// </summary>
    /// <param name="setDirectoryPath">The folder name path</param>
    /// <param name="setFileName">The name of the file</param>
    /// <param name="setFileExtension">The extension of the file</param>
    /// <param name="setUseEvents">Set to true if you are using events</param>
    public void SetFilePath(string setDirectoryPath = null, string setFileName = null, string setFileExtension = null)
    {
        directoryPath = setDirectoryPath ?? directoryPath;
        filename = setFileName ?? filename;
        fileExtension = setFileExtension ?? fileExtension;
        filePath = string.Format("{0}/{1}.{2}", directoryPath, filename, fileExtension);
    }

    public T Load<T>(T fileToLoad)
    {
        if (!DirectoryExists) CreateDirectory();

        //fileToLoad = SaveUtils.ReadFile(filePath);

        fileToLoad = SaveUtils.LoadFromJson<T>(filePath, "Save.sav");

        //fileToLoad = SaveUtils.LoadFromUnityJson<T>(filePath, "Save.sav");

        //fileToLoad = SaveUtils.LoadFromXML<object>(filePath);

        //DecryptFile(filePath, filePath);

        //Debug.Log("Loaded " + fileToLoad + " from " + filePath);

        return fileToLoad;
    }

    public void Save(object save)
    {
        if (!DirectoryExists) CreateDirectory();

        //SaveUtils.WriteFile(save, filePath);

        SaveUtils.SaveToJson<object>(filePath, save);
        //new JsonSerializerSettings()
        //{
        //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //});

        //SaveUtils.SaveToUnityJson<object>(filePath, save);

        //SaveUtils.SaveToXML(filePath, save);

        //Debug.Log("Saved " + save + " at " + filePath);
    }

    public void DeleteSaveFile()
    {
        if (File.Exists(filePath)) File.Delete(filePath);
        else return;

        Debug.Log("Deleted the file");
    }

    /// <summary>
    /// Create the save folder directory
    /// </summary>
    public void CreateDirectory()
    {
        Directory.CreateDirectory(directoryPath);
    }
}
