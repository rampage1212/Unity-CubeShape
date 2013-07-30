using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

    public AudioSource completeSound;

    private LevelManager levelManager;

    private GameObject currentLevel;
    private Transform playerCube;

    private Vector3 originalCameraPosition;
    private Vector3 startPosition;
    private Vector3 mapCenter;

	void Start() {
        originalCameraPosition = Camera.main.transform.position;
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        InitializeLevel(levelManager.CurrentLevel());
	}

    void InitializeLevel(GameObject level) {
        // Restore original camera position
        Camera.main.transform.position = originalCameraPosition;
        Camera.main.transform.rotation = Quaternion.identity;
        
        currentLevel = Instantiate(level, Vector3.zero, Quaternion.identity) as GameObject;

        playerCube = currentLevel.transform.Find("PlayerCube");
        startPosition = playerCube.transform.position;
        mapCenter = new Vector3(0.0f, 3.0f, 0.0f);
    }

    void OnGUI() {
        GUILayout.Label("Level "+ (levelManager.currentLevelID + 1));
    }
	
	void Update() {
        if (currentLevel == null || playerCube == null) {
            return;
        }

        // Player out of bounds
        if (Vector3.Distance(playerCube.transform.position, mapCenter) > 6.0f) {
            iTween.StopByName("playerMovement");
            playerCube.GetComponent<PlayerControls>().deathSound.Play();
            playerCube.transform.position = startPosition;
        }

        // Player reached the finish
        if (playerCube.GetComponent<PlayerControls>().finished &&
            iTween.Count(playerCube.gameObject) == 0) {
            completeSound.Play();
            Destroy(currentLevel);
            InitializeLevel(levelManager.NextLevel());
        }
	}
}
