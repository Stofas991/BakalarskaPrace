using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WFCCell : MonoBehaviour
{
    [SerializeField] List<WFCTile> possibilities;
    public int entropy;
    public bool collapsed = false;
    public int totalTiles;
    Dictionary<Direction, WFCCell> neighbours = new Dictionary<Direction, WFCCell>();

    public void AddNeighbour(Direction direction, WFCCell cell)
    {
        neighbours[direction] = cell;
        totalTiles = possibilities.Count();
    }

    public WFCCell GetNeighbour(Direction direction)
    {
        return neighbours[direction];
    }

    public List<WFCTile> GetPossibilities()
    {
        return possibilities;
    }

    public void SetPossibilities(WFCTile possibilitiesGiven)
    {
        possibilities.Clear();
        possibilities.Add(possibilitiesGiven);
    }

    public Direction[] GetDirections()
    {
        return neighbours.Keys.ToArray();
    }

    public WFCTile Collapse()
    {
        // Vyber náhodný prvek z možností podle váhy
        WFCTile collapsedTile = GetRandomWeightedTile();

        // Vyèisti seznam možností a pøidej pouze vybraný prvek
        possibilities.Clear();
        possibilities.Add(collapsedTile);

        // Instantializuj vybraný prvek
        Instantiate(collapsedTile, transform);

        // Nastav entropii na nulu a oznaè buòku jako collapsed
        entropy = 0;
        collapsed = true;

        return collapsedTile;
    }

    private WFCTile GetRandomWeightedTile()
    {
        System.Random random = new System.Random();

        // Seøaï možnosti podle váhy (vzestupnì)
        possibilities = possibilities.OrderBy(x => x.weight).ToList();

        // Seøaï možnosti podle typu dlaždice pro další vyvážený výbìr
        Dictionary<TileType, List<WFCTile>> possibilityDictionary = GetPossibilityDictionary();
        possibilityDictionary = possibilityDictionary.OrderBy(x => WeightForType(x.Key)).ToDictionary(x => x.Key, x => x.Value);

        // Vytvoø seznam dlaždic podle váhy pro další výbìr
        List<WFCTile> selectedTiles = GetRandomTileList(possibilityDictionary);

        // Vyber náhodný prvek z vyváženého seznamu
        return GetRandomItem(selectedTiles, x => x.weight);
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
                if (neighbourPossibility.directionNeighbours.Count == 0)
                    neighbourPossibility.SetValues();

                connectors.AddRange(neighbourPossibility.directionNeighbours[direction]);
            }

            connectors = connectors.Distinct().ToList();

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
            }
            
        }

        return reduced;
    }

    public WFCTile GetRandomValue()
    {
        System.Random random = new System.Random();
        //solution 1
        possibilities = possibilities.OrderBy(x => x.weight).ToList();
        Dictionary<TileType, List<WFCTile>> possibilitiesDictionary = GetPossibilityDictionary();
        possibilitiesDictionary = possibilitiesDictionary.OrderBy(x => WeightForType(x.Key)).ToDictionary(x => x.Key, x => x.Value);
        List<WFCTile> selectedTileList = GetRandomTileList(possibilitiesDictionary);

        var randomValue = random.Next(0, selectedTileList.Count - 1);

        return GetRandomItem(selectedTileList, x => x.weight);

        //solution 2
        /*
        var maxWeight = possibilities.Max(x => x.weight);
        int randomValue = random.Next(0, maxWeight);
        possibilities = possibilities.OrderBy(x => x.weight).ToList();

        List<WFCTile> sameWeightTiles = new List<WFCTile>();
        foreach (var possibility in possibilities)
        {
            if (possibility.weight >= randomValue)
            {
                if (!sameWeightTiles.Any(x => x.weight < possibility.weight))
                {
                    sameWeightTiles.Add(possibility);
                }
            }
        }
        randomValue = random.Next(0, sameWeightTiles.Count - 1);

        return sameWeightTiles[randomValue];
        */

        //solution 3
        //return GetRandomItem(possibilities, x => x.weight);
    }

    private Dictionary<TileType, List<WFCTile>> GetPossibilityDictionary()
    {
        Dictionary<TileType, List<WFCTile>> possibilitiesTypes = new Dictionary<TileType, List<WFCTile>>();
        foreach (WFCTile tile in possibilities)
        {
            if (!possibilitiesTypes.Keys.Contains(tile.tileType))
            {
                possibilitiesTypes.Add(tile.tileType, new List<WFCTile>());
                possibilitiesTypes[tile.tileType].Add(tile);
            }
            else
            {
                possibilitiesTypes[tile.tileType].Add(tile);
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
        else if (tileType == TileType.Mountain)
            return GameConstants.treesWeight;
        else
            return 0;
    }

    public T GetRandomItem<T>(IEnumerable<T> itemsEnumerable, Func<T, int> weightKey)
    {
        System.Random random = new System.Random();

        var items = itemsEnumerable.ToList();

        var totalWeight = items.Sum(x => weightKey(x));
        var randomWeightedIndex = random.Next(totalWeight);
        var itemWeightedIndex = 0;
        foreach (var item in items)
        {
            itemWeightedIndex += weightKey(item);
            if (randomWeightedIndex < itemWeightedIndex)
                return item;
        }
        throw new ArgumentException("Collection count and weights must be greater than 0");
    }

}

public class TypeWeight
{
    public TileType type;
    public int weight;
}