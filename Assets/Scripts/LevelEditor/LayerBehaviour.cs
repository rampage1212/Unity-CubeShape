using UnityEngine;
using System.Collections.Generic;

public class LayerBehaviour : MonoBehaviour {

    public GameObject cubePrefab;
    public int layerID;

	void Start() {

	}

    public void SpawnCubes() {
        int levelSize = Camera.main.GetComponent<LevelEditorLogic>().size;
       
        int half = Mathf.FloorToInt(levelSize / 2);
        int from, to;
        if (levelSize % 2 == 0) {
            from = -(half - 1);
            to = half;
        } else {
            from = -half;
            to = half;
        }

        foreach (Transform cube in transform) {
            Destroy(cube);
        }

        for (int x = from; x <= to; x++) {
            for (int z = from; z <= to; z++) {
                SpawnCube(x, z);
            }
        }
    }

    public void Resize(int oldSize) {
        int levelSize = Camera.main.GetComponent<LevelEditorLogic>().size;

        // Add cubes
        if (levelSize > oldSize) {
            for (int i = oldSize + 1; i <= levelSize; i++) {
                int i_half = Mathf.FloorToInt(i / 2);
                // "Even" layer
                if (i % 2 == 0) {
                    // Horizontal part
                    for (int x = -(i_half - 1); x <= i_half; x++) {
                        SpawnCube(x, i_half);
                    }

                    // Vertical part
                    for (int z = -(i_half - 1); z <= i_half - 1; z++) {
                        SpawnCube(i_half, z);
                    }
                } else { // "Odd" layer
                    // Horizontal part
                    for (int x = -i_half; x <= i_half; x++) {
                        SpawnCube(x, -i_half);
                    }

                    // Vertical part
                    for (int z = -(i_half - 1); z <= i_half ; z++) {
                        SpawnCube(-i_half, z);
                    }
                }
            }
        } else { // Remove cubes
            for (int i = oldSize; i > levelSize; i--) {
                int i_half = Mathf.FloorToInt(i / 2);

                foreach (Transform cube in transform) {
                    // "Even" layer
                    if (i % 2 == 0) {
                        if (cube.localPosition.x >= i_half ||
                            cube.localPosition.z >= i_half) {
                            Destroy(cube.gameObject);
                        }
                    } else { // "Odd" layer
                        if (cube.localPosition.x <= -i_half ||
                            cube.localPosition.z <= -i_half) {
                            Destroy(cube.gameObject);
                        }
                    }
                }
            }
        }
    }

    private void SpawnCube(int x, int z) {
        GameObject cube = Instantiate(cubePrefab, new Vector3(x, layerID, z), Quaternion.identity) as GameObject;
        cube.GetComponent<CubeBehaviour>().SetCube(x, layerID, z);
        cube.transform.parent = transform;
        cube.transform.localPosition = new Vector3(x, 0, z);
        cube.transform.localEulerAngles = Vector3.zero;
    }

    public void MarkHidden(bool hidden) {
        foreach (Transform cube in transform) {
            cube.GetComponent<CubeBehaviour>().MarkHidden(hidden);
        }
    }

    public void MarkLocked(bool locked) {
        foreach (Transform cube in transform) {
            cube.GetComponent<CubeBehaviour>().MarkLocked(locked);
        }
    }
}
