using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase {
    public Dictionary<string, Item> ItemDictionary;
    public ItemDatabase() {
        ItemDictionary = new Dictionary<string, Item>();
    }

    public ItemDatabase Load() {
        string itemsJSON = Resources.Load<TextAsset>("data/Items").text;
        Item[] itemArray = JsonHelper.FromJson<Item>(itemsJSON);
        foreach (Item item in itemArray) {
            ItemDictionary.Add(item.id, item);
//            item.DebugPrint();
        }
//        Debug.Log("Finished item database loading");
        return this;
    }
}