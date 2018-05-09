using UnityEngine;

public class WorldDatabase {
    public World World;
    public WorldDatabase() {
        World = new World();
    }

    public void Load() {
        string worldJSON = Resources.Load<TextAsset>("data/Worlds").text;
        World = JsonUtility.FromJson<World>(worldJSON);
    }
}