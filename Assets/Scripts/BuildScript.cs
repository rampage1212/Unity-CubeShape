#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class NewBehaviourScript : MonoBehaviour {

    [MenuItem("Tools/Generate Levels file")]
    public static void GenerateLevelsFile() {
        string path = "Build/Web/levels.data";
        string[] separators = { "Levels" };

        List<ImportPack> levelPacks = LevelManager.GetLevelPacks();
        foreach (ImportPack importPack in levelPacks) {
            for (int i = 0; i < importPack.levels.Count; i++) {
                string[] words = importPack.levels[i].Split(separators, System.StringSplitOptions.None);
                importPack.levels[i] = words[1].Replace('\\', '/');
            }
        }

        Stream stream = new FileStream(path, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, levelPacks);
        stream.Close();

        EditorUtility.DisplayDialog("Levels file", "Build completed!", "Ok");
    }
}
#endif