using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileBoard : MonoBehaviour
{
    public Tile tilePrefab;

    public GameManager gameManager;

    public TileState[] tileStates;

    private TileGrid grid;


    private List<Tile> tiles;

    private bool waiting;

    public float delayBetweenMoves = 1f; // for autoplaying


    public Vector2Int getNextMoveScore()
    {
        Vector2Int maxDirectionVert = Vector2Int.up;
        Vector2Int maxDirectionHorizon = Vector2Int.left;

        int a = MoveTilesScore(Vector2Int.up, 0, 1, 1, 1);
        int b = MoveTilesScore(Vector2Int.down, 0, 1, grid.height - 2, -1);
        int c = MoveTilesScore(Vector2Int.left, 1, 1, 0, 1);
        int d = MoveTilesScore(Vector2Int.right, grid.width - 2, -1, 0, 1);

        // Find the direction with the maximum score
        int maxScore = Mathf.Max(a, b, c, d);

        if (maxScore == a)
        {
            maxDirectionHorizon = Vector2Int.up;
        }
        else if (maxScore == b)
        {
            maxDirectionHorizon = Vector2Int.down;
        }
        else if (maxScore == c)
        {
            maxDirectionHorizon = Vector2Int.left;
        }
        else if (maxScore == d)
        {
            maxDirectionHorizon = Vector2Int.right;
        }

        return maxDirectionHorizon;
    }

    public int MoveTilesScore(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        int score = 0;

        // checking both bounds
        for (int x = startX; (x >= 0 && x < grid.width); x += incrementX)
        {
            for (int y = startY; (y >= 0 && y < grid.height); y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.occupied)
                {
                    score += MoveTileScore(cell.tile, direction);
                }
            }
        }
        return score;
    }



    private int MoveTileScore(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        
            while (adjacent != null)
            {
                if (adjacent.occupied)
                {
                    // To-do: merging
                    // if numbers are same merge

                    if (CanMerge(tile, adjacent.tile))
                    {
                        return adjacent.tile.number * 2;

                    }

                    break;
                }

                newCell = adjacent;
                adjacent = grid.GetAdjacentCell(adjacent, direction);
            }

        return newCell != null ? 1 : 0;
    }

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16); // capacity


    }
    private void Start()
    {
        //  RunSimulation();
    }

    public void RunSimulation()
    {
        StartCoroutine(AutoPlay());
    }

    IEnumerator AutoPlay()
    {
        while (!CheckForGameOver())
        {
            yield return new WaitForSeconds(0.5f);

            Vector2Int nextMove = getNextMoveScore();

            if (nextMove == Vector2Int.up)
            {
                MoveTiles(nextMove, 0, 1, 1, 1);
           
            }

            else if (nextMove == Vector2Int.down)
            {
                MoveTiles(nextMove, 0, 1, grid.height - 2, -1);
             
            }

            else if (nextMove == Vector2Int.left)
            {
                MoveTiles(nextMove, 1, 1, 0, 1);
               
            }

            else if (nextMove == Vector2Int.right)
            {
                MoveTiles(nextMove, grid.width - 2, -1, 0, 1);
               
            }

            if (CheckForGameOver())
            {
                gameManager.GameOver();
                yield break;
            }


        }
    }

    public string GetRandomMove()
    {
        string[] directions = { "up", "right", "down", "left" };

        var r = new System.Random();
        int randomIndex = r.Next(0, directions.Length);

        string nextMove = directions[randomIndex];

        return nextMove;
    }


    public void ClearBoard()
    {
        foreach (var cell in grid.cells)
        {
            cell.tile = null;
        }

        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }

    /*  private void Start()
      {
          CreateTile();
          CreateTile();
      }*/

    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }


    // we need to check for user input to know which direction
    // to move the tiles


    /* private void Update() // called by unity every single frame
     {
         if (!waiting)
         {
             // question = isn't that do something to work on mobile too or this is enough?? 
             if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
             {
                 // when moving up
                 MoveTiles(Vector2Int.up, 0, 1, 1, 1); // skip first one for y

             }
             else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
             {
                 // moving down
                 MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);

             }
             else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
             {
                 MoveTiles(Vector2Int.left, 1, 1, 0, 1);

             }
             else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
             {
                 MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
             }
         }
     }*/

    public void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        // checking both bounds
        for (int x = startX; (x >= 0 && x < grid.width); x += incrementX) // can incrementX be negative??
        {
            for (int y = startY; (y >= 0 && y < grid.height); y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.occupied)
                {
                    changed |= MoveTile(cell.tile, direction);
                    // once is true still is true
                    // |= !!!!!
                }
            }
        }
        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }



    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                // To-do: merging
                // if numbers are same merge

                if (CanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }
        if (newCell != null)
        {
            tile.MoveTo(newCell);
            // waiting = true; // once we move to a new cell
            // StartCoroutine(WaitForChanges());
            return true;

        }
        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.number == b.number && !b.locked;
    }

    private void Merge(Tile a, Tile b)
    {
        // destroyed one ,second one transformed
        tiles.Remove(a);
        a.Merge(b.cell);
        // looking state and then pass to another one
        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        // use last one if its finish in case 2048 because
        // i cant do some much of them
        int number = b.number * 2;

        b.SetState(tileStates[index], number);

        gameManager.IncreaseScore(number);


    }

   
    private Vector2 initialTouchPos;

    void Update()
    {
        if (!waiting)
        {
           
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                
                if (touch.phase == TouchPhase.Began)
                {
              
                    initialTouchPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    
                    Vector2 swipeDelta = touch.position - initialTouchPos;

                    if (swipeDelta.magnitude > 40f)
                    {
                        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                        {
                     
                            if (swipeDelta.x > 0)
                            {
                                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
                            }
                            else
                            {
                                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
                            }
                        }
                        else
                        {
                          
                            if (swipeDelta.y > 0)
                            {
                                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
                            }
                            else
                            {
                                MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
                            }
                        }
                    }
                }
            }

                // question = isn't that do something to work on mobile too or this is enough?? 
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    // when moving up
                    MoveTiles(Vector2Int.up, 0, 1, 1, 1); // skip first one for y

                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    // moving down
                    MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);

                }
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    MoveTiles(Vector2Int.left, 1, 1, 0, 1);

                }
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
                }
            }
        
    }

    // ... The rest of your code ...


    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i])
            {
                return i;
            }
        }
        return -1; // if not found, i hope not:D
    }


    private IEnumerator WaitForChanges()
    {
        waiting = true;

        yield return new WaitForSeconds(0.1f);

        waiting = false;

        foreach (var tile in tiles)
        {
            tile.locked = false;
        }

        if (tiles.Count != grid.size)
        { //  checking for spaces for creating new one
            CreateTile();
        }
        // to-do check for game over
        if (CheckForGameOver())
        {
            gameManager.GameOver();
        }



    }
    private bool CheckForGameOver()
    {
        if (tiles.Count != grid.size)
        {
            return false;

        }
        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile))
            {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile))
            {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile))
            {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile))
            {
                return false;
            }
        }

        return true;
    }


}