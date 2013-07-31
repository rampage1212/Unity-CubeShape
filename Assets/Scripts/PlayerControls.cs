using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

    const float animationSpeed = 15.0f;
    const int mapSize = 10;

    public AudioSource hitSound;
    public AudioSource deathSound;
    public Vector3 movement;

    public int movesCount { get; private set; }

    public bool finished { get; private set; }

	void Start() {
        movement = Vector3.zero;
        movesCount = 0;
	}

	void Update() {

        if (!finished && iTween.Count(gameObject) == 0
            && iTween.Count(Camera.main.gameObject) == 0) {
            // if not moving at the moment
            Transform direction = Camera.main.transform;
            movement = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                movement = -direction.right;
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                movement = direction.right;
            } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                movement = direction.up;
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                movement = -direction.up;
            }

            if (!movement.Equals(Vector3.zero)) {
                int i;
                Collider[] colliders;
                for (i = 1; i <= mapSize; i++) {
                    colliders = Physics.OverlapSphere(transform.position + movement * i, 0.1f);
                    if (colliders.Length > 0) {
                        // Collision with FinishCube
                        if (colliders[0].gameObject.tag == "FinishCube") {
                            finished = true;
                        }
                        i--;
                        break;
                    }
                }

                if (i > 0) {
                    increaseMovesCount();
                    iTween.MoveTo(gameObject, iTween.Hash("position", transform.position + movement * i,
                        "speed", animationSpeed, "easetype", "linear",
                        "onComplete", "AfterMovement"));
                }
            }
        }
	}

    IEnumerable AfterMovement() {
        hitSound.Play();
        return null;
    }

    public void resetMovesCount() {
        movesCount = 0;
    }

    public void increaseMovesCount() {
        movesCount++;
    }
}
