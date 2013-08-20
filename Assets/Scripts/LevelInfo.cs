using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class LevelInfo {

    public int size;
    public Cube playerCube;
    public Cube finishCube;
    public List<Cube> cubes { get; private set; }

    public LevelInfo() {
        if (cubes == null) {
            cubes = new List<Cube>();
        }
    }
}

[Serializable]
public class Cube {

    public int x, y, z;

    public Cube(Vector3 position) {
        x = (int) Mathf.Round(position.x);
        y = (int) Mathf.Round(position.y);
        z = (int) Mathf.Round(position.z);
    }

    public Vector3 position() {
        return new Vector3(x, y, z);
    }
}