using UnityEngine;
using System.Collections;

public class CubeGUI : MonoBehaviour {

    private static AudioClip buttonSound;

    static CubeGUI() {
        buttonSound = Resources.Load("buttonClick") as AudioClip;
    }

    public static bool Button(bool button) {
        if (button) {
            GameManager.logic.PlayAudio(buttonSound, Camera.main.transform);
        }
        
        return button;
    }
}
