using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase {
    public Dictionary<string, Item> ItemDictionary;
    public Dictionary<string, Sprite> Sprites;
    public ItemDatabase() {
        ItemDictionary = new Dictionary<string, Item>();
        Sprites = new Dictionary<string, Sprite>();
    }

    public ItemDatabase Load() {
        string itemsJSON = Resources.Load<TextAsset>("data/Items").text;
        Item[] itemArray = JsonHelper.FromJson<Item>(itemsJSON);
        foreach (Item item in itemArray) {
            ItemDictionary.Add(item.id, item);
            Sprite currentSprite = Resources.Load<Sprite>("itemTextures-assets/" + item.id);
//            Debug.Log("itemID: " + item.id + " sprite: " + currentSprite);
            if(currentSprite != null)
                Sprites.Add(item.id, currentSprite);
//            item.DebugPrint();
        }
//        Debug.Log("Finished item database loading");
        return this;
    }
}