using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            var Inventory = other.GetComponent<PlayerInventory>().Inventory;
            if (Inventory.ContainsKey(name)) {
                Inventory[name]++;
                Destroy(gameObject);
            }
            else if (Inventory.Keys.Count < 9) {
                Inventory.Add(name, 1);
                Destroy(gameObject);
            }
        }
    }
}