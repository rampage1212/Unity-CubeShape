using UnityEngine;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour {

    public GUISkin HUDSkin;

    public GameObject playerCubePrefab;
    public GameObject finishCubePrefab;
    public GameObject standardCubePrefab;
    
    // Player stuff
    public Transform playerCube { get; private set; }
    public PlayerControls playerControls { get; private set; }

    // Level info
    private GameObject finishCube;
    private List<GameObject> cubes;

    // Moves counter
    public int movesCount { get; private set; }

    private Vector3 cameraOrigin;
    private Vector3 borderOrigin;

    private Vector3 cubesOrigin;
    private Vector3 cubesOffset;

    public int levelSize { get; private set; }
    public Vector3 mapCenter { get; private set; }

    private LevelManager levelManager;

    private const float cameraDistance = 10.0f;

	void Start() {
        if (GameObject.Find("LevelManager") != null) {
            cameraOrigin = Camera.main.transform.position;
            borderOrigin = GameObject.Find("Border").transform.localScale;

            cubesOrigin = GameObject.Find("Cubes").transform.localPosition;
            cubesOffset = new Vector3(0.5f, 0, 0.5f);

            cubes = new List<GameObject>();

            levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            InitializeLevel(levelManager.CurrentLevel());
        } else {
            Application.LoadLevel("Menu");
        }
	}

    void InitializeLevel(LevelInfo level) {
        ResetMovesCount();

        // Restore original camera position
        Camera.main.transform.position = cameraOrigin;
        Camera.main.transform.rotation = Quaternion.identity;
        
        // Set level size
        SetLevelSize(level);

        // Instantiate cubes and set their parent
        GameObject cubesParent = GameObject.Find("Cubes");

        GameObject playerObject = Instantiate(playerCubePrefab, 
            level.playerCube.position(), Quaternion.identity) as GameObject;
        playerObject.transform.parent = cubesParent.transform;
        playerObject.transform.localPosition = level.playerCube.position();
        playerCube = playerObject.transform;

        finishCube = Instantiate(finishCubePrefab, level.finishCube.position(), Quaternion.identity) as GameObject;
        finishCube.transform.parent = cubesParent.transform;
        finishCube.transform.localPosition = level.finishCube.position();

        foreach (Cube standardCube in level.cubes) {
            GameObject cube = Instantiate(standardCubePrefab, standardCube.position(), Quaternion.identity) as GameObject;
            cube.transform.parent = cubesParent.transform;
            cube.transform.localPosition = standardCube.position();
            cubes.Add(cube);
        }

        playerControls = playerCube.GetComponent<PlayerControls>();
    }

    void OnGUI() {
        GUI.skin = HUDSkin;

        if (levelManager != null) {
            GUILayout.Label("Level " + (levelManager.currentLevelID + 1));
        } else {
            GUILayout.Label("Test Level");
        }

        GUILayout.Label("Moves: " + movesCount);

        // Return to LevelEditor or Main Menu
        if (levelManager.TestMode()) {
            if (CubeGUI.Button(GUILayout.Button("Return to Level Editor"))) {
                Application.LoadLevel("LevelEditor");
            }
        } else {
            if (CubeGUI.Button(GUILayout.Button("Return to Level select"))) {
                Application.LoadLevel("Menu");
            }

            if (CubeGUI.Button(GUILayout.Button("Return to Main Menu"))) {
                levelManager.Reset();
                Application.LoadLevel("Menu");
            }
        }
    }

    public void NextLevel() {
        // Clear the level
        Destroy(playerCube.gameObject);
        Destroy(finishCube);

        foreach (GameObject cube in cubes) {
            Destroy(cube);
        }
        cubes.Clear();

        if (levelManager != null) {
            // Load next level
            InitializeLevel(levelManager.NextLevel());
        }

    }

    private void SetLevelSize(LevelInfo level) {
        levelSize = level.size;
        GameObject border = GameObject.Find("Border");

        Vector3 borderStep = borderOrigin / LevelEditorLogic.DEFAULT_LAYERS_COUNT;
        int steps = level.size - LevelEditorLogic.DEFAULT_LAYERS_COUNT;

        border.transform.localScale = borderOrigin + borderStep * steps;

        // Update cubes position
        GameObject allCubes = GameObject.Find("Cubes");
        if (level.size % 2 == 0) {
            allCubes.transform.localPosition = cubesOrigin - cubesOffset;
        } else {
            allCubes.transform.localPosition = cubesOrigin;
        }

        // Update camera position
        Vector3 newPosition = border.renderer.bounds.center -
            new Vector3(0, 0, border.renderer.bounds.center.y + level.size * 1.1f);

        Camera.main.transform.position = newPosition;
        Camera.main.transform.rotation = Quaternion.identity;
        Camera.main.GetComponent<CameraControls>().SavePosition();

        mapCenter = border.renderer.bounds.center;
    }

    public void ResetMovesCount() {
        movesCount = 0;
    }

    public void IncreaseMovesCount() {
        movesCount++;
    }

    public bool Finished() {
        if (playerControls == null) {
            return true;
        } else {
            return playerControls.finished;
        }
    }
}
