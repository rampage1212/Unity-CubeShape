using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

    const float animationDuration = 15.0f;
    const int mapSize = 10;

    public AudioSource hitSound;
    public AudioSource deathSound;
    public Vector3 movement;

	void Start() {
        movement = Vector3.zero;
	}

	void Update() {

        if (iTween.Count(gameObject) == 0
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
                for (i = 1; i <= mapSize; i++) {
                    if (Physics.OverlapSphere(transform.position + movement * i, 0.1f).Length > 0) {
                        i--;
                        break;
                    }
                }

                if (i > 0) {
                    iTween.MoveTo(gameObject, iTween.Hash("name", "playerMovement", 
                        "position", transform.position + movement * i,
                        "speed", animationDuration, "easetype", iTween.EaseType.linear,
                        "onComplete", "AfterMovement"));
                }
            }
        }
	}

    IEnumerable AfterMovement() {
        hitSound.Play();
        return null;
    }
}
