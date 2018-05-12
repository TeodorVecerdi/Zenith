using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    private TMP_Text InventoryDebugText;
    private TMP_Text InventorySelectedType;
    private TMP_Text MousePosition;
    private TileMap TileMap;
    public Dictionary<string, int> Inventory;
    public string selectedType;

    void Start() {
        TileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
        Inventory = new Dictionary<string, int>();
        Debug.Log("PLAYERINVENTORY:START(): DEBUG ADD ITEMS TO INVENTORY");
        Inventory.Add("grass", 1000);
        Inventory.Add("dirt", 1000);
        Inventory.Add("stone", 1000);
        Inventory.Add("bedrock", 1000);
        InventoryDebugText = GameObject.Find("InventoryDebugText").GetComponent<TMP_Text>();
        InventorySelectedType = GameObject.Find("InventorySelectedType").GetComponent<TMP_Text>();
        MousePosition = GameObject.Find("MousePositionDebugText").GetComponent<TMP_Text>();
        setSelected("air");
    }

    void Update() {
        MousePosition.text = "px:"+(int)transform.position.x+"\npy:"+(int)transform.position.y+"\nmx:" + (int)Camera.main.ScreenToWorldPoint(Input.mousePosition).x + "\nmy:" + (int)Camera.main.ScreenToWorldPoint(Input.mousePosition).y +
                             "\n GH:" + TileMap.Planet.GenHeight[(int) Camera.main.ScreenToWorldPoint(Input.mousePosition).x];
        InventoryDebugText.text = "INVENTORY:\n";
        if (Inventory.Count == 0)
            InventoryDebugText.text += "EMPTY";
        else
            foreach (KeyValuePair<string, int> keyValuePair in Inventory) {
                InventoryDebugText.text += TileMap.Database.ItemDatabase.ItemDictionary[TileMap.Database.TileIDToItemID(keyValuePair.Key)].name + ": " +
                                           keyValuePair.Value + "\n";
            }
    }

    public void AddItem(string item) {
        if (!Inventory.ContainsKey(item))
            Inventory.Add(item, 0);
        Inventory[item]++;
    }

    public void RemoveItem(string item) {
        Inventory[item]--;
        if (Inventory[item] == 0) {
            Inventory.Remove(item);
            if (selectedType == item)
                setSelected(Inventory.Count > 0 ? Inventory.First().Key : "air");
        }
    }

    public bool HasItem(string item) {
        return Inventory.ContainsKey(item);
    }

    public void setSelected(int hotbar) {
        int i = 1;
        foreach (string key in Inventory.Keys) {
            if (i == hotbar) {
                setSelected(key);
                return;
            }
            i++;
        }
    }

    public void setSelected(string selected) {
        selectedType = selected;
        InventorySelectedType.text = "SELECTED:\n" + (selectedType != "air"
                                         ? TileMap.Database.ItemDatabase.ItemDictionary[TileMap.Database.TileIDToItemID(selected)].name
                                         : "NOTHING");
    }

    public string getSelected() {
        return selectedType;
    }
}