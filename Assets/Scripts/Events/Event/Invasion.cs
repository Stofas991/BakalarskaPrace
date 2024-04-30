using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[GameEvent]
public class Invasion : IGameEvent
{
    List<GameObject> unitPrefabList = new List<GameObject>();
    List<GameObject> unitList = new List<GameObject>();
    List<Tilemap> bannedTilemaps = new List<Tilemap>();
    int height;
    int width;
    int unitsToSpawn;
    int callAmmount = 0;
    float difficulty = 1;

    public void InitEvent()
    {
        MapGenerator generator = MapGenerator.GetInstance();
        height = generator.mapHeight;
        width = generator.mapWidth;
        var eventManager = EventManager.GetInstance();
        unitPrefabList = eventManager.GetEnemyPrefabList();
        bannedTilemaps = eventManager.GetBannedTilemaps();
        if (SelectedValues.isSet)
            difficulty = SelectedValues.difficulty;
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
            return false;
        }
        return true;
    }

    public PopupWindowInfo EndEvent()
    {
        PopupWindowInfo windowInfo;
        windowInfo.isDenieable = false;
        windowInfo.textBoxContent = "All enemies were eliminated! Congratulations.";
        windowInfo.deniedContent = string.Empty;
        windowInfo.acceptContent = "Continue";
        return windowInfo;
    }

    public PopupWindowInfo GetPopupWindowInfo()
    {
        unitsToSpawn = Convert.ToInt32((callAmmount+1) * 3 * difficulty);
        PopupWindowInfo windowInfo;
        windowInfo.isDenieable = false;
        windowInfo.textBoxContent = "A group of <color=red>" + unitsToSpawn + "</color> blood thirsty demons has grouped close to your settlement. Get ready for battle.";
        windowInfo.deniedContent = string.Empty;
        windowInfo.acceptContent = "Continue";
        return windowInfo;
    }

    private List<GameObject> SpawnUnit(int mapHeight, int mapWidth, int callAmmount)
    {
        List<GameObject> unitList = new List<GameObject>();

        int rectangleWidth = unitsToSpawn;

        var squareEdges = GetEdgeSquareWithRequiredSpace(mapHeight, mapWidth, unitsToSpawn, rectangleWidth, 3);

        var closestUnit = FindClosestPlayerUnit(squareEdges);

        for (int i = 0; i < unitsToSpawn; i++)
        {
            GameObject unitPrefab = GetRandomUnitPrefab();

            if (unitPrefab != null)
            {

                var spawnPosition = GetValidSpawnPosition(squareEdges);
                var unit = GameObject.Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
                var agent = unit.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.SetDestination(closestUnit.transform.position);
                }
                unitList.Add(unit);
            }
        }

        return unitList;
    }

    private Vector4 GetEdgeSquareWithRequiredSpace(int mapHeight, int mapWidth, int unitsToSpawn, float rectangleWidth, int offset)
    {
        while (true)
        {
            Vector2 edgePosition = Vector2.zero;
            Vector4 edgeSquare = new Vector4(); // left, bottom, top and right border of square

            bool isHorizontalEdge = Random.Range(0, 2) == 0;

            if (isHorizontalEdge)
            {
                edgePosition.x = Random.Range(offset, mapWidth - rectangleWidth - offset);
                edgePosition.y = Random.Range(0, 2) == 0 ? offset : mapHeight - rectangleWidth - offset;
            }
            else
            {
                edgePosition.x = Random.Range(0, 2) == 0 ? offset : mapWidth - rectangleWidth - offset;
                edgePosition.y = Random.Range(offset, mapHeight - rectangleWidth - offset);
            }

            // Nastavte okraje ètverce
            edgeSquare.x = edgePosition.x; // Levý okraj
            edgeSquare.y = edgePosition.x + rectangleWidth; // Pravý okraj
            edgeSquare.z = edgePosition.y; // Horní okraj
            edgeSquare.w = edgePosition.y + rectangleWidth; // Dolní okraj

            // Zkontrolujte, zda jsou v ètverci alespoò unitsToSpawn volná místa
            int freeSpaces = 0;
            for (float x = edgeSquare.x; x <= edgeSquare.y; x++)
            {
                for (float y = edgeSquare.z; y <= edgeSquare.w; y++)
                {
                    if (!IsBannedTilemap(new Vector3Int((int)x, (int)y, 0)))
                    {
                        freeSpaces++;
                    }
                }
            }

            // Pokud je dostatek volných míst pro spawnout jednotek, vrate okraje ètverce
            if (freeSpaces >= unitsToSpawn)
            {
                return edgeSquare;
            }
        }
    }

    public GameObject FindClosestPlayerUnit(Vector4 edgeSquare)
    {
        GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("selectable");

        if (playerUnits.Length == 0)
        {
            Debug.LogWarning("No player units found.");
            return null;
        }

        GameObject closestUnit = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject unit in playerUnits)
        {
            Vector3 unitPosition = unit.transform.position;

            // Calculate distance from center of square
            float distance = Vector2.Distance(new Vector2(unitPosition.x, unitPosition.y), new Vector2((edgeSquare.x + edgeSquare.y) / 2f, (edgeSquare.z + edgeSquare.w) / 2f));

            // closer than unit before
            if (distance < closestDistance)
            {
                closestUnit = unit;
                closestDistance = distance;
            }
        }

        return closestUnit;
    }

    private Vector2 GetValidSpawnPosition(Vector4 edgeSquare)
    {
        while (true)
        {
            Vector2 spawnPosition = GetRandomPositionInSquare(edgeSquare);

            // Zkontrolujte, zda je pozice volná pro spawnout jednotek
            if (!IsBannedTilemap(new Vector3Int((int)spawnPosition.x, (int)spawnPosition.y, 0)))
            {
                return spawnPosition; // Pokud je pozice volná, vrate ji
            }
            // Pokud není, opakujte proces vybírání náhodné pozice
        }
    }

    private Vector2 GetRandomPositionInSquare(Vector4 edgeSquare)
    {
        float x = Random.Range(edgeSquare.x, edgeSquare.y);
        float y = Random.Range(edgeSquare.z, edgeSquare.w);
        return new Vector2(x, y);
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
