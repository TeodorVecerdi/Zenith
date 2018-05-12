using System;
using UnityEngine;

public class WorldManager : MonoBehaviour {
    [Header("Script References")] 
    public TargetFollow CameraFollow;
    [Space] 
    [Header("Prefabs")] 
    public GameObject ItemPickupPrefab;

    private TileMap tileMap;
    private bool shouldUpdate;

    //ChunkHandler
    private bool shouldFindChunks;
    private int chunkLeft, chunkBottom;
    private GameObject[,] chunks;

    //Block Breaking
    public float MaxBlockBreakingDistance = 10f;
    private Vector2Int currentBlockPosition;
    private double currentMineTime;
    private double requiredMineTime;
    private bool isCurrentlyMining;

    //Player
    private PlayerInventory playerInventory;

    private void Update() {
        if (!shouldUpdate) return;
        HandleChunks();
        HandleInput();
    }

    private void HandleChunks() {
        if (shouldFindChunks) {
            FindChunks();
            shouldFindChunks = false;
        }
        Vector2 currentChunkIndex = getCurrentChunkIndex();
        if (currentChunkIndex.y == chunkBottom) {
            //move down  
            chunkBottom--;
            //move chunks[0,0],[1,0],[2,0] down
            chunks[0, 0].transform.position -= Vector3.up * 3 * Utils.CHUNKHEIGHT;
            chunks[1, 0].transform.position -= Vector3.up * 3 * Utils.CHUNKHEIGHT;
            chunks[2, 0].transform.position -= Vector3.up * 3 * Utils.CHUNKHEIGHT;

            //reinitialize
            //build
            chunks[0, 0].GetComponent<Chunk>().Reinitialize((int) chunks[0, 0].transform.position.x, (int) chunks[0, 0].transform.position.y).Build();
            chunks[1, 0].GetComponent<Chunk>().Reinitialize((int) chunks[1, 0].transform.position.x, (int) chunks[1, 0].transform.position.y).Build();
            chunks[2, 0].GetComponent<Chunk>().Reinitialize((int) chunks[2, 0].transform.position.x, (int) chunks[2, 0].transform.position.y).Build();

            //rename
            chunks[0, 0].name = "Chunk@" + chunkLeft + "," + chunkBottom;
            chunks[1, 0].name = "Chunk@" + (chunkLeft + 1) + "," + chunkBottom;
            chunks[2, 0].name = "Chunk@" + (chunkLeft + 2) + "," + chunkBottom;

            //find chunks
            shouldFindChunks = true;
        }
        else if (currentChunkIndex.y == chunkBottom + 2) {
            //move up  
            chunkBottom++;

            //move chunks[0,2],[1,2],[2,2] up
            chunks[0, 2].transform.position += Vector3.up * 3 * Utils.CHUNKHEIGHT;
            chunks[1, 2].transform.position += Vector3.up * 3 * Utils.CHUNKHEIGHT;
            chunks[2, 2].transform.position += Vector3.up * 3 * Utils.CHUNKHEIGHT;

            //reinitialize
            //build
            chunks[0, 2].GetComponent<Chunk>().Reinitialize((int) chunks[0, 2].transform.position.x, (int) chunks[0, 2].transform.position.y).Build();
            chunks[1, 2].GetComponent<Chunk>().Reinitialize((int) chunks[1, 2].transform.position.x, (int) chunks[1, 2].transform.position.y).Build();
            chunks[2, 2].GetComponent<Chunk>().Reinitialize((int) chunks[2, 2].transform.position.x, (int) chunks[2, 2].transform.position.y).Build();

            //rename
            chunks[0, 2].name = "Chunk@" + (chunkLeft) + "," + (chunkBottom + 2);
            chunks[1, 2].name = "Chunk@" + (chunkLeft + 1) + "," + (chunkBottom + 2);
            chunks[2, 2].name = "Chunk@" + (chunkLeft + 2) + "," + (chunkBottom + 2);

            //find chunks
            shouldFindChunks = true;
        }
        else if (currentChunkIndex.x == chunkLeft) {
            //move left
            chunkLeft--;

            //move chunks left
            chunks[2, 0].transform.position += Vector3.left * 3 * Utils.CHUNKWIDTH;
            chunks[2, 1].transform.position += Vector3.left * 3 * Utils.CHUNKWIDTH;
            chunks[2, 2].transform.position += Vector3.left * 3 * Utils.CHUNKWIDTH;

            //reinitialize
            //build
            chunks[2, 0].GetComponent<Chunk>().Reinitialize((int) chunks[2, 0].transform.position.x, (int) chunks[2, 0].transform.position.y).Build();
            chunks[2, 1].GetComponent<Chunk>().Reinitialize((int) chunks[2, 1].transform.position.x, (int) chunks[2, 1].transform.position.y).Build();
            chunks[2, 2].GetComponent<Chunk>().Reinitialize((int) chunks[2, 2].transform.position.x, (int) chunks[2, 2].transform.position.y).Build();

            //rename
            chunks[2, 0].name = "Chunk@" + (chunkLeft) + "," + (chunkBottom + 2);
            chunks[2, 1].name = "Chunk@" + (chunkLeft) + "," + (chunkBottom + 1);
            chunks[2, 2].name = "Chunk@" + (chunkLeft) + "," + (chunkBottom);

            //find chunks
            shouldFindChunks = true;
        }
        else if (currentChunkIndex.x == chunkLeft + 2) {
            //move right
            chunkLeft++;
            //move chunks right
            chunks[0, 0].transform.position -= Vector3.left * 3 * Utils.CHUNKWIDTH;
            chunks[0, 1].transform.position -= Vector3.left * 3 * Utils.CHUNKWIDTH;
            chunks[0, 2].transform.position -= Vector3.left * 3 * Utils.CHUNKWIDTH;

            //reinitialize
            //build
            chunks[0, 0].GetComponent<Chunk>().Reinitialize((int) chunks[0, 0].transform.position.x, (int) chunks[0, 0].transform.position.y).Build();
            chunks[0, 1].GetComponent<Chunk>().Reinitialize((int) chunks[0, 1].transform.position.x, (int) chunks[0, 1].transform.position.y).Build();
            chunks[0, 2].GetComponent<Chunk>().Reinitialize((int) chunks[0, 2].transform.position.x, (int) chunks[0, 2].transform.position.y).Build();

            //rename
            chunks[0, 0].name = "Chunk@" + (chunkLeft + 2) + "," + (chunkBottom + 2);
            chunks[0, 1].name = "Chunk@" + (chunkLeft + 2) + "," + (chunkBottom + 1);
            chunks[0, 2].name = "Chunk@" + (chunkLeft + 2) + "," + (chunkBottom);

            //find chunks
            shouldFindChunks = true;
        }
    }

    private void FindChunks() {
        chunks[0, 0] = GameObject.Find("Chunk@" + (chunkLeft) + "," + (chunkBottom + 2));
        chunks[0, 1] = GameObject.Find("Chunk@" + (chunkLeft) + "," + (chunkBottom + 1));
        chunks[0, 2] = GameObject.Find("Chunk@" + (chunkLeft) + "," + (chunkBottom));
        chunks[1, 0] = GameObject.Find("Chunk@" + (chunkLeft + 1) + "," + (chunkBottom + 2));
        chunks[1, 1] = GameObject.Find("Chunk@" + (chunkLeft + 1) + "," + (chunkBottom + 1));
        chunks[1, 2] = GameObject.Find("Chunk@" + (chunkLeft + 1) + "," + (chunkBottom));
        chunks[2, 0] = GameObject.Find("Chunk@" + (chunkLeft + 2) + "," + (chunkBottom + 2));
        chunks[2, 1] = GameObject.Find("Chunk@" + (chunkLeft + 2) + "," + (chunkBottom + 1));
        chunks[2, 2] = GameObject.Find("Chunk@" + (chunkLeft + 2) + "," + (chunkBottom));
    }

    private void HandleInput() {
        int hbSelection = CheckHotbarSelection();
        if (hbSelection != -1)
            playerInventory.setSelected(hbSelection);

        HandleBlockBreaking();
        HandleBlockPlacing();
    }

    private void HandleBlockPlacing() {
        if (Input.GetButton("Fire2")) {
            Vector2Int mousePosition = GetMousePositionAsInt();
            Tile tileUnderMouse = tileMap.Planet.getBlocks()[mousePosition.x, mousePosition.y];
            
            //Stop if trying to place on another block or too far
            if (tileUnderMouse.id != "air" || !IsDistanceOk(mousePosition)  || !IsPlayerOk(mousePosition)) return;

            string selected = playerInventory.getSelected();
            
            //Stop if inventory is out of selected block
            if (!playerInventory.HasItem(selected)) return;
            playerInventory.RemoveItem(selected);
            tileMap.Planet.getBlocks()[mousePosition.x, mousePosition.y] = tileMap.Database.TileDatabase.TileDictionary[selected];
            
            RebuildChunk(mousePosition);
        }
    }
    
    private void HandleBlockBreaking() {
        if (Input.GetButtonDown("Fire1")) {
            Vector2Int mousePosition = GetMousePositionAsInt();
            if (!StartMining(mousePosition, tileMap.Planet.getBlocks()[mousePosition.x, mousePosition.y]))
                StopMining();
        }
        if (Input.GetButton("Fire1")) {
            Vector2Int mousePosition = GetMousePositionAsInt();
            Tile tileUnderMouse = tileMap.Planet.getBlocks()[mousePosition.x, mousePosition.y];
            if (isCurrentlyMining) {
                if (currentBlockPosition != mousePosition) {
                    if (!StartMining(mousePosition, tileUnderMouse))
                        StopMining();
                }
                else if (currentMineTime >= requiredMineTime) {
                    BreakBlock(mousePosition, tileUnderMouse);
                }
                else if (!tileUnderMouse.properties.undestructible) {
                    currentMineTime += Time.deltaTime;
                }
            }
            else {
                if(!StartMining(mousePosition, tileUnderMouse))
                    StopMining();
            }
        }
        if (Input.GetButtonUp("Fire1")) {
            StopMining();
        }
    }

    private bool StartMining(Vector2Int mousePosition, Tile tileUnderMouse) {
        if (tileUnderMouse.id != "air" && IsDistanceOk(mousePosition)) {
            isCurrentlyMining = true;
            currentBlockPosition = mousePosition;
            currentMineTime = 0.0d;
            requiredMineTime = tileUnderMouse.properties.hardness;
            return true;
        }
        return false;
    }

    private void StopMining() {
        isCurrentlyMining = false;
    }

    private bool IsDistanceOk(Vector2Int mousePosition) {
        Vector3 playerPosition = playerInventory.transform.position;
        return Utils.Distance(mousePosition, playerPosition) <= MaxBlockBreakingDistance;
    }

    private bool IsPlayerOk(Vector2Int mousePosition) {
        Vector2Int playerPosition = new Vector2Int((int) playerInventory.transform.position.x, (int) playerInventory.transform.position.y);
        return Mathf.Abs(playerPosition.x - mousePosition.x) > 1 || Mathf.Abs(playerPosition.y - mousePosition.y) > 2;
    }

    private void BreakBlock(Vector2Int mousePosition, Tile tileUnderMouse) {
        GameObject pickup = Instantiate(ItemPickupPrefab, new Vector3(mousePosition.x + 0.5f, mousePosition.y + 0.5f, 0), Quaternion.identity);
        pickup.name = tileUnderMouse.id;
        pickup.transform.Find("ItemPickupRender").GetComponent<SpriteRenderer>().sprite = tileMap.Database.ItemDatabase.Sprites[tileUnderMouse.itemID];

        Debug.Log("Instantiated pickup: " + pickup);

        tileMap.Planet.getBlocks()[mousePosition.x, mousePosition.y] = tileMap.Database.TileDatabase.TileDictionary["air"];

        RebuildChunk(mousePosition);
        StopMining();
    }

    private int CheckHotbarSelection() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) return 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) return 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) return 3;
        if (Input.GetKeyDown(KeyCode.Alpha4)) return 4;
        if (Input.GetKeyDown(KeyCode.Alpha5)) return 5;
        if (Input.GetKeyDown(KeyCode.Alpha6)) return 6;
        if (Input.GetKeyDown(KeyCode.Alpha7)) return 7;
        if (Input.GetKeyDown(KeyCode.Alpha8)) return 8;
        if (Input.GetKeyDown(KeyCode.Alpha9)) return 9;
        return -1;
    }

    public void Initialize(GameObject PlayerPrefab, TileMap tileMap) {
        this.tileMap = tileMap;
        GameObject player = Instantiate(PlayerPrefab, tileMap.Planet.getSpawnPoint() + Vector2.up * 4, Quaternion.identity);
        player.name = "Player";
        Debug.Log("WORLDMANAGER:INITIALIZE():PLAYERPOS:" + player.transform.position);

        playerInventory = player.GetComponent<PlayerInventory>();
        CameraFollow.target = player.transform;
        CameraFollow.transform.position = new Vector3(tileMap.Planet.getSpawnPoint().x, tileMap.Planet.getSpawnPoint().y, CameraFollow.transform.position.z);

        Vector2 centerChunk = getCurrentChunkIndex();
        chunkLeft = (int) centerChunk.x - 1;
        chunkBottom = (int) centerChunk.y - 1;
        chunks = new GameObject[3, 3];

        shouldFindChunks = true;
        shouldUpdate = true;
    }

    private void RebuildChunk(Vector2Int mousePosition) {
        Vector2 chunkIndex = tileMap.Planet.getChunkIndex(mousePosition.x, mousePosition.y);
        Chunk chunk = GameObject.Find("Chunk@" + chunkIndex.x + "," + chunkIndex.y).GetComponent<Chunk>();
        chunk.Build();
    }

    private Vector2Int GetMousePositionAsInt() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2Int((int) mousePosition.x, (int) mousePosition.y);
    }

    private Vector2 getCurrentChunkIndex() {
        return tileMap.Planet.getChunkIndex((int) CameraFollow.target.position.x, (int) CameraFollow.target.position.y);
    }
}