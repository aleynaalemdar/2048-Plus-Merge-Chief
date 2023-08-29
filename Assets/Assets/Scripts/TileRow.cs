using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRow : MonoBehaviour
{
    // each row needs to keep track of all the cells within that row

    public TileCell[] cells { get; private set; }

    // unity call this func when this script is first initialized
    private void Awake() // A is capital !
    {
        cells = GetComponentsInChildren<TileCell>();
    }


}
