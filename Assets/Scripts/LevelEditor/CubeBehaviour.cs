using UnityEngine;
using System.Collections;

public enum CubeMaterials {
    PLAYER_CUBE = 0,
    FINISH_CUBE,
    STANDARD_CUBE,
    TRANSPARENT,
    SELECTED,
    GOLD
}

public class CubeBehaviour : MonoBehaviour {

    public Material[] materials;

    public bool occupied { get; private set; }
    private bool selected;
    private bool hidden;
    private bool locked;
    public Cube cube { get; private set; }

    private Material markedMaterial;

	void Start() {
        cube = new Cube(transform.position);
	}

	void Update() {
	
	}
        
    void OnMouseOver() {
        if (occupied) {
            renderer.material = material(CubeMaterials.GOLD);
        } else {
            renderer.material = materials[Camera.main.GetComponent<LevelEditorLogic>().toolsSelection];
        } 

        gameObject.tag = "marked";
    }

    void OnMouseExit() {
        if (occupied) {
            renderer.material = markedMaterial;
        } else {
            if (GetComponent<BoxCollider>().enabled) {
                renderer.material = material(CubeMaterials.SELECTED);
            } else {
                renderer.material = material(CubeMaterials.TRANSPARENT);
            }
        }

        gameObject.tag = "Untagged";
    }

    public void MarkTransparent() {
        if (!occupied) {
            renderer.material = material(CubeMaterials.TRANSPARENT);
        }
         
        GetComponent<BoxCollider>().enabled = false;
        selected = false;
    }

    public void MarkSelected() {
        if (!occupied) {
            renderer.material = material(CubeMaterials.SELECTED);
        }

        if (!locked && !hidden) {
            GetComponent<BoxCollider>().enabled = true;
        }
        selected = true;
    }

    public void MarkOccupied(bool occupy) {
        occupied = occupy;

        renderer.material = materials[Camera.main.GetComponent<LevelEditorLogic>().toolsSelection];

        if (occupy) {            
            markedMaterial = renderer.material;
        } else {
            if (GetComponent<BoxCollider>().enabled) {
                renderer.material = material(CubeMaterials.SELECTED);
            } else {
                renderer.material = material(CubeMaterials.TRANSPARENT);
            }

            markedMaterial = null;
        }
    }

    public void MarkHidden(bool _hidden) {
        hidden = _hidden;

        if (hidden) {
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(true);
            if (selected && !locked) {
                GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    public void MarkLocked(bool _locked) {
        locked = _locked;
        if (selected) {
            if (locked) {
                GetComponent<BoxCollider>().enabled = false;
            } else {
                GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    public void RestoreState(CubeMaterials newMaterial) {
        MarkOccupied(true);
        renderer.material = material(newMaterial);
        markedMaterial = renderer.material;
    }

    Material material(CubeMaterials materialID) {
        return materials[(int) materialID];
    }
}
