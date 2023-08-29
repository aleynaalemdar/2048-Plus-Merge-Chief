using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    // they dont move so we need to give them coordinates
    // getting coordinates by getters and setters from grid

    public Vector2Int coordinates { get; set; }
    public Tile tile { get; set; }
    // track which cell on that tile
    // if cell is empty and occupied or occupied
    public bool empty => tile == null;
    public bool occupied => tile != null;

    // how can it tell its occupied?
}
