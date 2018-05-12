using System;
using UnityEngine;

[Serializable]
public class Structure {
    public Vector2 size;
    public StructureTile[] tiles;
}

[Serializable]
public class StructureTile {
    public Vector2 pos;
    public string tile;
}