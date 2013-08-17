using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

    public AudioSource moveSound;
    public AudioSource rotationSound;

    private const float cameraDistance = 13.0f;
    private const float animationDuration = 0.5f;

    private Vector3 originalPosition;

    private GameLogic game;

	void Start() {
        originalPosition = transform.position;
        game = GameObject.Find("GameLogic").GetComponent<GameLogic>();
	}

    void OnGUI() {
       
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
            } else if (Input.GetKeyDown(KeyCode.Space)) {
                // Return to the original position
                if (!transform.position.Equals(originalPosition)) {
                    iTween.MoveTo(gameObject, iTween.Hash("position", originalPosition,
                        "time", animationDuration, "easetype", "linear"));
                }

                // Return to the original rotation  
                if (!transform.rotation.Equals(Quaternion.identity)) {
                    iTween.RotateTo(gameObject, iTween.Hash("rotation", Vector3.zero,
                        "time", animationDuration, "easetype", "linear", "onComplete", "AfterRotation"));
                }
            }

            if (!movement.Equals(Vector3.zero)) {
                Vector3 newPosition = transform.position + movement * cameraDistance;
                newPosition.Set(Mathf.Round(newPosition.x), Mathf.Round(newPosition.y), Mathf.Round(newPosition.z));

                Hashtable moveTo = new Hashtable();
                moveTo.Add("position", newPosition);
                moveTo.Add("time", animationDuration);
                moveTo.Add("easetype", "linear");
                iTween.MoveTo(gameObject, moveTo);
            }

            if (!rotation.Equals(Vector3.zero)) {
                Hashtable rotateBy = new Hashtable();
                rotateBy.Add("amount", rotation);
                rotateBy.Add("time", animationDuration);
                rotateBy.Add("easetype", "linear");
                rotateBy.Add("onComplete", "AfterRotation");
                iTween.RotateBy(gameObject, rotateBy);
            }

            // Increase moves count if move was made
            if (iTween.Count(gameObject) > 0) {
                game.playerControls.increaseMovesCount();
            }

            // Play sound
            if (iTween.Count(gameObject) == 1) {
                // Just the rotation
                rotationSound.Play();
            } else if (iTween.Count(gameObject) > 1) {
                moveSound.Play();
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
}
