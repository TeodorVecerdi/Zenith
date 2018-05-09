using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameManager : MonoBehaviour {

	public TileMap TileMap;

	public string GameName;
	public int GameSeed;

	public Database Database;
	public Random random;

	public void Initialize(Database database) {
		Noise.Seed = GameSeed;
		Database = database;
		random = new Random(GameSeed);
	}

	public void Create() {
		TileMap.Create(random, Database, GeneratePlanet());
	}

	public PlanetSettings GeneratePlanet() {
		WorldSize worldSize = Database.WorldDatabase.World.sizes[Utils.RandomInt(random, 0, Database.WorldDatabase.World.sizes.Length)];
		
		int oreSamples = Database.WorldDatabase.World.oreSamples[Utils.RandomInt(random, 0, Database.WorldDatabase.World.oreSamples.Length)];
		
		List<string> ores = new List<string>();
		for (int i = 0; i < oreSamples; i++) {
			ores.Add(Database.WorldDatabase.World.ores[Utils.RandomInt(random, 0, Database.WorldDatabase.World.ores.Length)]);
		}

		float oreLengthBoost = Database.WorldDatabase.World.oreLengthBoost[Utils.RandomInt(random, 0, Database.WorldDatabase.World.oreLengthBoost.Length)];
		
		float caveLengthBoost = Database.WorldDatabase.World.caveLengthBoost[Utils.RandomInt(random, 0, Database.WorldDatabase.World.caveLengthBoost.Length)];

		string biome = Database.WorldDatabase.World.biomes[Utils.RandomInt(random, 0, Database.WorldDatabase.World.biomes.Length)];
		
		string surfaceTile = Database.BiomeDatabase.BiomeDictionary[biome].surfaceTile;
		
		string undergroundTile = Database.BiomeDatabase.BiomeDictionary[biome].undergroundTile;
		
		string[] features = Database.BiomeDatabase.BiomeDictionary[biome].features;
		
		return new PlanetSettings {
			worldSize = worldSize,
			oreSamples = oreSamples,
			ores = ores.ToArray(),
			oreLengthBoost = oreLengthBoost,
			caveLengthBoost = caveLengthBoost,
			biome = biome,
			surfaceTile = surfaceTile,
			undergroundTile = undergroundTile,
			features = features
		};
	}
}
