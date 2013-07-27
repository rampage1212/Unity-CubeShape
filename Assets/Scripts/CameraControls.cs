using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

    private const float cameraDistance = 10.0f;
    private const float animationDuration = 1.0f;

	void Start() {

	}

	void Update() {
        if (iTween.Count(gameObject) == 0) {
            Vector3 movement = Vector3.zero;
            Vector3 rotation = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                movement = transform.right + transform.forward;
                rotation.y = -0.25f;
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                movement = -transform.right + transform.forward;
                rotation.y = 0.25f;
            } else if (Input.GetKeyDown(KeyCode.UpArrow)) {                
                movement = transform.up + transform.forward;
                rotation.x = 0.25f;
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                movement = -transform.up + transform.forward;
                rotation.x = -0.25f;
            }

            if (!movement.Equals(Vector3.zero) || !rotation.Equals(Vector3.zero)) {
                iTween.MoveTo(gameObject, transform.position + movement * cameraDistance, animationDuration);
                iTween.RotateBy(gameObject, rotation, animationDuration);
            }
        }
	}
}
