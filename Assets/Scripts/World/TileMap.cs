using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;

public class TileMap : MonoBehaviour {
    [Header("Prefabs")]
    public GameObject ChunkPrefab;
    public GameObject PlayerPrefab;
    [Space]
    [Header("Object References")] 
    public GeneralManager GeneralManager;
    public Animator SplashScreenAnimator;
    public GameObject GameBackground;

    public Planet Planet;

    private int selectedType;

    private System.Random GameRNG;
    public PlanetSettings PlanetSettings;
    public Database Database;

    public void Create(System.Random GameRNG, Database database, PlanetSettings planetSettings) {
        this.GameRNG = GameRNG;
        PlanetSettings = planetSettings;
        Database = database;
        StartCoroutine(GenWorldOnSecondThread(GameRNG));
    }

    private void AddChunks() {
        int startX = (int) Planet.getChunkIndex((int)Planet.getSpawnPoint().x, (int)Planet.getSpawnPoint().y).x - 1;
        int startY = (int) Planet.getChunkIndex((int) Planet.getSpawnPoint().x, (int) Planet.getSpawnPoint().y).y - 1;
        for (int i = startY; i < startY + 3; i++)
        for (int j = startX; j < startX + 3; j++) {
            GameObject chunk = Instantiate(ChunkPrefab, new Vector3(j * Utils.CHUNKWIDTH, i * Utils.CHUNKHEIGHT), Quaternion.identity);
            chunk.name = "Chunk@" + j + "," + i;
            chunk.transform.parent = transform;
            chunk.GetComponent<Chunk>().Initialize(Planet, j * Utils.CHUNKWIDTH, i * Utils.CHUNKHEIGHT);
            chunk.GetComponent<Chunk>().Build();
        }
    }

    private void FinishedGenerating(Planet world) {
        Planet = world;
        AddChunks();
        GameBackground.SetActive(true);
        GameObject.Find("GameNameText").GetComponent<TMP_Text>().text = "Game Name: " + GeneralManager.GameManager.GameName;
        GetComponent<WorldManager>().Initialize(PlayerPrefab, this);
    }

    private IEnumerator GenWorldOnSecondThread(System.Random GameRNG) {
        Planet world = null;
        GeneralManager.StartLoading();
        Thread thread = new Thread(() => {
            GeneralManager.SetStatus("Initializing World");
            world = new Planet(Database, PlanetSettings, GameRNG);
            GeneralManager.SetStatus("Generating Terrain");
            world.Generate(GeneralManager.SetStatus);
        });
        thread.Start();
        while (thread.IsAlive) {
            yield return null;
        }
        GeneralManager.EndLoading();
        FinishedGenerating(world);
    }
    
    
}