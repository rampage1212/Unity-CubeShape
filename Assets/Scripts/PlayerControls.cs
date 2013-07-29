using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

    const float animationDuration = 10.0f;
    const int mapSize = 10;

    public Vector3 movement;

	void Start() {
        movement = Vector3.zero;
	}

	void Update() {

        if (Vector3.Distance(transform.position, Vector3.zero) > 10.0f) {
            Destroy(gameObject);
            return;
        }

        if (iTween.Count(gameObject) == 0) {
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

                if (i > mapSize) {
                    i = 20;
                }

                iTween.MoveTo(gameObject, iTween.Hash("position", transform.position + movement * i, 
                    "speed", animationDuration, "easetype", iTween.EaseType.linear));
            }
        }
	}
}
