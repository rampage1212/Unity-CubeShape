using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

    private AudioClip moveSound;
    private AudioClip rotateSound;

    private Vector3 originalPosition;
    private float cameraDistance;

    private const float animationDuration = 0.5f;

    private GameLogic game;

	void Start() {
        game = GameObject.Find("GameLogic").GetComponent<GameLogic>();

        moveSound = Resources.Load("move") as AudioClip;
        rotateSound = Resources.Load("rotate") as AudioClip;
	}

    public void SavePosition() {
        originalPosition = transform.position;
        cameraDistance = Vector3.Distance(originalPosition, game.mapCenter);
    }

	void Update() { 
        // If the object is not moving, check for input
        if (iTween.Count(gameObject) == 0 && 
            game.playerCube != null && iTween.Count(game.playerCube.gameObject) == 0) {
            Vector3 movement = Vector3.zero;
            Vector3 rotation = Vector3.zero;

            // Check for user input and decide on new movement and/or rotation
            // The 0.25f values are just 90 degrees
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
            } else if (Input.GetKeyDown(KeyCode.Space)) {
                // Return to the original position
                if ((transform.position - originalPosition).magnitude > 0.1f) {
                    iTween.MoveTo(gameObject, iTween.Hash("position", originalPosition,
                        "time", animationDuration, "easetype", "linear"));
                }

                // Return to the original rotation  
                if (!transform.rotation.Equals(Quaternion.identity)) {
                    iTween.RotateTo(gameObject, iTween.Hash("rotation", Vector3.zero,
                        "time", animationDuration, "easetype", "linear", 
                        "onComplete", "AfterRotation"));
                }
            }

            // If new position is set, translate
            if (!movement.Equals(Vector3.zero)) {
                Vector3 newPosition = transform.position + movement * cameraDistance;
                
                iTween.MoveTo(gameObject, iTween.Hash("position", newPosition,
                    "time", animationDuration,
                    "easetype", "linear"));
            }

            // If new rotation is set, rotate
            if (!rotation.Equals(Vector3.zero)) {
                iTween.RotateBy(gameObject, iTween.Hash("amount", rotation,
                    "time", animationDuration,
                    "easetype", "linear",
                    "onComplete", "AfterRotation"));
            }

            // Increase moves count if any move was made
            if (iTween.Count(gameObject) > 0) {
                game.IncreaseMovesCount();
            }

            // Play sound
            if (iTween.Count(gameObject) == 1) {
                // Just the rotation
                GameManager.instance.PlayAudio(rotateSound);    
            } else if (iTween.Count(gameObject) > 1) {
                GameManager.instance.PlayAudio(moveSound);
            }
        }
	}

    IEnumerable AfterRotation() {
        // Round the angles
        transform.eulerAngles = new Vector3(
            Mathf.Round(transform.rotation.eulerAngles.x),
            Mathf.Round(transform.rotation.eulerAngles.y),
            Mathf.Round(transform.rotation.eulerAngles.z));
        return null;
    }

    public void ReturnToOriginalPosition() {
        transform.position = originalPosition;
        transform.rotation = Quaternion.identity;
    }
}
