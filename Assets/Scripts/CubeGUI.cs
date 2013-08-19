using UnityEngine;
using System.Collections;

public class CubeGUI : MonoBehaviour {
    
    // GUI Sounds
    private static AudioClip buttonSound;

    void Awake() {
        buttonSound = Resources.Load("buttonClick") as AudioClip;
    }

    public static bool Button(bool button) {
        if (button) {
            GameManager.instance.PlayAudio(buttonSound);
        }
        
        return button;
    }
}
