using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[Serializable]
public class ImportPack {
    public string name;
    public List<string> levels;
    public ImportPack(string _name) {
        name = _name;
        levels = new List<string>();
    }
}

public class LevelManager : MonoBehaviour {

    public static LevelManager instance { get; private set; }

    public List<LevelPack> levelPacks { get; private set; }
    public LevelPack currentLevelPack { get; private set; }
    public LevelInfo testLevel { get; private set; }

    public int currentLevelID { get; private set; }

    void Awake() {
        if (instance) {
            Destroy(gameObject);
            return;
        }

        GameObject.DontDestroyOnLoad(gameObject);
        instance = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        levelPacks = new List<LevelPack>();
    }

    public void LoadLevelPacks() {
        levelPacks.Clear();

        if (Application.isWebPlayer) {
            StartCoroutine(LoadLevelPacksWebplayer());
        } else {
            LoadLevelPacksStandalone();
        }
    }

    // Load Levels for Standalone build
    void LoadLevelPacksStandalone() {
        foreach (ImportPack importPack in GetLevelPacks()) {
            LevelPack pack = new LevelPack(importPack.name);
            foreach (string levelName in importPack.levels) {
                Stream stream = new FileStream(levelName, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize level file
                pack.AddLevel(formatter.Deserialize(stream) as LevelInfo);

                stream.Close();
            }

            levelPacks.Add(pack);
        }
    }

    // Load levels for Webplayer
    IEnumerator LoadLevelPacksWebplayer() {
        WWW dataFile = new WWW(Application.dataPath + "/levels.data");
        yield return dataFile;
   
        BinaryFormatter formatter = new BinaryFormatter();

        MemoryStream memoryStream = new MemoryStream(dataFile.bytes);
        List<ImportPack> packs = formatter.Deserialize(memoryStream) as List<ImportPack>;
        memoryStream.Close();

        foreach (ImportPack importPack in packs) {
            LevelPack pack = new LevelPack(importPack.name);
            foreach (string levelName in importPack.levels) {
                WWW levelFile = new WWW(Application.dataPath + "/StreamingAssets/Levels" + levelName);
                yield return levelFile;
                
                // Deserialize level file
                MemoryStream levelStream = new MemoryStream(levelFile.bytes);
                pack.AddLevel(formatter.Deserialize(levelStream) as LevelInfo);
                levelStream.Close();
            }

            levelPacks.Add(pack);
        }

        yield return null;
    }

    public static List<ImportPack> GetLevelPacks() {
        List<ImportPack> levelPacks = new List<ImportPack>();

        foreach (string packName in Directory.GetDirectories(Application.streamingAssetsPath + "/Levels")) {
            // Get the name of the pack
            String[] words = packName.Split('\\');
            ImportPack importPack = new ImportPack(words[words.Length - 1]);
            levelPacks.Add(importPack);
            
            // Get level names
            foreach (string levelName in Directory.GetFiles(packName, "*.lvl")) {
                importPack.levels.Add(levelName);
            }
        }

        return levelPacks;
    }

    public void Reset() {
        SetCurrentLevel(0);
        SetCurrentLevelPack(null);
        SetTestLevel(null);
    }

    public void SetCurrentLevel(int id) {
        currentLevelID = id;
    }

    public void SetCurrentLevelPack(LevelPack levelPack) {
        currentLevelPack = levelPack;
    }

    public void SetTestLevel(LevelInfo levelInfo) {
        testLevel = levelInfo;
    }

    public LevelInfo NextLevel() {
        if (!TestMode()) {
            if (currentLevelID < currentLevelPack.levels.Count - 1) {
                currentLevelID++;
            }
        }

        return CurrentLevel();
    }

    public LevelInfo CurrentLevel() {
        if (testLevel != null) {
            return testLevel;
        } else {
            return currentLevelPack.levels[currentLevelID];
        }
    }

    public bool TestMode() {
        return testLevel != null;
    }
}
