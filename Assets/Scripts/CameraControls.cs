using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

    private const float cameraDistance = 13.0f;
    private const float animationDuration = 1.0f;

	void Start() {

	}

    void OnGUI() {
        if (GameObject.Find("PlayerCube") == null) {
            GUILayout.Label("Game Over");
        }
    }

	void Update() {
        if (iTween.Count(gameObject) == 0) {
            Vector3 movement = Vector3.zero;
            Vector3 rotation = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.D)) {
                movement = transform.right + transform.forward;
                rotation.y = -0.25f;
            } else if (Input.GetKeyDown(KeyCode.A)) {
                movement = -transform.right + transform.forward;
                rotation.y = 0.25f;
            } else if (Input.GetKeyDown(KeyCode.W)) {                
                movement = transform.up + transform.forward;
                rotation.x = 0.25f;
            } else if (Input.GetKeyDown(KeyCode.S)) {
                movement = -transform.up + transform.forward;
                rotation.x = -0.25f;
            } else if (Input.GetKeyDown(KeyCode.Q)) {
                rotation.z = -0.25f;
            } else if (Input.GetKeyDown(KeyCode.E)) {
                rotation.z = 0.25f;
            }

            if (!movement.Equals(Vector3.zero)) {
                Debug.Log(transform.position + movement * cameraDistance);
                iTween.MoveTo(gameObject, transform.position + movement * cameraDistance, animationDuration);
            }

            if (!rotation.Equals(Vector3.zero)) {
                iTween.RotateBy(gameObject, rotation, animationDuration);
            }
        }
	}
}
