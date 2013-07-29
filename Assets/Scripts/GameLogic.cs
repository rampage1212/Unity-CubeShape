using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

    private GameObject playerCube;
    private Vector3 startPosition;
    private Vector3 mapCenter;

	void Start() {
        Instantiate(Config.levelToLoad, Vector3.zero, Quaternion.identity);

        playerCube = GameObject.Find("PlayerCube");
        startPosition = playerCube.transform.position;
        mapCenter = new Vector3(0.0f, 3.0f, 0.0f);
	}
	
	void Update() {
        if (Vector3.Distance(playerCube.transform.position, mapCenter) > 6.0f) {
            iTween.StopByName("playerMovement");
            playerCube.GetComponent<PlayerControls>().deathSound.Play();
            playerCube.transform.position = startPosition;
        }
	}
}
