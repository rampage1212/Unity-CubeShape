using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    public List<LevelPack> levelPacks;
    public LevelPack currentLevelPack { get; private set; }

    public int currentLevelID { get; private set; }

	void Start() {
        levelPacks = new List<LevelPack>();

        String directory;
        #if UNITY_EDITOR 
            directory = "Build\\Levels";
        #else
            directory = "Levels";
        #endif

        foreach (string packName in Directory.GetDirectories(directory)) {
            // Get the name of the pack
            String[] words = packName.Split('\\');
            LevelPack pack = new LevelPack(words[words.Length - 1]);
            
            foreach (string levelName in Directory.GetFiles(packName, "*.lvl")) {
                Stream stream = new FileStream(levelName, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                
                // Deserialize level file
                pack.AddLevel((LevelInfo) formatter.Deserialize(stream));
                
                stream.Close();
            }

            levelPacks.Add(pack);
        }
	}

    public void SetCurrentLevel(int id) {
        currentLevelID = id;
    }

    public void setCurrentLevelPack(LevelPack levelPack) {
        currentLevelPack = levelPack;
    }

    public LevelInfo NextLevel() {
        if (currentLevelID < currentLevelPack.levels.Count - 1) {
            currentLevelID++;            
        }

        return CurrentLevel();
    }

    public LevelInfo CurrentLevel() {
        return currentLevelPack.levels[currentLevelID];
    }
}
