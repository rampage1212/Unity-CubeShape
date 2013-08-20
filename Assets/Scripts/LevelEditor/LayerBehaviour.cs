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
                GameObject cube = Instantiate(cubePrefab, new Vector3(x, layerID, z), Quaternion.identity) as GameObject;
                cube.transform.parent = transform;
            }
        }
    }

    public void Resize(int oldSize) {
        int levelSize = Camera.main.GetComponent<LevelEditorLogic>().size;
        int half = Mathf.FloorToInt(levelSize / 2);

        // Add cubes
        if (levelSize > oldSize) {

        } else { // Remove cubes
            foreach (Transform cube in transform) {
                if (Mathf.Abs(cube.position.x) > half ||
                    Mathf.Abs(cube.position.z) > half) {
                        Destroy(cube.gameObject);
                }
            }
        }
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
