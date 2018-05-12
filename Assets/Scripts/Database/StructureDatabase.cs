using System.Collections.Generic;
using UnityEngine;

public class StructureDatabase {
    public Dictionary<string, Structure[]> StructureDictionary;

    public StructureDatabase() {
        StructureDictionary = new Dictionary<string, Structure[]>();
    }

    public StructureDatabase Load() {
        string structures = Resources.Load<TextAsset>("data/StructureList").text;
        string[] structureTypes = JsonHelper.FromJson<string>(structures);
        foreach (string structureType in structureTypes) {
            string structuresJSON = Resources.Load<TextAsset>("structures/" + structureType).text;
            Structure[] structureArray = JsonHelper.FromJson<Structure>(structuresJSON);
            StructureDictionary.Add(structureType, structureArray);
        }
        return this;
    }
}