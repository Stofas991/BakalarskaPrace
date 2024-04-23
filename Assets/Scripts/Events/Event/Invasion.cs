using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;

[GameEvent]
public class Invasion : IGameEvent
{
    List<GameObject> unitPrefabList = new List<GameObject>();
    List<GameObject> unitList = new List<GameObject>();
    List<Tilemap> bannedTilemaps = new List<Tilemap>();
    int height;
    int width;
    int callAmmount = 0;

    public void InitEvent()
    {
        MapGenerator generator = MapGenerator.GetInstance();
        height = generator.mapHeight;
        width = generator.mapWidth;
        var eventManager = EventManager.GetInstance();
        unitPrefabList.AddRange(eventManager.GetEnemyPrefabList());
        bannedTilemaps.AddRange(eventManager.GetBannedTilemaps());
    }

    public void StartEvent()
    {
        callAmmount++;
        unitList.AddRange(SpawnUnit(height, width, callAmmount));
        Debug.Log("Spawned: " +  unitList.Count + " units");
    }

    public bool UpdateEvent()
    {
        for (int i = unitList.Count-1; i >= 0; i--)
        {
            if (unitList[i] == null)
            {
                unitList.Remove(unitList[i]);
            }
        }
        if (unitList.Count == 0)
        {
            EndEvent();
            return false;
        }
        return true;
    }

    public void EndEvent()
    {
        Debug.Log("All units were eliminated, congratulations");
    }

    private List<GameObject> SpawnUnit(int mapHeight, int mapWidth, int callAmmount)
    {
        List<GameObject> unitList = new List<GameObject>();
        bool isHorizontalEdge = Random.Range(0, 2) == 0;

        int unitsToSpawn = callAmmount * 3;

        // chosing side to spawn unit
        for (int i = 0; i < unitsToSpawn; i++)
        {
            var spawnPosition = GetRandomSpawnablePosition(mapHeight/2, mapWidth/2, isHorizontalEdge);
            GameObject unitPrefab = GetRandomUnitPrefab();

            if (unitPrefab != null)
            {
                var unit = GameObject.Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
                unitList.Add(unit);
            }
        }

        // spawning unit on position
        return unitList;
    }

    private Vector2 GetRandomSpawnablePosition(int mapHeight, int mapWidth, bool isHorizontalEdge)
    {
        Vector2 edgePosition = Vector2.zero;
        bool isBannedTile = true;

        while (isBannedTile)
        {
            if (isHorizontalEdge)
            {
                // Bod na horizontální hranì mapy
                edgePosition.x = Random.Range(1, mapWidth-1); // Náhodná x souøadnice
                edgePosition.y = Random.Range(0, 2) == 0 ? 1 : mapHeight-1; // Náhodná y souøadnice na nulové nebo maximální výšce mapy
            }
            else
            {
                // Bod na vertikální hranì mapy
                edgePosition.x = Random.Range(0, 2) == 0 ? 1 : mapWidth-1; // Náhodná x souøadnice na nulové nebo maximální šíøce mapy
                edgePosition.y = Random.Range(1, mapHeight-1); // Náhodná y souøadnice
            }
            
            isBannedTile = IsBannedTilemap(new Vector3Int((int)edgePosition.x, (int)edgePosition.y, 0));
        }
        return edgePosition;
    }

    private GameObject GetRandomUnitPrefab()
    {
        if (unitPrefabList.Count > 0)
        {
            int randomIndex = Random.Range(0, unitPrefabList.Count);
            return unitPrefabList[randomIndex];
        }
        else
        {
            Debug.LogWarning("No enemy unit prefabs available.");
            return null;
        }
    }

    private bool IsBannedTilemap(Vector3Int position)
    {
        foreach (Tilemap tilemap in bannedTilemaps)
        {
            if (tilemap.GetTile(position) != null)
                return true;
        }
        return false;
    }
}
