using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

    private Vector3 startPosition;

    private AudioClip hitSound;
    private AudioClip deathSound;
    private AudioClip completeSound;

    const float animationSpeed = 15.0f;

    private GameLogic game;

	void Start() {
        startPosition = transform.position;

        hitSound = Resources.Load("hit") as AudioClip;
        deathSound = Resources.Load("death") as AudioClip;
        completeSound = Resources.Load("complete") as AudioClip;

        game = GameObject.Find("GameLogic").GetComponent<GameLogic>();
	}

	void Update() {

        // If not moving at the moment, check input
        if (iTween.Count(gameObject) == 0
            && iTween.Count(Camera.main.gameObject) == 0) {
            
            Transform direction = Camera.main.transform;
            Vector3 movement = Vector3.zero;

            // Handle Cube movement
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                movement = -direction.right;
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                movement = direction.right;
            } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                movement = direction.up;
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                movement = -direction.up;
            }

            // If there was any input, move the cube
            if (!movement.Equals(Vector3.zero)) {
                string onComplete = "AfterMovement";
                
                int i;
                for (i = 1; i <= game.levelSize * 2; i++) {
                    Collider[] colliders = Physics.OverlapSphere(transform.position + movement * i, 0.1f);
                    if (colliders.Length > 0) {
                        GameObject collision = colliders[0].gameObject;

                        // Collision with FinishCube
                        if (collision.CompareTag("FinishCube")) {
                            onComplete = "AfterFinish";
                        }
                        // Player out of the borders
                        else if (collision.CompareTag("ExitTrigger")) {
                            onComplete = "AfterDeath";
                        } else {
                            i--;
                        }
                        break;
                    }
                }

                // Move the Cube
                if (i > 0) {
                    game.IncreaseMovesCount();
                    iTween.MoveTo(gameObject, iTween.Hash("position", transform.position + movement * i,
                        "speed", animationSpeed, 
                        "easetype", "linear",
                        "onComplete", onComplete));
                }
            }
        }
	}

    void AfterMovement() {
        // Play the hit sound
        GameManager.instance.PlayAudio(hitSound);
    }

    void AfterDeath() {
        iTween.Stop(gameObject);
        GameManager.instance.PlayAudio(deathSound);
        transform.position = startPosition;
        game.ResetMovesCount();
        Camera.main.GetComponent<CameraControls>().ReturnToOriginalPosition();
    }

    void AfterFinish() {
        // Play the finish sound
        GameManager.instance.PlayAudio(completeSound);
        game.NextLevel();
    }
}
