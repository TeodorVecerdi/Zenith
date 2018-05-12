using System;

[Serializable]
public class World {
    public WorldSize[] sizes;
    public double[] heightBoost;
    public string[] biomes;
    public int[] oreSamples;
    public string[] ores;
    public float[] oreLengthBoost;
    public float[] caveLengthBoost;
}

[Serializable]
public class WorldSize {
    public int x;
    public int y;

    public override string ToString() {
        return "WorldSize["+x+","+y+"]";
    }
}