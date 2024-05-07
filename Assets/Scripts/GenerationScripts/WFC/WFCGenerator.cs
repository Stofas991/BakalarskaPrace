using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WFCGenerator : Singleton<WFCGenerator>
{
    [SerializeField] GameObject WFCTileObject;
    public bool finished = false;
    private HashSet<WFCCell> constrainedList = new HashSet<WFCCell>();
    public int mountainLimit = 800;
    public int waterLimit = 1000;
    public bool waterLimited = false;
    public bool mountainLimited = false;
    public Tilemap grassTilemap;
    public Tilemap waterTilemap;
    public Tilemap mountainTilemap;
    public TileBase grassBase;
    public TileBase waterBase;
    public TileBase mountainBase;

    public WFCCell PlaceTiles(int posX, int posY, Transform parent)
    {
        var instance = Instantiate(WFCTileObject, new Vector3(posX, posY, 0), Quaternion.identity);
        instance.transform.parent = parent;
        return instance.GetComponent<WFCCell>();
    }

    public void WaveFunction(WFCCell cellToCollapse)
    {
        cellToCollapse.Collapse();

        if (constrainedList.Contains(cellToCollapse))
            constrainedList.Remove(cellToCollapse);

        Stack<WFCCell> stack = new Stack<WFCCell>();
        HashSet<WFCCell> processedCells = new HashSet<WFCCell>();

        stack.Push(cellToCollapse);

        while (stack.Count > 0)
        {
            WFCCell cell = stack.Pop();
            var directions = cell.GetDirections();
            var possibilities = cell.GetPossibilities();

            foreach (var direction in directions)
            {
                var neighbour = cell.GetNeighbour(direction);
                if (neighbour.entropy != 0 && !processedCells.Contains(neighbour))
                {
                    var reduced = neighbour.Constrain(possibilities, direction);
                    if (!neighbour.collapsed && neighbour.entropy == 0)
                    {
                        return;
                    }

                    if (reduced)
                    {
                        constrainedList.Add(neighbour);
                        stack.Push(neighbour);
                        processedCells.Add(neighbour);
                    }
                }
            }
        }
    }

    public List<WFCCell> GetLowestEntropy(Transform parent)
    {
        constrainedList.RemoveWhere(x => x == null);
        List<WFCCell> lowestEntropyCells = new List<WFCCell>();
        int lowestEntropy = int.MaxValue;

        if (constrainedList.Count > 0)
        {
            foreach (WFCCell currentCell in constrainedList)
            {
                if (currentCell.entropy > 0 && currentCell.entropy < lowestEntropy)
                {
                    lowestEntropyCells.Clear();
                    lowestEntropy = currentCell.entropy;
                    lowestEntropyCells.Add(currentCell);
                }
                else if (currentCell.entropy == lowestEntropy)
                {
                    lowestEntropyCells.Add(currentCell);
                }
            }
        }
        else
        {
            foreach (Transform child in parent)
            {
                WFCCell currentCell = child.GetComponent<WFCCell>();
                if (currentCell.entropy > 0 && currentCell.entropy < lowestEntropy)
                {
                    lowestEntropyCells.Clear();
                    lowestEntropy = currentCell.entropy;
                    lowestEntropyCells.Add(currentCell);
                }
                else if (currentCell.entropy == lowestEntropy)
                {
                    lowestEntropyCells.Add(currentCell);
                }
            }
        }
        return lowestEntropyCells;
    }
}