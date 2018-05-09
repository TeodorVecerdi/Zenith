    using System.Collections.Generic;
    using UnityEngine;

public class BiomeDatabase {
    public Dictionary<string, Biome> BiomeDictionary;

    public BiomeDatabase() {
        BiomeDictionary = new Dictionary<string, Biome>();
    }

    public BiomeDatabase Load() {
//        Debug.Log("Started biome database loading");
        string biomeJSON = Resources.Load<TextAsset>("data/Biomes").text;
        Biome[] biomeArray = JsonHelper.FromJson<Biome>(biomeJSON);
        foreach (Biome biome in biomeArray) {
            BiomeDictionary.Add(biome.id, biome);
//            item.DebugPrint();
        }
//        Debug.Log("Finished biome database loading");
        return this;
    }
}