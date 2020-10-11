using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnEnemiesManager : MonoBehaviour
{
    [ReadOnly] public int unitDefeatedIndex = -1;

    private static SpawnEnemiesManager _instance;

    private string sceneName;
    private List<bool> enabledEnemies;
    private List<GameObject> enemies;

    public static SpawnEnemiesManager Instance
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
        new GameObject("SpawnEnemiesManager").AddComponent<SpawnEnemiesManager>();
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.Log("Destroying this...");
            Destroy(this);
        }

        enabledEnemies = new List<bool>();
        enemies = new List<GameObject>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SaveData()
    {
        SpawnedEnemiesData data = new SpawnedEnemiesData(sceneName, enabledEnemies.ToArray());
        SaveSystem.SetObject<SpawnedEnemiesData>(SaveSystemConstants.SpawnedEnemiesData, data);

        for (int i = 0; i < enabledEnemies.Count; i++)
        {
            Debug.Log($"<color=blue>SaveData() -> Enemy State: </color>" + enabledEnemies[i]);
        }
    }

    private void LoadData()
    {
        if (enemies.Count == 0)
        {
            Debug.Log("<color=yellow> Won't load data since there no enemies was found in current scene. </color>");
            return;
        }

        SpawnedEnemiesData data = SaveSystem.GetObject<SpawnedEnemiesData>(SaveSystemConstants.SpawnedEnemiesData);

        if (string.IsNullOrEmpty(data.sceneName))
        {
            sceneName = SceneManager.GetActiveScene().name;

            Debug.Log("<color=red>Data fetched was empty.</color>");
            Debug.Log("<color=blue>New spawned enemies data:</color> \nScene Name: "
                + sceneName + "\nEnemies List count: " + enabledEnemies.Count);
        }
        else
        {
            sceneName = SceneManager.GetActiveScene().name;

            Debug.Log("Scene name saved: " + data.sceneName + " - Current Scene: " + sceneName);
            if (data.sceneName == SceneManager.GetActiveScene().name)
            {
                Debug.Log("Player remains in the same scene, updating enemies...");
                enabledEnemies.Clear();
                enabledEnemies.AddRange(data.enabledEnemies);

                for (int i = 0; i < enabledEnemies.Count; i++)
                    Debug.Log($"<color=blue>Enemy State: </color>" + enabledEnemies[i]);
            }
            else
            {
                Debug.Log("Player changed from one scene to another! All enemies will be active now.");
                unitDefeatedIndex = -1;

                enabledEnemies.Clear();
                for (int i = 0; i < enemies.Count; i++)
                {
                    enabledEnemies.Add(true);
                }
                SaveData();
            }
        }
    }

    /// <summary>
    /// Enable or disable enemy units in overworld scene.
    /// </summary>
    private void InitEnemies()
    {
        Debug.Log($"Enemies count: {enemies.Count} - Enabled count: {enabledEnemies.Count}");

        if (enemies.Count == 0 || enabledEnemies.Count == 0)
            return;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (i == unitDefeatedIndex)
            {
                Debug.Log($"Updating enemy defeated! The one with index {i}");
                Debug.Log($"BEFORE: "+ enemies[i].activeInHierarchy);
                enemies[i].SetActive(false);
                enabledEnemies[i] = false;
                unitDefeatedIndex = -1;

                Debug.Log($"AFTER: " + enemies[i].activeInHierarchy);
            }
            else
            {
                if (enemies[i] == null)
                    Debug.Log($"Unexpected: Unit w/ index {i} was null");
                else
                {
                    Debug.Log("SETACTIVE - enabled enemy: " + enabledEnemies[i]);
                    enemies[i].SetActive(enabledEnemies[i]);
                    Debug.Log("SETACTIVE - active? R: " + enemies[i].activeInHierarchy);
                }
            }
        }

        for (int i = 0; i < enabledEnemies.Count; i++)
            Debug.Log($"<color=blue>AFTER INIT - Enemy State: </color>" + enabledEnemies[i]);
    }

    /// <summary>
    /// Finds and saves enemies found in @SpawnEnemiesManager.enemiesHolder list.
    /// </summary>
    /// <returns></returns>
    private bool FindEnemies()
    {
        GameObject enemiesHolder = GameObject.Find("Enemies");

        if (enemiesHolder != null)
        {
            enemies.Clear();
            enabledEnemies.Clear();

            foreach (Transform enemy in enemiesHolder.transform)
            {
                enemies.Add(enemy.gameObject);
                enabledEnemies.Add(enemy.gameObject.activeInHierarchy);
                enemy.gameObject.SetActive(true);
            }

            Debug.Log("<color=green>FindEnemies()</color> - found: " + enemies.Count);
            if (enemies.Count == 0) return false;
            else return true;
        }

        return false;
    }

    public int FindEnemyIndex(string enemyName)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].name == enemyName) return i*-1;
        }

        return -404;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (scene.name == "Combat") return;

        FindEnemies();
        LoadData();
        InitEnemies();

        SaveData();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name != "Combat")
            SaveData();
    }
}
