using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {
    public int Count;
    public bool Collided;

    private void Awake() {
        Count = 1;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("ItemPickup")) {
            Physics2D.IgnoreCollision(GetComponent<PolygonCollider2D>(), other.collider);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            var Inventory = other.GetComponent<PlayerInventory>().Inventory;
            if (Inventory.ContainsKey(name)) {
                Inventory[name] += Count;
                Destroy(gameObject);
            }
            else if (Inventory.Keys.Count < 9) {
                Inventory.Add(name, Count);
                Destroy(gameObject);
            }
        }
    }


    private void Update() {
        Collided = false;

//        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        Collider2D[] col = Physics2D.OverlapBoxAll(transform.position, new Vector2(1.2f, 1.2f), 0f);
        foreach (Collider2D c in col) {
            if (c.CompareTag("ItemPickup") && c.name == name && !Collided && !c.gameObject.Equals(gameObject)) {
                if (c.GetComponent<ItemPickup>().Collided) return;
                Collided = true;
                Count += c.GetComponent<ItemPickup>().Count;
                Destroy(c.gameObject);
            }
        }
    }
}