using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public static Sprite BlankSprite { get; } = Resources.Load<Sprite>("BlankSquare.png");
    public static Sprite FlagSprite { get; } = Resources.Load<Sprite>("FlagSquare.png");
    public static Sprite MineSprite { get; } = Resources.Load<Sprite>("MineSquare.png");
    public static Sprite MineClickedSprite { get; } = Resources.Load<Sprite>("MineClickedSquare.png");
    public static Sprite MineCrossedSprite { get; } = Resources.Load<Sprite>("MineCrossedSquare.png");
    public static Sprite UnknownSprite { get; } = Resources.Load<Sprite>("UnknownSquare.png");
    public const int SpriteSize = 32;

    public bool IsMine { get; private set; }
    public bool IsFlagged { get; private set; }
    public bool IsRevealed { get; private set; }
    private List<Tile> neighbors;
    private GameObject gameObject;

    public Tile(bool isMine)
    {
        IsMine = isMine;
        IsFlagged = false;
        IsRevealed = false;
        neighbors = new List<Tile>();
        gameObject = new GameObject("Tile", typeof(SpriteRenderer));
        gameObject.GetComponent<SpriteRenderer>().sprite = UnknownSprite;
    }
    
    public void SetBomb()
    {
        IsMine = true;
    }

    public void SetFlagged(bool flagged)
    {
        IsFlagged = flagged;
    }

    public void AddNeighbor(Tile neighbor)
    {
        neighbors.Add(neighbor);
    }

    /// <summary>
    /// Reveal a tile and it's neighbours if it's empty.
    /// </summary>
    /// <returns>True if a mine is revealed, false otherwise</returns>
    public bool Reveal()
    {
        IsRevealed = true;
        if (IsMine)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = MineClickedSprite;
            return true;
        }

        // Reavel itself
        int bombCount = 0;
        foreach (Tile neighbor in neighbors)
        {
            if (neighbor.IsMine)
            {
                bombCount++;
            }
        }
        gameObject.GetComponent<SpriteRenderer>().sprite = BlankSprite;
        // TODO: Write number of mines around

        // Reveal all neighbors
        if (bombCount != 0) return false; // Can't reveal if there are mines around
        foreach (Tile neighbor in neighbors)
        {
            if (!neighbor.IsRevealed)
            {
                neighbor.Reveal();
            }
        }
        return false;
    }
}
