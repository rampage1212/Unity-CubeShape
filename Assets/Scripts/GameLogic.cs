using UnityEngine;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour {

    public GUISkin HUDSkin;
    public AudioSource completeSound;

    public GameObject playerCubePrefab;
    public GameObject finishCubePrefab;
    public GameObject cubePrefab;

    private LevelManager levelManager;
    
    // Player stuff
    public Transform playerCube { get; private set; }
    public PlayerControls playerControls { get; private set; }

    // Level info
    private GameObject finishCube;
    private List<GameObject> cubes;

    private Vector3 originalCameraPosition;
    private Vector3 startPosition;
    private Vector3 mapCenter;

	void Start() {
        originalCameraPosition = Camera.main.transform.position;
        cubes = new List<GameObject>();

        if (GameObject.Find("LevelManager") != null) {
            levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            InitializeLevel(levelManager.CurrentLevel());
        }
	}

    void InitializeLevel(LevelInfo level) {
        // Restore original camera position
        Camera.main.transform.position = originalCameraPosition;
        Camera.main.transform.rotation = Quaternion.identity;
        
        GameObject playerObject = Instantiate(playerCubePrefab, 
            level.playerCube.position(), Quaternion.identity) as GameObject;
        playerCube = playerObject.transform;

        finishCube = Instantiate(finishCubePrefab, level.finishCube.position(), Quaternion.identity) as GameObject;

        foreach (Cube standardCube in level.cubes) {
            cubes.Add(Instantiate(cubePrefab, standardCube.position(), Quaternion.identity) as GameObject);
        }

        playerControls = playerCube.GetComponent<PlayerControls>();
        startPosition = playerCube.transform.position;
        mapCenter = new Vector3(0.0f, 3.0f, 0.0f);
    }

    void OnGUI() {
        GUI.skin = HUDSkin;

        if (levelManager != null) {
            GUILayout.Label("Level " + (levelManager.currentLevelID + 1));
        } else {
            GUILayout.Label("Test Level");
        }

        GUILayout.Label("Moves: " + playerControls.movesCount);
    }
	
	void Update() {
        if (playerCube == null) {
            return;
        }

        // Player out of bounds
        if (Vector3.Distance(playerCube.transform.position, mapCenter) > 6.0f) {
            iTween.Stop(playerCube.gameObject);
            playerControls.resetMovesCount();
            playerControls.deathSound.Play();
            playerCube.transform.position = startPosition;
        }

        // Player reached the finish
        if (playerControls.finished &&
            iTween.Count(playerCube.gameObject) == 0) {
            completeSound.Play();

            Destroy(playerCube.gameObject);
            Destroy(finishCube);

            foreach (GameObject cube in cubes) {
                Destroy(cube);
            }
            cubes.Clear();

            if (levelManager != null) {
                InitializeLevel(levelManager.NextLevel());
            }
        }
	}
}
