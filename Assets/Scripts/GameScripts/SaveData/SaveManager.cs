using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class SaveManager : MonoBehaviour
{
    public List<ISaveable> saveObjects;
    private static SaveManager _instance;

    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Create();
            }

            return _instance;
        }
    }

    private static void Create()
    {
        new GameObject("SaveManager").AddComponent<SaveManager>();
    }

    public void SaveAll()
    {
        foreach (ISaveable item in saveObjects)
        {
            item.Save();
        }

        SaveSystem.Save();

        Debug.Log("Saved all");
    }

    public void LoadAll()
    {
        if (SaveSystem.saveData.SaveFileExists)
        {
            foreach (ISaveable item in saveObjects)
            {
                item.Load();
            }
        }

        Debug.Log("Loaded all");
    }

    private void Awake()
    {
        saveObjects = new List<ISaveable>();

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += GetSaveablesFromScene;
        SceneManager.sceneLoaded += LoadSceneObjectStates;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= GetSaveablesFromScene;
        SceneManager.sceneLoaded -= LoadSceneObjectStates;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void GetSaveablesFromScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        IEnumerable<ISaveable> _saveObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

        saveObjects = new List<ISaveable>(_saveObjects.Count());

        foreach (ISaveable s in _saveObjects)
        {
            saveObjects.AddRange(_saveObjects);
        }
    }

    private void LoadSceneObjectStates(Scene scene, LoadSceneMode loadSceneMode)
    {
        SaveManager.Instance.LoadAll();
    }

    private GameObject[] FindAll<T>() where T : MonoBehaviour
    {
        UnityEngine.Object[] objectT = UnityEngine.Object.FindObjectsOfType(typeof(T));
        GameObject[] allObjectsT = new GameObject[objectT.Length];

        for (int i = 0; i < objectT.Length; i++)
        {
            allObjectsT[i] = ((T)objectT[i]).gameObject;
        }

        return allObjectsT;
    }

    private void OnApplicationQuit()
    {
        SaveAll();
    }
}