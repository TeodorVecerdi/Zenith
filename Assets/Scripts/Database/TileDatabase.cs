using System.Collections.Generic;
using UnityEngine;

public class TileDatabase {
    public Dictionary<string, Tile> TileDictionary;
    public Dictionary<string, string> ItemToTileConverter;

    public TileDatabase() {
        TileDictionary = new Dictionary<string, Tile>();
        ItemToTileConverter = new Dictionary<string, string>();
    }

    public TileDatabase Load() {
//        Debug.Log("Started tile database loading");
        string tilesJSON = Resources.Load<TextAsset>("data/Tiles").text;
//        Debug.Log(tilesJSON);
        Tile[] tileArray = JsonHelper.FromJson<Tile>(tilesJSON);
//        Debug.Log(tileArray);
        foreach (Tile tile in tileArray) {
//            tile.DebugPrint();
            TileDictionary.Add(tile.id, tile);
            ItemToTileConverter.Add(tile.itemID, tile.id);
        }
//        Debug.Log("Finished tile database loading");
        return this;
    }

    public string GetTileFromItem(string itemID) {
        return ItemToTileConverter[itemID];
    }
}