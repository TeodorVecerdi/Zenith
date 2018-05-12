using System;
using UnityEngine;

[Serializable]
public class Tile {
    public string id;
    public string itemID;
    public Vector2 texture;
    public Properties properties;

    public void DebugPrint() {
        Debug.Log(string.Format("{0}: {1}, {2}, {3}, {4}, {5}", id, itemID, texture, properties.hardness, properties.undestructible, properties.transparent));
    }
}

[Serializable]
public class Properties {
    public double hardness = 0;
    public bool undestructible = false;
    public bool transparent = false;
}