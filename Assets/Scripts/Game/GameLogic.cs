using UnityEngine;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour {

    public GUISkin HUDSkin;

    private AudioClip completeSound;

    public GameObject playerCubePrefab;
    public GameObject finishCubePrefab;
    public GameObject standardCubePrefab;
    
    // Player stuff
    public Transform playerCube { get; private set; }
    public PlayerControls playerControls { get; private set; }

    // Level info
    private GameObject finishCube;
    private List<GameObject> cubes;

    // Moves counter
    public int movesCount { get; private set; }

    private Vector3 originalCameraPosition;
    private Vector3 mapCenter;

    private LevelManager levelManager;

	void Start() {
        if (GameObject.Find("LevelManager") != null) {
            originalCameraPosition = Camera.main.transform.position;
            cubes = new List<GameObject>();

            completeSound = Resources.Load("complete") as AudioClip;

            levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            InitializeLevel(levelManager.CurrentLevel());
        } else {
            Application.LoadLevel("Menu");
        }
	}

    void InitializeLevel(LevelInfo level) {
        ResetMovesCount();

        // Restore original camera position
        Camera.main.transform.position = originalCameraPosition;
        Camera.main.transform.rotation = Quaternion.identity;
        
        // Instantiate cubes and set their parent
        GameObject levelParent = GameObject.Find("Level");

        GameObject playerObject = Instantiate(playerCubePrefab, 
            level.playerCube.position(), Quaternion.identity) as GameObject;
        playerObject.transform.parent = levelParent.transform;
        playerCube = playerObject.transform;

        finishCube = Instantiate(finishCubePrefab, level.finishCube.position(), Quaternion.identity) as GameObject;
        finishCube.transform.parent = levelParent.transform;

        foreach (Cube standardCube in level.cubes) {
            GameObject cube = Instantiate(standardCubePrefab, standardCube.position(), Quaternion.identity) as GameObject;
            cube.transform.parent = levelParent.transform;
            cubes.Add(cube);
        }

        playerControls = playerCube.GetComponent<PlayerControls>();
        mapCenter = new Vector3(0.0f, 3.0f, 0.0f);
    }

    void OnGUI() {
        GUI.skin = HUDSkin;

        if (levelManager != null) {
            GUILayout.Label("Level " + (levelManager.currentLevelID + 1));
        } else {
            GUILayout.Label("Test Level");
        }

        GUILayout.Label("Moves: " + movesCount);
    }

    void Update() {
        if (playerCube == null) {
            return;
        }

        // Player out of bounds
        if (Vector3.Distance(playerCube.transform.position, mapCenter) > 6.0f) {
            iTween.Stop(playerCube.gameObject);
            ResetMovesCount();
            playerControls.AfterDeath();
        }

        // Player reached the finish
        if (playerControls.finished &&
            iTween.Count(playerCube.gameObject) == 0) {
            // Play the finish sound
            GameManager.instance.PlayAudio(completeSound);

            // Clear the level
            Destroy(playerCube.gameObject);
            Destroy(finishCube);

            foreach (GameObject cube in cubes) {
                Destroy(cube);
            }
            cubes.Clear();

            if (levelManager != null) {
                // Load next level
                InitializeLevel(levelManager.NextLevel());
            }
        }
    }

    public void ResetMovesCount() {
        movesCount = 0;
    }

    public void IncreaseMovesCount() {
        movesCount++;
    }
}
