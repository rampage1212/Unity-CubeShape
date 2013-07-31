using UnityEngine;
using System.Collections;

public class MenuLogic : MonoBehaviour {

    public GUISkin skin;
    public AudioSource buttonSound;

    private Rect innerArea = new Rect(400.0f, 200.0f, 150.0f, 500.0f);

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

        GUILayout.BeginArea(innerArea);
        for (int i = 0; i < levelManager.levels.Length; i++) {
            GameObject level = levelManager.levels[i];
                if (GUILayout.Button(level.name)) {
                    iTween.Stop();
                    buttonSound.Play();

                    levelManager.SetCurrentLevel(i);
                    Object.DontDestroyOnLoad(levelManager);
                    Invoke("LoadGameLevel", 0.5f);
                }
            }
        GUILayout.EndArea();

        // Restore matirx
        GUI.matrix = matrix;
    }

    void LoadGameLevel() {
        Application.LoadLevel("Game");
    }
}
