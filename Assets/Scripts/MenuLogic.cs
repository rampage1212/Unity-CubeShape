using UnityEngine;
using System.Collections;

public class MenuLogic : MonoBehaviour {

    public GUISkin skin;

    private Rect innerArea = new Rect(500.0f, 200.0f, 500.0f, 500.0f);

    const float targetWidth = 1024.0f;
    const float targetHeight = 768.0f;

    private LevelManager levelManager;
    private Vector3 scale;

    private GameObject decorationCube;

	void Start() {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        decorationCube = GameObject.Find("DecorationCube");
	}
	
	void Update() {
	    if (iTween.Count(decorationCube) == 0) {
            iTween.RotateBy(decorationCube, iTween.Hash("y", 1.0f, "time", 7.0f, "easetype", "linear"));
        }
	}

    void OnGUI() {
        GUI.skin = skin;

        // Apply scale
        scale.x = Screen.width / targetWidth;
        scale.y = Screen.height / targetHeight;
        scale.z = 1.0f;

        // Apply GUI scaling
        Matrix4x4 matrix = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);

        GUILayout.BeginArea(new Rect(50.0f, 50.0f, 500.0f, 300.0f));
        GUILayout.Label("Quick tutorial:");
        GUILayout.Label("Move the red cube to the green one.");
        GUILayout.Label("");
        GUILayout.Label("Controls:");
        GUILayout.Label("Arrow keys - move the red cube up, down, left or right.");
        GUILayout.Label("WSAD - move around the big cube.");
        GUILayout.Label("QE - rotate the big cube.");
        GUILayout.Label("Space - return to the default view.");
        GUILayout.EndArea();

        GUILayout.BeginArea(innerArea);

        if (levelManager.currentLevelPack == null) {
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(500.0f));
            for (int i = 0; i < levelManager.levelPacks.Count; i++) {
                LevelPack levelPack = levelManager.levelPacks[i];
                if (CubeGUI.Button(GUILayout.Button(levelPack.packName, skin.customStyles[0]))) {
                    iTween.Stop();
                    levelManager.setCurrentLevelPack(levelPack);
                }
            }
            GUILayout.EndHorizontal();
            if (CubeGUI.Button(GUILayout.Button("Level Editor"))) {
                iTween.Stop();
                Invoke("LoadLevelEditor", 0.5f);
            }
        } else {
            if (CubeGUI.Button(GUILayout.Button("Back", skin.customStyles[0]))) {
                levelManager.setCurrentLevelPack(null);
            } else {
                int size = levelManager.currentLevelPack.levels.Count;
                for (int i = 0; i < size; i++) {
                    int id = i + 1;
                    if (id == 1 || id % 9 == 0) {
                        GUILayout.BeginHorizontal(GUILayout.MaxWidth(100.0f));
                    }

                    if (CubeGUI.Button(GUILayout.Button("" + (i + 1), skin.customStyles[1]))) {
                        iTween.Stop();
                        levelManager.SetCurrentLevel(i);
                        
                        Invoke("LoadGameLevel", 0.5f);
                    }

                    if (id % 8 == 0 || i == size - 1) {
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }

        GUILayout.EndArea();

        // Restore matirx
        GUI.matrix = matrix;
    }

    void LoadGameLevel() {
        Application.LoadLevel("Game");
    }

    void LoadLevelEditor() {
        Application.LoadLevel("LevelEditor");
    }
}
