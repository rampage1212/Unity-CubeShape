using UnityEngine;
using System.Collections.Generic;

public class LevelPack {

    public string packName;
    public List<LevelInfo> levels { get; private set; }

    public LevelPack(string _packName) {
        packName = _packName;
        levels = new List<LevelInfo>();
    }

    public void AddLevel(LevelInfo level) {
        levels.Add(level);
    }
}
