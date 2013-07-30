using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

    public AudioSource completeSound;

    private GameObject currentLevel;
    private GameObject playerCube;

    private Vector3 startPosition;
    private Vector3 mapCenter;

	void Start() {
        InitializeLevel(Config.levelToLoad);
	}

    void InitializeLevel(GameObject level) {
        currentLevel = Instantiate(level, Vector3.zero, Quaternion.identity) as GameObject;

        playerCube = GameObject.Find("PlayerCube");
        startPosition = playerCube.transform.position;
        mapCenter = new Vector3(0.0f, 3.0f, 0.0f);
    }
	
	void Update() {
        if (currentLevel == null) {
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
            iTween.Count(playerCube) == 0) {
            completeSound.Play();
            Destroy(currentLevel);           
        }
	}
}
