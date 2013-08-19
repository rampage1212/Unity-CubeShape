using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

    private Vector3 startPosition;
    public bool finished { get; private set; }
    
    private AudioClip hitSound;
    private AudioClip deathSound;

    const float animationSpeed = 15.0f;
    const int mapSize = 10; // TO-DO: this should not be hardcoded

    private GameLogic game;

	void Start() {
        startPosition = transform.position;
        hitSound = Resources.Load("hit") as AudioClip;
        deathSound = Resources.Load("death") as AudioClip;

        game = GameObject.Find("GameLogic").GetComponent<GameLogic>();
	}

	void Update() {

        // If not moving at the moment, check input
        if (!finished && iTween.Count(gameObject) == 0
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
                int i;
                for (i = 1; i <= mapSize; i++) {
                    Collider[] colliders = Physics.OverlapSphere(transform.position + movement * i, 0.1f);
                    if (colliders.Length > 0) {
                        // Collision with FinishCube
                        if (colliders[0].gameObject.tag == "FinishCube") {
                            finished = true;
                        }

                        // Decrease the amount of fields to go, so our Cube will stop just before its collider
                        i--;
                        break;
                    }
                }

                // Move the Cube
                if (i > 0) {
                    game.IncreaseMovesCount();
                    iTween.MoveTo(gameObject, iTween.Hash("position", transform.position + movement * i,
                        "speed", animationSpeed, 
                        "easetype", "linear",
                        "onComplete", "AfterMovement"));
                }
            }
        }
	}

    IEnumerable AfterMovement() {
        // Play the hit sound
        GameManager.instance.PlayAudio(hitSound);
        return null;
    }

    public void AfterDeath() {
        GameManager.instance.PlayAudio(deathSound);
        transform.position = startPosition;
    }
}
