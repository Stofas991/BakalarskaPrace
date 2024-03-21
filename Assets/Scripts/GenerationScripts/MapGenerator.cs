using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap, TileMap};
    public DrawMode drawMode;

    public int mapWidth;
    public int treeLimit = 50;
    public int mapHeight;
    public float noiseScale;


    public int octaves;
    [Range(0f, 1f)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public GameObject treeParent;

    public bool autoUpdate;

    public TerrainType[] regions;

    public GameObject treePrefab;
    private int treeCount;

    private bool end;

    List<GameObject> treePrefabList = new List<GameObject>();

    [SerializeField] Tilemap[] tilemaps;

    public void GenerateMap()
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();

        //removing trees from old map
        if (treePrefabList.Count > 0)
        {
            foreach (GameObject tree in treePrefabList)
            {
                DestroyImmediate(tree);
            }
            treePrefabList.Clear();
        }
        Debug.Log(treePrefabList.Count);

        if (drawMode == DrawMode.NoiseMap)
        {
            float[,] noisemapp = noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
            display.DrawTexture(TextureGenerator.DrawNoiseMap(noisemapp));
        }

        foreach (Tilemap tilemap in tilemaps)
        {
            tilemap.ClearAllTiles();
        }

        float[,] noiseMap = noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] treeNoiseMap = noise.GenerateNoiseMap(mapWidth, mapHeight, seed, 1, octaves, persistance, lacunarity, new Vector2(2.87f, 1.8f));
        Color[] colourMap = new Color[mapWidth * mapHeight];
        treeCount = 0;
        end = false;

        for (int y = 0; y< mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                float treeCurrentHeight = treeNoiseMap[x, y];
                
                for(int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        //Generating tilemap with tiles
                        regions[i].tileMap.SetTile(new Vector3Int(x, y, 0), regions[i].tile);
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
                //adding trees on next layer and objects representing them
                if (!end && currentHeight > regions[1].height && currentHeight <= regions[2].height && treeCurrentHeight > regions[4].height)
                {

                    regions[4].tileMap.SetTile(new Vector3Int(x, y, 0), regions[4].tile);
                    GameObject tree = Instantiate(treePrefab, new Vector3(x + 0.5f, y + 0.5f, 0f), new Quaternion());
                    tree.transform.parent = treeParent.transform;
                    treePrefabList.Add(tree);
                    treeCount++;

                    if (treeCount > treeLimit)
                    {
                        end = true;
                    }
                }
                //placing nothing everywhere where the trees are not present
                //used for erasing trees
                else
                {
                    regions[5].tileMap.SetTile(new Vector3Int(x,y,0), regions[5].tile);
                } 
             
            }
        }
        Debug.Log(treePrefabList.Count);

        if (drawMode == DrawMode.NoiseMap)
        {
            //display.DrawTexture(TextureGenerator.TextureFromHeightMap(treeNoiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colourMap, mapWidth, mapHeight));
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
