using UnityEngine;
using System.Collections.Generic;

public class LayerBehaviour : MonoBehaviour {

    public GameObject cubePrefab;
    public int layerID;

	void Start() {
        List<GameObject> cubes = new List<GameObject>();

        // Instantiate cubes
        foreach (Transform dummy in transform) {
            GameObject newCube = Instantiate(cubePrefab, dummy.transform.position, Quaternion.identity) as GameObject;
            cubes.Add(newCube);
            Destroy(dummy.gameObject);
        }

        // Add cubes as a child node
        foreach (GameObject cube in cubes) {
            cube.transform.parent = transform;
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

	void Update() {
	
	}
}
