using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
  // track all the rows within the grid, track all of the cells too

    public TileRow[] rows { get; private set; }

    public TileCell[] cells { get; private set; }

    public int size => cells.Length;

    public int height => rows.Length;

    public int width => size / height;


    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();


        cells = GetComponentsInChildren<TileCell>();
    }

    private void Start() // first calling by unity ! capital
    {
        for (int y = 0; y < rows.Length; y++) // i == y axis
        {
            for (int x = 0; x < rows[y].cells.Length; x++) // j == x axis
            {
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }

    public TileCell GetCell(int x, int y) // for accessing cell
    {
        if(x >= 0 && x < width && y >=0 && y < height)
        {
            return rows[y].cells[x];
        } else
        {
            return null;
        }
         
    }

    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }


    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        // question: if celss coordinates's are on the bottom corners?
        // what will happen then??

        return GetCell(coordinates);
    }

    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, cells.Length);
        int startingIndex = index;
        while (cells[index].occupied)
        {
            index++;

            if(index >= cells.Length)
            {
                index = 0;
            }
            if (index == startingIndex)
            {
                return null;
            }
        } 
        return cells[index];
    }
}
