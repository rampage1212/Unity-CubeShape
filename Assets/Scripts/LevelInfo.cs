using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class LevelInfo {

    public Cube playerCube;
    public Cube finishCube;
    public Cube[] cubes;
    //public List<Cube> cubes { get; private set; }

    public LevelInfo(Cube player, Cube finish) {
        playerCube = player;
        finishCube = finish;
      //  cubes = new List<Cube>();
    }

    public void AddCube(Cube cube) {
        //cubes.Add(cube);
    }
}

[Serializable]
public class Cube {

    public int x, y, z;

    public Cube(int _x, int _y, int _z) {
        x = _x;
        y = _y;
        z = _z;        
    }

    public Vector3 position() {
        return new Vector3(x, y, z);
    }
}