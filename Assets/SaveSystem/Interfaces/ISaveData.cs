using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveData
{
    void Initialize();
    void SetFilePath(string setDirectoryPath, string setFileName, string setFileExtension);
    void Save(object save);
    T Load<T>(T fileToLoad);
    void DeleteSaveFile();

    bool DirectoryExists { get; }
    bool SaveFileExists { get; }
    string SaveFileDirectory { get; }

    void CreateDirectory();
}
