using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    public GameObject[] levels;

    public int currentLevelID { get; private set; }

	void Start() {
	
	}
	
	void Update() {
	
	}

    public void SetCurrentLevel(int id) {
        currentLevelID= id;
    }

    public GameObject NextLevel() {
        if (currentLevelID < levels.Length - 1) {
            currentLevelID++;            
        }

        return CurrentLevel();
    }

    public GameObject CurrentLevel() {
        return levels[currentLevelID];
    }
}
