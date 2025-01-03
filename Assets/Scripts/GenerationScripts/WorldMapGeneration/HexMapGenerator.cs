using UnityEngine;
using UnityEngine.Tilemaps;

public class HexMapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile oceanTile;
    public Tile forestTile;
    public Tile mountainTile;
    public Tile riverTile;
    public Tile desertTile;

    public int mapWidth = 50;
    public int mapHeight = 50;
    public float scale = 0.1f;  // Míra detailù šumu
    public int riverCount = 5;  // Poèet øek

    // Generování výškové mapy
    float[,] GenerateHeightMap(int width, int height)
    {
        float[,] map = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = x * scale;
                float yCoord = y * scale;
                map[x, y] = Mathf.PerlinNoise(xCoord, yCoord); // Vytváøení výšky na základì šumu
            }
        }
        return map;
    }

    // Pøiøazení biomù na základì výšky
    void AssignBiomesBasedOnHeight(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float heightValue = heightMap[x, y];
                if (heightValue < 0.3f)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), oceanTile);  // Oceán
                }
                else if (heightValue < 0.5f)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), forestTile);  // Les
                }
                else if (heightValue < 0.8f)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), mountainTile);  // Hory
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), desertTile);  // Pustina
                }
            }
        }
    }

    // Generování øek
    void GenerateRivers(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        for (int i = 0; i < riverCount; i++)  // Generování nìkolika øek
        {
            int startX = Random.Range(0, width);  // Náhodný start
            int startY = Random.Range(0, height);

            int x = startX;
            int y = startY;

            while (x >= 0 && x < width && y >= 0 && y < height)
            {
                // Najít nejnižší sousední bod (sešup vody)
                float lowestNeighbor = heightMap[x, y];
                Vector2Int direction = Vector2Int.zero;

                // Zkontrolujeme všechny sousední pixely (nahoøe, dolù, vlevo, vpravo)
                if (x + 1 < width && heightMap[x + 1, y] < lowestNeighbor)
                {
                    lowestNeighbor = heightMap[x + 1, y];
                    direction = Vector2Int.right;
                }
                if (x - 1 >= 0 && heightMap[x - 1, y] < lowestNeighbor)
                {
                    lowestNeighbor = heightMap[x - 1, y];
                    direction = Vector2Int.left;
                }
                if (y + 1 < height && heightMap[x, y + 1] < lowestNeighbor)
                {
                    lowestNeighbor = heightMap[x, y + 1];
                    direction = Vector2Int.up;
                }
                if (y - 1 >= 0 && heightMap[x, y - 1] < lowestNeighbor)
                {
                    lowestNeighbor = heightMap[x, y - 1];
                    direction = Vector2Int.down;
                }

                // Pokud jsme našli cestu, pohybujeme se tam
                if (direction != Vector2Int.zero)
                {
                    x += direction.x;
                    y += direction.y;
                    tilemap.SetTile(new Vector3Int(x, y, 0), riverTile);
                }
                else
                {
                    // Pokud neexistuje žádná nižší sousední buòka, zastavíme
                    break;
                }
            }
        }
    }


    void Start()
    {
        // Vytvoøení výškové mapy
        float[,] heightMap = GenerateHeightMap(mapWidth, mapHeight);

        // Pøiøazení biomù na základì výšky
        AssignBiomesBasedOnHeight(heightMap);

        // Generování øek
        GenerateRivers(heightMap);
    }



    public class HexCell
    {
        public int X;
        public int Y;
        public BiomeType Biome;
        public bool HasRuins;
        public bool IsOccupied;
    }

    public enum BiomeType
    {
        Water,
        Grass,
        Desert,
        Mountain
    }
}

