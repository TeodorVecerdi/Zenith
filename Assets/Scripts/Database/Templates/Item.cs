using System;
using UnityEngine;

[Serializable]
public class Item {
    public string id;
    public string name;
    public string description;
    public void DebugPrint() {
        Debug.Log(string.Format("\"{0}\":{{\n \"name\": \"{1}\", \"description\": \"{2}\"\n}}", id, name, description));
    }
}
