using UnityEngine;

public class PlanetSettings {
    public WorldSize worldSize;

    public string[] ores;
    public double heightBoost;
    public double oreLengthBoost;
    public double caveLengthBoost;
    
    public string biome;
    public string surfaceTile;
    public string middleTile;
    public string undergroundTile;
    public string[] features;

    public void DebugPrint() {
        Debug.Log(string.Format("worldSize: {0}\noreSamples: {1}\nores: {2}\noreLengthBoost: {3}\ncaveLengthBoost: {4}\nbiome: {5}\nsurfaceTile: {6}\nundergroundTile: {7}\nfeatures: {8}", worldSize, oreLengthBoost, ores, oreLengthBoost, caveLengthBoost, biome, surfaceTile, undergroundTile, features));
    }
}