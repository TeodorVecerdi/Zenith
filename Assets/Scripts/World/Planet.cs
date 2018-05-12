using System;
using UnityEngine;
using Random = System.Random;

public class Planet {
    private Tile[,] blocks;
    private Tile[,] blocksBG;
    public int[] GenHeight;

    public PlanetSettings PlanetSettings;

    private int caves;
    private double caveLengthBoost;
    private int stonePockets;

    private Vector2 SpawnPoint;

    public Zenith.Database Database;
    private Random random;

    public Planet(Zenith.Database database, PlanetSettings planetSettings, Random GameRNG) {
        Database = database;
        PlanetSettings = planetSettings;
        caves = Mathf.Max(planetSettings.worldSize.x, planetSettings.worldSize.y) / 16;
        caveLengthBoost = planetSettings.caveLengthBoost;
        stonePockets = Mathf.Max(planetSettings.worldSize.x, planetSettings.worldSize.y) / 32;
        blocks = new Tile[planetSettings.worldSize.x, planetSettings.worldSize.y];
        blocksBG = new Tile[planetSettings.worldSize.x, planetSettings.worldSize.y];
        GenHeight = new int[planetSettings.worldSize.x];
        random = GameRNG;
    }

    public void Generate(Action<string> setStatus) {
        setStatus("Building base terrain");
        BuildBaseTerrain();
        SpawnPoint = new Vector2(PlanetSettings.worldSize.x / 2 + Utils.CHUNKWIDTH / 2, GenHeight[PlanetSettings.worldSize.x / 2 + Utils.CHUNKWIDTH / 2]);
        setStatus("Building caves");
        BuildCaves(caves, caveLengthBoost);
        setStatus("Populating the underground");
        PopulateUnderground(stonePockets);
        setStatus("Populating the surface");
        PopulateSurface();
    }

    private void BuildBaseTerrain() {
        float noiseScale = 0.008f;
        for (int i = 0; i < PlanetSettings.worldSize.x; i++) {
            int genHeight = Mathf.FloorToInt(Utils.map(Noise.CalcPixel1D(i, noiseScale) * 1, 0, 256,
                PlanetSettings.worldSize.y / 2 - 128, PlanetSettings.worldSize.y / 2 + (float) (128 * PlanetSettings.heightBoost)));
            GenHeight[i] = genHeight;
            for (int j = 0; j < PlanetSettings.worldSize.y; j++) {
                if (j > genHeight) {
                    blocks[i, j] = Database.TileDatabase.TileDictionary["air"];
                    blocksBG[i, j] = Database.TileDatabase.TileDictionary["air"];
                }
                else if (j == genHeight) {
                    blocks[i, j] = Database.TileDatabase.TileDictionary[PlanetSettings.surfaceTile];
                    blocksBG[i, j] = Database.TileDatabase.TileDictionary[PlanetSettings.surfaceTile];
                }else if (j < genHeight && j >= genHeight-8) {
                    blocks[i, j] = Database.TileDatabase.TileDictionary[PlanetSettings.middleTile];
                    blocksBG[i, j] = Database.TileDatabase.TileDictionary[PlanetSettings.middleTile];
                }
                else if (j < genHeight - 8) {
                    blocks[i, j] = Database.TileDatabase.TileDictionary[PlanetSettings.undergroundTile];
                    blocksBG[i, j] = Database.TileDatabase.TileDictionary[PlanetSettings.undergroundTile];
                }
            }
        }
        for (int i = 0; i < PlanetSettings.worldSize.x; i++)
        for (int j = 0; j < Utils.CHUNKHEIGHT * 2; j++) {
            blocks[i, j] = Database.TileDatabase.TileDictionary["bedrock"];
            blocksBG[i, j] = Database.TileDatabase.TileDictionary["bedrock"];
        }
    }

    private void BuildCaves(int numberOfWorms, double lifeSpanBoost) {
        for (int i = 0; i < numberOfWorms; i++) {
            SpawnWorm("air", 0, 1, lifeSpanBoost);
        }
    }

    private void PopulateUnderground(int numberOfStonePatches) {
        //Create stone patches
        for (int i = 0; i < numberOfStonePatches; i++) {
            SpawnWorm("stone", 2, 2);
        }
    }

    private void PopulateSurface() {
        foreach (string biomeFeature in Database.BiomeDatabase.BiomeDictionary[PlanetSettings.biome].features) {
            if (Database.StructureDatabase.StructureDictionary.ContainsKey(biomeFeature)) {
                int bounds = PlanetSettings.worldSize.x / 8;
                for (int i = 0; i < bounds; i++) {
                    int posX;
                    Structure structure = Database.StructureDatabase.StructureDictionary[biomeFeature][Utils.RandomInt(random, 0, Database.StructureDatabase.StructureDictionary[biomeFeature].Length)];
                    do {
                        posX = Utils.RandomInt(random, 0, PlanetSettings.worldSize.x);
                    } while (!CanPlaceStructure(structure, posX));
                    PlaceStructure(structure, posX);
                }
            }
        }
    }

    private bool CanPlaceStructure(Structure structure, int posX) {
        int offsetY = GenHeight[posX] + 1;
        int offsetX = posX - (int) structure.size.x / 2;
        foreach (StructureTile structureTile in structure.tiles) {
            if (blocks[offsetX + (int) structureTile.pos.x, offsetY + (int) structureTile.pos.y].id != structureTile.tile &&
                blocks[offsetX + (int) structureTile.pos.x, offsetY + (int) structureTile.pos.y].id != "air")
                return false;
        }
        return true;
    }

    private void PlaceStructure(Structure structure, int posX) {
        int offsetY = GenHeight[posX] + 1;
        int offsetX = posX - (int) structure.size.x / 2;
        foreach (StructureTile structureTile in structure.tiles) {
            blocks[offsetX + (int) structureTile.pos.x, offsetY + (int) structureTile.pos.y] = Database.TileDatabase.TileDictionary[structureTile.tile];
        }
    }

    /*
     * Spawns a new worm
     */
    private void SpawnWorm(string tileID, int layer, int brushBoost, double lifespanBoost = 10f) {
        Vector2 wormDirection = new Vector2(
            Mathf.Floor(Utils.RandomFloat(random, -1f, 1f)),
            Mathf.Floor(Utils.RandomFloat(random, -1f, 1f))
        );
        float x = Mathf.Floor(Utils.RandomFloat(random, 0f, PlanetSettings.worldSize.x));
        Vector2 currentWormPosition = new Vector2(
            x,
            Mathf.Floor(Utils.RandomFloat(random, 0f, GenHeight[(int) x]))
        );
        int wormLifetimeMax = (int) Utils.RandomFloat(random, 7 * (float) lifespanBoost, 10 * (float) lifespanBoost);
        int currentWormSteps = 0;

        while (currentWormSteps < wormLifetimeMax) {
            // The amount of steps the worm will take this turn
            int moveAmount = Utils.RandomInt(random, 5, 11);

            // Until we have taken the appropriate number of steps, loop
            for (int currentStep = 0; currentStep < moveAmount; currentStep++) {
                // Validate worm position and punch blocks
                if (!IsValidPositionForWorm(currentWormPosition, layer, tileID, currentWormSteps, wormLifetimeMax,
                    brushBoost)) {
                    return;
                }
                // Move the worm
                currentWormPosition += wormDirection;
                // Increase worm steps
                currentWormSteps++;
            }

            // Decide if the worm will move a negative amount
            int isNegativeX = 1;
            int isNegativeY = 1;
            if (Utils.RandomInt(random, 0, 2) == 1)
                isNegativeX = -1;
            if (Utils.RandomInt(random, 0, 2) == 1)
                isNegativeY = -1;
            wormDirection = new Vector2(
                isNegativeX * Mathf.RoundToInt(Utils.map(Noise.Generate(Utils.RandomFloat(random, 0f, 1f), Utils.RandomFloat(random, 0f, 1f), 0), -1, 1, 0, 1)),
                isNegativeY * Mathf.RoundToInt(Utils.map(Noise.Generate(Utils.RandomFloat(random, 0f, 1f), Utils.RandomFloat(random, 0f, 1f), 0), -1, 1, 0, 1))
            );
        }
    }

    /*
     * Attempts to fill the area with the specified terrain and returns if it's out of the world bounds
     */
    private bool IsValidPositionForWorm(Vector2 position, int layer, string tileID, int currentStep, int maxSteps,
        int brushBoost) {
        int brushRadius = Mathf.FloorToInt(Utils.map(currentStep, 0, maxSteps, 3, 6)) * brushBoost;
        float dist = brushRadius * brushRadius;
        for (float y = -brushRadius; y < brushRadius; y++) {
            for (float x = -brushRadius; x < brushRadius; x++) {
                if (new Vector2(x, y).sqrMagnitude < dist) {
                    if (position.x + x >= PlanetSettings.worldSize.x - 1 || position.x + x <= 0)
                        return false;

                    if (position.y + y >= PlanetSettings.worldSize.y - 1 || position.y + y <= 0)
                        return false;
                    if (!blocks[(int) position.x + (int) x, (int) position.y + (int) y].properties.undestructible)
                        if (layer == 0) {
                            blocks[(int) position.x + (int) x, (int) position.y + (int) y] = Database.TileDatabase.TileDictionary[tileID];
                        }
                        else if (layer == 1) {
                            blocksBG[(int) position.x + (int) x, (int) position.y + (int) y] = Database.TileDatabase.TileDictionary[tileID];
                        }
                        else if (layer == 2) {
                            blocks[(int) position.x + (int) x, (int) position.y + (int) y] = Database.TileDatabase.TileDictionary[tileID];
                            blocksBG[(int) position.x + (int) x, (int) position.y + (int) y] = Database.TileDatabase.TileDictionary[tileID];
                        }
                }
            }
        }
        return true;
    }

    public Vector2 getSpawnPoint() {
        return SpawnPoint;
    }

    public Vector2 getChunkIndex(int x, int y) {
        return new Vector2(x / Utils.CHUNKWIDTH, y / Utils.CHUNKHEIGHT);
    }

    public Tile[,] getBlocks() {
        return blocks;
    }

    public Tile[,] getBlocksBG() {
        return blocksBG;
    }
}