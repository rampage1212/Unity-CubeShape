using UnityEngine;
using System.Collections;

public class MenuLogic : MonoBehaviour {

    public GUISkin skin;
    public GameObject[] levels;

    private Rect innerArea = new Rect(400.0f, 200.0f, 150.0f, 500.0f);

    const float targetWidth = 1024.0f;
    const float targetHeight = 768.0f;

    private Vector3 scale;

	void Start() {
	
	}
	
	void Update() {
	
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
        foreach (GameObject level in levels) {
            if (GUILayout.Button(level.name)) {
                Config.levelToLoad = level;
                Application.LoadLevel("Game");
            }
        }
        GUILayout.EndArea();

        // Restore matirx
        GUI.matrix = matrix;
    }
}
