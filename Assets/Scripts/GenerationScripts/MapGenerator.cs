using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;



public class MapGenerator : Singleton<MapGenerator>
{
    public enum DrawMode { NoiseMap, ColourMap, TileMap };
    public DrawMode drawMode;

    public int mapWidth = 100;
    public int treeLimit = 50;
    public int mapHeight = 100;
    public float noiseScale;
    public float treeNoisScale;


    public int octaves;
    [Range(0f, 1f)]
    public float persistance;
    public float lacunarity;

    public int seed = 100000000;
    public Vector2 offset;

    public bool autoUpdate;
    public GenerationType generationType;

    public TerrainType[] regions;

    public GameObject treePrefab;
    private int treeCount;
    private GameObject wfcParent;
    [SerializeField] WFCTile wfcFiller;
    [SerializeField] NavMeshSurface2d nmSurface;
    private WFCGenerator wfcGenerator;

    private bool end;

    [SerializeField] Tilemap[] tilemaps;

    public void Start()
    {
        if (SelectedValues.isSet)
        {
            wfcGenerator = GetComponent<WFCGenerator>();

            seed = SelectedValues.seed;
            mapWidth = SelectedValues.mapSize;
            mapHeight = SelectedValues.mapSize;
            wfcGenerator.mountainLimit = 1200;
            wfcGenerator.mountainLimited = false;
            wfcGenerator.waterLimit = 1000;
            wfcGenerator.waterLimited = false;

            if (mapWidth == 200)
            {
                treeLimit = 1000;
                wfcGenerator.mountainLimit = wfcGenerator.mountainLimit * 2;
                wfcGenerator.waterLimit = wfcGenerator.waterLimit * 2;
            }
            else if (mapHeight == 50) 
            {
                wfcGenerator.mountainLimit = wfcGenerator.mountainLimit / 6;
                wfcGenerator.waterLimit = wfcGenerator.waterLimit / 6;
            }

            if (SelectedValues.isPerlin)
                generationType = GenerationType.Perlin;
            else
                generationType = GenerationType.WFC;

            GenerateMap();
        }
        CameraController.GetInstance().setCameraBorders();
        nmSurface.BuildNavMeshAsync();
    }

    public void GenerateMap()
    {
        float[,] noiseMap = noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] treeNoiseMap = noise.GenerateNoiseMap(mapWidth, mapHeight, seed, treeNoisScale, octaves, persistance, lacunarity, new Vector2(2.87f, 1.8f));
        float[,] vegetationMap = noise.GenerateNoiseMap(mapWidth, mapHeight, seed, 2, octaves, persistance, lacunarity, new Vector2(2.87f, 1.8f));

        Color[] colourMap = new Color[mapWidth * mapHeight];

        //removing old structures on map
        ClearMap();

        List<List<WFCCell>> tileRows = new List<List<WFCCell>>();
        for (int y = 0; y < mapHeight; y++)
        {
            List<WFCCell> tiles = new List<WFCCell>();
            for (int x = 0; x < mapWidth; x++)
            {
                if (generationType == GenerationType.Perlin)
                {
                    //Placing tiles according to regions
                    PlaceRegionTiles(noiseMap, treeNoiseMap, vegetationMap, colourMap, x, y);
                }
                else if (generationType == GenerationType.WFC)
                {
                    WFCCell cell = wfcGenerator.PlaceTiles(x, y, wfcParent.transform);
                    cell.entropy = cell.GetPossibilities().Count;
                    tiles.Add(cell);
                }
            }
            tileRows.Add(tiles);
        }

        if (generationType == GenerationType.WFC)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    var tile = tileRows[y][x];
                    if (y > 0)
                        tile.AddNeighbour(Direction.Down, tileRows[y - 1][x]);
                    if (x < mapWidth - 1)
                        tile.AddNeighbour(Direction.Right, tileRows[y][x + 1]);
                    if (y < mapHeight - 1)
                        tile.AddNeighbour(Direction.Up, tileRows[y + 1][x]);
                    if (x > 0)
                        tile.AddNeighbour(Direction.Left, tileRows[y][x - 1]);
                }
            }

            while (!wfcGenerator.finished)
            {
                if (wfcGenerator.mountainLimit == 0 && !wfcGenerator.mountainLimited)
                {
                    wfcGenerator.mountainLimited = true;
                    foreach (Transform child in wfcParent.transform)
                    {
                        WFCCell currentCell = child.GetComponent<WFCCell>();
                        if (!currentCell.collapsed)
                        {
                            currentCell.RemoveMountainPossibilities();
                        }
                    }
                }
                if (wfcGenerator.waterLimit == 0 && !wfcGenerator.waterLimited)
                {
                    wfcGenerator.waterLimited = true;
                    foreach (Transform child in wfcParent.transform)
                    {
                        WFCCell currentCell = child.GetComponent<WFCCell>();
                        if (!currentCell.collapsed)
                        {
                            currentCell.RemoveWaterPossibilities();
                        }
                    }
                }
                //getting list of lowest entropies in grid
                var wFCCells = wfcGenerator.GetLowestEntropy(wfcParent.transform);

                //if no cells were returned, algorithm finished
                if (wFCCells.Count == 0)
                    break;

                //getting random one from this list
                System.Random random = new System.Random();
                int randomIndex = random.Next(0, wFCCells.Count - 1);
                WFCCell cell = wFCCells[randomIndex];

                //calling function on this cell
                wfcGenerator.WaveFunction(cell);
            }
            foreach (Transform child in wfcParent.transform)
            {
                WFCCell currentCell = child.GetComponent<WFCCell>();
                if (currentCell.entropy == 0 && !currentCell.collapsed)
                {
                    Instantiate(wfcFiller, child.transform);
                    wfcGenerator.grassTilemap.SetTile(new Vector3Int((int)child.transform.position.x, (int)child.transform.position.y, 0), wfcGenerator.grassBase);
                    currentCell.entropy = 0;
                    currentCell.collapsed = true;
                }
            }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    float treeCurrentHeight = treeNoiseMap[x, y];
                    float vegetationCurrentHeight = vegetationMap[x, y];

                    PlaceVegetation(currentHeight, x, y, treeCurrentHeight, vegetationCurrentHeight);
                }
            }
            DestroyImmediate(wfcParent);
        }
    }

    private void ClearMap()
    {
        treeCount = 0;
        end = false;
        foreach (Tilemap tilemap in tilemaps)
        {
            tilemap.ClearAllTiles();
        }
        if (wfcParent == null)
        {
            wfcParent = new GameObject();
        }
        else
        {
            DestroyImmediate(wfcParent);
            wfcParent = new GameObject();
        }

    }

    private void PlaceRegionTiles(float[,] noiseMap, float[,] treeNoiseMap, float[,] vegetationMap, Color[] colourMap, int x, int y)
    {
        float currentHeight = noiseMap[x, y];
        float treeCurrentHeight = treeNoiseMap[x, y];
        float vegetationCurrentHeight = vegetationMap[x, y];

        for (int i = 0; i < regions.Length; i++)
        {
            if (currentHeight <= regions[i].height)
            {
                //Generating tilemap with tiles
                regions[i].tileMap.SetTile(new Vector3Int(x, y, 0), regions[i].tile);
                if (regions[i].name == "mountain")
                {
                    regions[2].tileMap.SetTile(new Vector3Int(x, y, 0), regions[2].tile);
                }

                if (x == 0)
                {
                    regions[i].tileMap.SetTile(new Vector3Int(x - 1, y, 0), regions[i].tile);
                    regions[i].tileMap.SetColor(new Vector3Int(x - 1, y, 0), Color.clear);
                }
                if (y == 0)
                {
                    regions[i].tileMap.SetTile(new Vector3Int(x, y - 1, 0), regions[i].tile);
                    regions[i].tileMap.SetColor(new Vector3Int(x, y - 1, 0), Color.clear);
                }
                if (x >= mapWidth - 1)
                {
                    regions[i].tileMap.SetTile(new Vector3Int(x + 1, y, 0), regions[i].tile);
                    regions[i].tileMap.SetColor(new Vector3Int(x + 1, y, 0), Color.clear);
                }
                if (y >= mapHeight - 1)
                {
                    regions[i].tileMap.SetTile(new Vector3Int(x, y + 1, 0), regions[i].tile);
                    regions[i].tileMap.SetColor(new Vector3Int(x, y + 1, 0), Color.clear);
                }

                if (drawMode == DrawMode.ColourMap)
                {
                    colourMap[y * mapWidth + x] = regions[i].colour;
                }
                break;
            }
        }

        //adding trees on next layer and objects representing them
        PlaceVegetation(currentHeight, x, y, vegetationCurrentHeight, treeCurrentHeight);
    }

    public void PlaceVegetation(float currentHeight, int x, int y, float vegetationCurrentHeight, float treeCurrentHeight)
    {
        //adding trees on next layer and objects representing them
        if (!end && currentHeight > regions[0].height && currentHeight <= regions[1].height && treeCurrentHeight > regions[3].height && vegetationCurrentHeight <= regions[4].height)
        {
            if (generationType == GenerationType.WFC)
            {
                var wfcGenerator = WFCGenerator.GetInstance();
                if (!wfcGenerator.mountainTilemap.HasTile(new Vector3Int(x, y, 0)) && !wfcGenerator.waterTilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    regions[3].tileMap.SetTile(new Vector3Int(x, y, 0), regions[3].tile);
                    treeCount++;
                    if (treeCount > treeLimit)
                    {
                        end = true;
                    }
                }
            }
            else
            {
                regions[3].tileMap.SetTile(new Vector3Int(x, y, 0), regions[3].tile);
                treeCount++;
                if (treeCount > treeLimit)
                {
                    end = true;
                }
            }
        }
        else if (currentHeight > regions[0].height && currentHeight <= regions[1].height && vegetationCurrentHeight >= regions[4].height)
        {
            regions[5].tileMap.SetTile(new Vector3Int(x, y, 0), regions[5].tile);
        }

        //placing nothing everywhere where the trees are not present
        //used for erasing last existent tree tiles
        else
        {
            regions[4].tileMap.SetTile(new Vector3Int(x, y, 0), regions[4].tile);
        }
    }

    //After every change VALIDATES if conditions are ok
    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
    public TileBase tile;
    public Tilemap tileMap;
    public int ID;
}

public enum GenerationType
{
    Perlin,
    WFC
}
