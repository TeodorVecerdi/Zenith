using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {
    private Planet world;
    private MeshFilter meshFilterFG;
    private MeshFilter meshFilterBG;
    
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    
    
    public List<Rectangle> rectangles = new List<Rectangle>();

    private Stack<BoxCollider2D> activeColliders = new Stack<BoxCollider2D>();
    private Stack<BoxCollider2D> pooledColliders = new Stack<BoxCollider2D>();

    private const float UV_TEXTURE_SIZE = 1f / 16f;
    private int squareCount;

    private Vector2 chunkIndex;
    private int xOffset, yOffset;
    
    public void Initialize(Planet world, int x, int y) {
        this.world = world;
        chunkIndex = world.getChunkIndex(x, y);
        xOffset = (int) (chunkIndex.x * Utils.CHUNKWIDTH);
        yOffset = (int) (chunkIndex.y * Utils.CHUNKHEIGHT);
        meshFilterFG = transform.Find("FG").GetComponent<MeshFilter>();
        meshFilterBG = transform.Find("BG").GetComponent<MeshFilter>();
    }

    public Chunk Reinitialize(int x, int y) {
        chunkIndex = world.getChunkIndex(x, y);
        xOffset = (int) (chunkIndex.x * Utils.CHUNKWIDTH);
        yOffset = (int) (chunkIndex.y * Utils.CHUNKHEIGHT);
        return this;
    }
    
    public void Build() {
        BuildFG();
        BuildBG();
    }

    private void BuildFG() {
        for(int i = 0; i < Utils.CHUNKHEIGHT; i++)
        for (int j = 0; j < Utils.CHUNKWIDTH; j++) {
            GenSquare(j, i, getBlockAt(j, i));
        }
        UpdateMesh(meshFilterFG);
        createColliders();
    }

    private void BuildBG() {
        for(int i = 0; i < Utils.CHUNKHEIGHT; i++)
        for (int j = 0; j < Utils.CHUNKWIDTH; j++) {
            GenSquare(j, i, getBlockAt(j, i, 1));
        }
        UpdateMesh(meshFilterBG);
    }

    private void UpdateMesh(MeshFilter meshFilter) {
        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = vertices.ToArray();
        meshFilter.mesh.triangles = triangles.ToArray();
        meshFilter.mesh.uv = uvs.ToArray();
        meshFilter.mesh.RecalculateNormals();

        squareCount = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }
    
    private void GenSquare(int x, int y, Tile tile) {
        if(tile.id == "air") return;
        y++;
        vertices.Add(new Vector3(x, y, 0));
        vertices.Add(new Vector3(x + 1, y, 0));
        vertices.Add(new Vector3(x + 1, y - 1, 0));
        vertices.Add(new Vector3(x, y - 1, 0));

        triangles.Add(squareCount * 4);
        triangles.Add(squareCount * 4 + 1);
        triangles.Add(squareCount * 4 + 3);
        triangles.Add(squareCount * 4 + 1);
        triangles.Add(squareCount * 4 + 2);
        triangles.Add(squareCount * 4 + 3);
        Vector2 texture = tile.texture;
        uvs.Add(new Vector2(UV_TEXTURE_SIZE * texture.x, UV_TEXTURE_SIZE * texture.y + UV_TEXTURE_SIZE));
        uvs.Add(new Vector2(UV_TEXTURE_SIZE * texture.x + UV_TEXTURE_SIZE,
            UV_TEXTURE_SIZE * texture.y + UV_TEXTURE_SIZE));
        uvs.Add(new Vector2(UV_TEXTURE_SIZE * texture.x + UV_TEXTURE_SIZE, UV_TEXTURE_SIZE * texture.y));
        uvs.Add(new Vector2(UV_TEXTURE_SIZE * texture.x, UV_TEXTURE_SIZE * texture.y));

        squareCount++;
    }
    
    private void createColliders() {
        while (activeColliders.Count != 0) {
            BoxCollider2D collider = activeColliders.Pop();
            collider.enabled = false;
            pooledColliders.Push(collider);
        }

        rectangles.Clear();
        Tile[,] blocks = new Tile[Utils.CHUNKWIDTH,Utils.CHUNKHEIGHT];
        for(int i = 0; i < Utils.CHUNKWIDTH; i++)
            for(int j = 0; j < Utils.CHUNKHEIGHT; j++)
                blocks[i,j] = world.getBlocks()[i+xOffset,j+yOffset];

        for (int i = 0; i < Utils.CHUNKWIDTH; i++) {
            for (int j = 0; j < Utils.CHUNKHEIGHT; j++) {
                if (!blocks[i, j].properties.transparent) {
                    Rectangle current = new Rectangle(new Vector2(i, j), new Vector2(i + 1, j + 1));
                    ExtendRectangle(ref current, blocks);
                    blocks[i, j] = world.Database.TileDatabase.TileDictionary["air"];
                    rectangles.Add(current);
                }
            }
        }

        foreach (var rectangle in rectangles) {
            BoxCollider2D current;
            if (pooledColliders.Count != 0) {
                current = pooledColliders.Pop();
                current.enabled = true;
            }
            else {
                current = gameObject.AddComponent<BoxCollider2D>();
            }
            int sizeX = (int)Mathf.Abs(rectangle.max.x - rectangle.min.x);
            int sizeY = (int)Mathf.Abs(rectangle.max.y - rectangle.min.y);
            current.size = new Vector2(sizeX, sizeY);
            current.offset = new Vector2(rectangle.min.x + current.size.x / 2, rectangle.min.y + current.size.y / 2);
            activeColliders.Push(current);
        }
    }

    private void ExtendRectangle(ref Rectangle current, Tile[,] blocks) {
        ExtendRectangle_H(ref current, blocks);
        ExtendRectangle_V(ref current, blocks);
    }

    private void ExtendRectangle_H(ref Rectangle current, Tile[,] blocks) {
        for (int i = (int) current.min.x; i < Utils.CHUNKWIDTH; i++) {
            if (!blocks[i, (int) current.min.y].properties.transparent) {
                current.max.x = i + 1;
                blocks[i, (int) current.min.y] = world.Database.TileDatabase.TileDictionary["air"];
            }
            else {
                break;
            }
        }
    }

    private void ExtendRectangle_V(ref Rectangle current, Tile[,] blocks) {
        for (int i = (int) current.min.y + 1; i < Utils.CHUNKHEIGHT; i++) {
            if (ExtendRectangle_VerifyExtension(i, current, blocks)) {
                ExtendRectangle_SetExtension(i, current, blocks);
                current.max.y = i + 1;
            }
            else break;
        }
    }

    private bool ExtendRectangle_VerifyExtension(int i, Rectangle current, Tile[,] blocks) {
        for (int x = (int) current.min.x; x < (int) current.max.x; x++)
            if (blocks[x, i].properties.transparent)
                return false;
        return true;
    }

    private void ExtendRectangle_SetExtension(int i, Rectangle current, Tile[,] blocks) {
        for (int x = (int) current.min.x; x < (int) current.max.x; x++)
            blocks[x, i] = world.Database.TileDatabase.TileDictionary["air"];
    }

    private Tile getBlockAt(int x, int y, int layer = 0) {
        return (layer==0?world.getBlocks():world.getBlocksBG())[x + xOffset, y + yOffset];
    }   
}

public struct Rectangle {
    public Vector2 min, max;

    public Rectangle(Vector2 min, Vector2 max) {
        this.min = min;
        this.max = max;
    }
}