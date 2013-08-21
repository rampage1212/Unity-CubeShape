using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;

public class LevelEditorLogic : MonoBehaviour {

    public const int DEFAULT_LAYERS_COUNT = 7;

    public GameObject layerPrefab;
    public GUIStyle layersSelectionStyle;

    private Dictionary<int, GameObject> layers;

    private Transform workplace;

    private LevelInfo levelInfo;
    private GameObject playerCube;
    private GameObject finishCube;

    private Vector3 oldPosition;
    private Quaternion oldRotation;

    public int size { get; private set; }
    private int newSize;

    private bool ortho;
    public float speed = 5.0f;

    // GUI stuff
    private Rect menuRect = new Rect(50, 60, 150, 100);
    private Rect toolsRect = new Rect(50, 180, 150, 75);
    private Rect sizeRect = new Rect(50, 270, 150, 50);
    private Rect miscRect = new Rect(50, 350, 150, 50);
    private Rect layersRect = new Rect(800, 120, 150, 275);

    public int toolsSelection { get; private set; }
    private string[] toolsSelections = {"Start Cube", "Finish Cube", "Standard Cube"};

    public int activeLayer { get; private set; }
    private int layersSelection = DEFAULT_LAYERS_COUNT - 1;
    private string[] layersSelectionTexts;

    private Dictionary<int, bool> layersHidden;
    private Dictionary<int, bool> layersLocked;

    private Dictionary<int, bool> layersHiddenTest;
    private Dictionary<int, bool> layersLockedTest;

    private GameObject border;
    private Vector3 borderOrigin;
    private Vector3 borderStep;

    private string levelName = "test";
    private string directory;

    void Start() {
        layers = new Dictionary<int, GameObject>();

        layersHidden = new Dictionary<int, bool>();
        layersLocked = new Dictionary<int, bool>();
        layersHiddenTest = new Dictionary<int, bool>();
        layersLockedTest = new Dictionary<int, bool>();

        border = GameObject.Find("Border");
        borderOrigin = border.transform.localScale;
        borderStep = borderOrigin / DEFAULT_LAYERS_COUNT;

        levelInfo = new LevelInfo();
        workplace = GameObject.Find("Workplace").transform;
        directory = Application.streamingAssetsPath + "/Levels/";

        // Initialize layers        
        SetLevelSize(DEFAULT_LAYERS_COUNT);
        levelInfo.size = size;
        newSize = size;

        activeLayer = -1;
        ActivateLayer(0);
    }

    void OnGUI() {
        GUI.Box(menuRect, "");
        GUILayout.BeginArea(menuRect);
            if (CubeGUI.Button(GUILayout.Button("New level"))) {
                NewLevel();
            }
        
            levelName = GUILayout.TextField(levelName);

            if (CubeGUI.Button(GUILayout.Button("Load"))) {
                if (levelName != "") {
                    Stream stream = null;
                    try {
                        stream = new FileStream(directory + levelName + ".lvl", FileMode.Open);
                        BinaryFormatter formatter = new BinaryFormatter();

                        // Clear the workplace
                        NewLevel();

                        // Load level from file
                        levelInfo = (LevelInfo) formatter.Deserialize(stream);

                        if (levelInfo.playerCube != null) {
                            playerCube = RestoreCubeAt(levelInfo.playerCube.position(), CubeMaterials.PLAYER_CUBE);
                            levelInfo.playerCube = playerCube.GetComponent<CubeBehaviour>().cube;
                        }

                        if (levelInfo.finishCube != null) {
                            finishCube = RestoreCubeAt(levelInfo.finishCube.position(), CubeMaterials.FINISH_CUBE);
                            levelInfo.finishCube = finishCube.GetComponent<CubeBehaviour>().cube;
                        }

                        for (int i = 0; i < levelInfo.cubes.Count; i++) {
                            levelInfo.cubes[i] = RestoreCubeAt(levelInfo.cubes[i].position(), CubeMaterials.STANDARD_CUBE)
                                .GetComponent<CubeBehaviour>().cube;
                        }
                    } catch (FileNotFoundException) {
                        Debug.Log("File not found: " + levelName);
                    } finally {
                        if (stream != null) {
                            stream.Close();
                        }
                    }
                }
            }

            if (CubeGUI.Button(GUILayout.Button("Save"))) {
                if (levelName != "") {
                    Stream stream = new FileStream(directory + levelName + ".lvl", FileMode.Create);
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, levelInfo);
                    stream.Close();
                }
            }
        GUILayout.EndArea();

        GUI.Box(toolsRect, "");            
        GUILayout.BeginArea(toolsRect);
            toolsSelection = GUILayout.SelectionGrid(toolsSelection, toolsSelections, 1);
        GUILayout.EndArea();

        // Layers size
        GUI.Box(sizeRect, "");
        GUILayout.BeginArea(sizeRect);
            GUILayout.Label("Level size: " + newSize);
            newSize = (int) GUILayout.HorizontalSlider(newSize, 3, 10, GUILayout.Width(140));            
        GUILayout.EndArea();

        GUI.Box(miscRect, "");
        GUILayout.BeginArea(miscRect);
            if (CubeGUI.Button(GUILayout.Button("Main Menu"))) {
                Invoke("BackToMenu", 0.5f);
            }
        GUILayout.EndArea();

        GUI.Box(layersRect, "");
        GUILayout.BeginArea(layersRect);
            GUILayout.Label("Layers");
            GUILayout.BeginHorizontal();
                layersSelection = GUILayout.SelectionGrid(layersSelection, layersSelectionTexts, 1, layersSelectionStyle);

                GUILayout.BeginVertical();
                for (int i = 0; i < layers.Count; i++) {
                    GUILayout.BeginHorizontal();
                    layersHidden[i] = GUILayout.Toggle(layersHidden[i], Resources.Load("eye") as Texture);
                    layersLocked[i] = GUILayout.Toggle(layersLocked[i], Resources.Load("padlock") as Texture);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        GUILayout.EndArea();
    } 

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            GameObject cube = GameObject.FindGameObjectWithTag("marked");
            if (cube) {
                CubeBehaviour cubeBehaviour = cube.GetComponent<CubeBehaviour>();
                if (cubeBehaviour.occupied) {
                    cubeBehaviour.MarkOccupied(false);
                    if (cubeBehaviour.cube == levelInfo.playerCube) {
                        playerCube = null;
                        levelInfo.playerCube = null;
                    } else if (cubeBehaviour.cube == levelInfo.finishCube) {
                        finishCube = null;
                        levelInfo.finishCube = null;
                    } else {
                        levelInfo.cubes.Remove(cubeBehaviour.cube);
                    }
                } else {
                    switch (toolsSelection) {
                        // Player's cube
                        case 0:
                            if (playerCube) {
                                playerCube.GetComponent<CubeBehaviour>().MarkOccupied(false);
                            }
                            
                            cubeBehaviour.MarkOccupied(true);
                            playerCube = cube;
                            levelInfo.playerCube = cubeBehaviour.cube;
                            break;
                        // Finish cube
                        case 1:
                            if (finishCube) {
                                finishCube.GetComponent<CubeBehaviour>().MarkOccupied(false);
                            }
                            
                            cubeBehaviour.MarkOccupied(true);
                            finishCube = cube;
                            levelInfo.finishCube = cubeBehaviour.cube;
                            break;
                        // Standard cube
                        case 2:
                            cubeBehaviour.MarkOccupied(true);
                            levelInfo.cubes.Add(cubeBehaviour.cube);
                            break;
                    }
                }
            }
        }

        if (Input.GetMouseButton(1)) {
            // Rotate vertically
            if (Input.GetMouseButton(0)) {
                if (Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.1f) {
                    workplace.transform.Rotate(workplace.transform.up, Input.GetAxis("Mouse Y") * speed);
                }
            } else {
                // Rotate horizontally
                if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0.1f) {
                    workplace.transform.Rotate(workplace.transform.up, Input.GetAxis("Mouse X") * -speed);
                }
            }
        }

        // Layers
        if (((layersSelectionTexts.Length-1) - layersSelection) != activeLayer) {
            ActivateLayer((layersSelectionTexts.Length - 1) - layersSelection);
        }

        // Layers size
        if (newSize != size) {
            SetLevelSize(newSize);
        }

        // Layers hidden or locked
        for (int i = 0; i < layers.Count; i++) {
            if (layersHidden[i] != layersHiddenTest[i]) {
                layersHiddenTest[i] = layersHidden[i];
                layers[layers.Count-1 - i].GetComponent<LayerBehaviour>().MarkHidden(layersHidden[i]);
            }

            if (layersLocked[i] != layersLockedTest[i]) {
                layersLockedTest[i] = layersLocked[i];
                layers[layers.Count-1 - i].GetComponent<LayerBehaviour>().MarkLocked(layersLocked[i]);
            }
        }

        // Mouse scroll back
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            ActivateLayer(activeLayer - 1);
        }

        // Mouse scroll forward
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            ActivateLayer(activeLayer + 1);
        }
  
        if (iTween.Count(gameObject) == 0) {
            if (Input.GetMouseButtonDown(2)) {
                Vector3 newPosition;
                Vector3 newRotation;

                if (!ortho) {
                    ortho = true;
                    oldPosition = Camera.main.transform.position;
                    oldRotation = Camera.main.transform.rotation;

                    newPosition = new Vector3(0, 16, 0);
                    newRotation = new Vector3(90, 0, 0);
                } else {
                    ortho = false;
                    newPosition = oldPosition;
                    newRotation = oldRotation.eulerAngles;
                }

                iTween.MoveTo(Camera.main.gameObject, iTween.Hash("position", newPosition, "time", 0.3f, "easetype", "linear"));
                iTween.RotateTo(Camera.main.gameObject, iTween.Hash("rotation", newRotation, "time", 0.3f, "easetype", "linear"));
            }
        }

        // Hotkeys
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ActivateLayer(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            ActivateLayer(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            ActivateLayer(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            ActivateLayer(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            ActivateLayer(4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            ActivateLayer(5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            ActivateLayer(6);
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            toolsSelection = 0;
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            toolsSelection = 1;
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            toolsSelection = 2;
        }
    }

    void ActivateLayer(int layerID) {
        if (layerID < 0 || layerID > layers.Count-1) {
            return;
        }

        if (!layersLocked[layers.Count - 1 - layerID] && !layersHidden[layers.Count - 1 - layerID]) {
            // Hide previous layer
            if (activeLayer > -1 && activeLayer < layers.Count) {
                foreach (Transform cube in layers[activeLayer].transform) {
                    cube.GetComponent<CubeBehaviour>().MarkTransparent();
                }
            }

            activeLayer = layerID;
            layersSelection = (layersSelectionTexts.Length - 1) - layerID;

            // Show current layer
            foreach (Transform cube in layers[activeLayer].transform) {
                if (cube.GetComponent<CubeBehaviour>() != null) {
                    cube.GetComponent<CubeBehaviour>().MarkSelected();
                }
            }
        } else {
            if (activeLayer < layerID) {
                ActivateLayer(layerID + 1);
            } else {
                ActivateLayer(layerID - 1);
            }
        }
    }

    void SetLevelSize(int sizeToSet) {
        int oldSize = size;
        size = sizeToSet;
        levelInfo.size = size;

        // Add layers
        if (sizeToSet > oldSize) {
            // Border's scale
            if (oldSize > 0) {
                border.transform.localScale += borderStep * (sizeToSet - oldSize);
            }

            GameObject layersParent = GameObject.Find("Layers");
            for (int y = oldSize; y < sizeToSet; y++) {
                GameObject layer = Instantiate(layerPrefab, new Vector3(0, y, 0), Quaternion.identity) as GameObject;
                layer.transform.parent = layersParent.transform;
                layer.transform.localPosition = new Vector3(0, y, 0);
                layer.transform.localEulerAngles = Vector3.zero;
                layer.GetComponent<LayerBehaviour>().layerID = y;
                layer.GetComponent<LayerBehaviour>().SpawnCubes();
                layers.Add(y, layer);

                layersHidden.Add(y, false);
                layersHiddenTest.Add(y, false);
                layersLocked.Add(y, false);
                layersLockedTest.Add(y, false);
            }
        } else { // Remove layers
            // Border's scale
            border.transform.localScale -= borderStep * (oldSize - sizeToSet);

            for (int y = oldSize - 1; y >= sizeToSet; y--) {
                Destroy(layers[y]);
                layers.Remove(y);
                layersHidden.Remove(y);
                layersHiddenTest.Remove(y);
                layersLocked.Remove(y);
                layersLockedTest.Remove(y);
            }
        }

        // Update the size of existing layers
        for (int i = 0; i < (sizeToSet > oldSize ? oldSize : size); i++) {
            layers[i].GetComponent<LayerBehaviour>().Resize(oldSize);
        }
        
        // Update layers selection texts
        layersSelectionTexts = new string[size];
        for (int i = 0; i < size; i++) {
            layersSelectionTexts[i] = "" + (size - i);
        }

        // Activate bottom layer
        ActivateLayer(0);
    }

    void NewLevel() {
        levelInfo = new LevelInfo();

        foreach (GameObject layer in layers.Values) {
            foreach (Transform cube in layer.transform) {
                cube.GetComponent<CubeBehaviour>().MarkOccupied(false);
                cube.GetComponent<CubeBehaviour>().MarkTransparent();
            }
        }

        ActivateLayer(0);
    }

    GameObject RestoreCubeAt(Vector3 position, CubeMaterials type) {
        foreach (Transform cubeTransform in layers[(int) position.y].transform) {
            Cube cube = cubeTransform.GetComponent<CubeBehaviour>().cube;
            if (cube.position().Equals(position)) {
                cubeTransform.GetComponent<CubeBehaviour>().RestoreState(type);
                return cubeTransform.gameObject;
            }
        }

        return null;
    }

    void BackToMenu() {
        Application.LoadLevel("Menu");
    }
}