using UnityEngine;
using System.Collections;

public class CubeGUI : MonoBehaviour {
    
    // GUI Sounds
    private static AudioClip buttonSound;

    //private static Rect PopupDialogBackground = CenterRect(400, 200);

    void Awake() {
        buttonSound = Resources.Load("buttonClick") as AudioClip;
    }

    public static bool Button(bool button) {
        if (button) {
            GameManager.instance.PlayAudio(buttonSound);
        }
        
        return button;
    }
    
    public static Rect CenterRect(float width, float height) {
        return new Rect(Screen.width / 2.0f - width / 2.0f, 
                        Screen.height / 2.0f - height / 2.0f, 
                        width, height);
    }
}
