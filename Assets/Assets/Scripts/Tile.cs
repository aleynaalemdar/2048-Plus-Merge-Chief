using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for reference to image component
using TMPro; // to declare a field for our text



public class Tile : MonoBehaviour
{
  // needs to manage what state it's currently in
  public TileState state { get; private set; }

  public TileCell cell { get; private set; }

  public int number { get; private set; }

  public bool locked { get;  set; } // for prevent all of them merged at same time

  private Image background;

  private TextMeshProUGUI text;

   private void Awake()
   {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();

   }

    public void SetState(TileState state, int number)
    {
        this.state = state;
        this.number = number;

        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();

    }

    public int GetNumber(int number)
    {
        return this.number;
    }

    public void Spawn(TileCell cell)
    {
        Tile tile = this;
        if (tile.cell != null){
            tile.cell.tile = null;
        }

        tile.cell = cell;
        tile.cell.tile = tile;

        transform.position = cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        StartCoroutine(Animate(cell.transform.position,false));
    }

    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }
        this.cell = null; // is it because its destroyeded?
        cell.tile.locked = true; // locking  

        StartCoroutine(Animate(cell.transform.position,true));

    }

    // FOR ANIMATION EFFECT 
    private IEnumerator Animate(Vector3 to,bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        if (merging)
        {
            Destroy(gameObject); // !!!!!!
        }
    }
}
