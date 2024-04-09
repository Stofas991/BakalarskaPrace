using Sentry;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class WFCCell : MonoBehaviour
{
    [SerializeField] List<WFCTile> possibilities;
    public int entropy;
    public bool collapsed = false;
    Dictionary<Direction, WFCCell> neighbours = new Dictionary<Direction, WFCCell>();

    public void AddNeighbour(Direction direction, WFCCell cell)
    {
        neighbours[direction] = cell;
    }

    public WFCCell GetNeighbour(Direction direction)
    {
        return neighbours[direction];
    }

    public List<WFCTile> GetPossibilities()
    {
        return possibilities;
    }

    public Direction[] GetDirections()
    {
        return neighbours.Keys.ToArray();
    }

    public WFCTile Collapse()
    {
        WFCTile colapsedTile = GetRandomValue();
        possibilities.Clear();
        possibilities.Add(colapsedTile);
        Instantiate(colapsedTile, transform);
        entropy = 0;
        collapsed = true;
        return colapsedTile;
    }

    public bool Constrain(List<WFCTile> neighbourPossibilities, Direction direction)
    {
        bool reduced = false;
        var possibilitiesCopy = possibilities;
        if (entropy > 0)
        {
            List<WFCTile> connectors = new List<WFCTile>();
            foreach (WFCTile neighbourPossibility in neighbourPossibilities)
            {
                if (!connectors.Contains(neighbourPossibility))
                    connectors.Add(neighbourPossibility);
            }

            for (int i = possibilities.Count - 1; i >= 0; i--)
            {
                if (!connectors.Contains(possibilities[i]))
                {
                    possibilities.Remove(possibilities[i]);
                    reduced = true;
                }
            }

            entropy = possibilities.Count;
            if (entropy == 0 && !collapsed)
            {
                Debug.Log(possibilitiesCopy);
                SentrySdk.CaptureMessage("Test event");
            }
        }

        return reduced;
    }

    public WFCTile GetRandomValue()
    {
        System.Random random = new System.Random();

        Dictionary<TileType, List<WFCTile>> possibilitiesDictionary = GetPossibilityDictionary();
        List<WFCTile> selectedTileList = GetRandomTileList(possibilitiesDictionary);

        int randomValue = random.Next(0, selectedTileList.Count-1);

        return selectedTileList[randomValue];
    }

    private Dictionary<TileType, List<WFCTile>> GetPossibilityDictionary()
    {
        Dictionary<TileType, List<WFCTile>> possibilitiesTypes = new Dictionary<TileType, List<WFCTile>>();
        foreach (WFCTile tile in possibilities)
        {
            if (!possibilitiesTypes.Keys.Contains(tile.typeAndWeight.type))
            {
                possibilitiesTypes.Add(tile.typeAndWeight.type, new List<WFCTile>());
                possibilitiesTypes[tile.typeAndWeight.type].Add(tile);
            }
            else
            {
                possibilitiesTypes[tile.typeAndWeight.type].Add(tile);
            }

        }
        //ordering dictionary 
        return possibilitiesTypes;
    }

    private List<WFCTile> GetRandomTileList(Dictionary<TileType, List<WFCTile>> dictionary)
    {
        System.Random random = new System.Random();

        int totalWeight = 0;

        foreach (TileType value in dictionary.Keys)
        {
            totalWeight += WeightForType(value);
        }

        int randomValue = random.Next(0, totalWeight);

        int cursor = 0;
        foreach (TileType value in dictionary.Keys)
        {
            cursor += WeightForType(value);
            if (cursor >= randomValue)
            {
                return dictionary[value];
            }
        }
        return null;
    }

    private int WeightForType(TileType tileType)
    {
        if (tileType == TileType.Grass)
            return GameConstants.grassWeight;
        else if (tileType == TileType.Water)
            return GameConstants.waterWeight;
        else if (tileType == TileType.Beach)
            return GameConstants.beachWeight;
        else
            return 0;
    }

}

public class TypeWeight
{
    public TileType type;
    public int weight;
}