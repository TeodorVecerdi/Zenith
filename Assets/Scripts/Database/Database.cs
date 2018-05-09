using System;

namespace Zenith {
    public class Database {
        public ItemDatabase ItemDatabase;
        public TileDatabase TileDatabase;
        public WorldDatabase WorldDatabase;
        public BiomeDatabase BiomeDatabase;

        public Database() {
            ItemDatabase = new ItemDatabase();
            TileDatabase = new TileDatabase();
            WorldDatabase = new WorldDatabase();
            BiomeDatabase = new BiomeDatabase();
        }

        public Database Load(Action<string> SetStatus) {
//        Debug.Log("Started database loading");
            SetStatus("Loading Item Database");
            ItemDatabase.Load();
            SetStatus("Loading Tile Database");
            TileDatabase.Load();
            SetStatus("Loading World Settings Database");
            WorldDatabase.Load();
            SetStatus("Loading Biome Database");
            BiomeDatabase.Load();
//        Debug.Log("Finished database loading");
            return this;
        }

        public string ItemIDToTileID(string itemID) {
            return TileDatabase.GetTileFromItem(itemID);
        }

        public string TileIDToItemID(string tileID) {
            return TileDatabase.TileDictionary[tileID].itemID;
        }
    }
}