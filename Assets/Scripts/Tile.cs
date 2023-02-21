using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public static GameObject SquarePrefab { get; } = Resources.Load<GameObject>("Prefabs/Square");
    public static Sprite BlankSprite { get; } = Resources.Load<Sprite>("Textures/BlankSquare");
    public static Sprite FlagSprite { get; } = Resources.Load<Sprite>("Textures/FlagSquare");
    public static Sprite MineSprite { get; } = Resources.Load<Sprite>("Textures/MineSquare");
    public static Sprite MineClickedSprite { get; } = Resources.Load<Sprite>("Textures/MineClickedSquare");
    public static Sprite MineCrossedSprite { get; } = Resources.Load<Sprite>("Textures/MineCrossedSquare");
    public static Sprite UnknownSprite { get; } = Resources.Load<Sprite>("Textures/UnknownSquare");
    public const int SpriteSize = 32;

    public int X { get; }
    public int Y { get; }
    public bool IsMine { get; private set; }
    public bool IsFlagged { get; private set; }
    public bool IsRevealed { get; private set; }
    private List<Tile> neighbors;
    private GameObject gameObject;

    public Tile(int x, int y)
    {
        X = x;
        Y = y;
        IsMine = false;
        IsFlagged = false;
        IsRevealed = false;
        neighbors = new List<Tile>();
        gameObject = Object.Instantiate(SquarePrefab, new Vector3(X, Y), Quaternion.identity);
    }

    public void SetBomb()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = MineSprite;
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
        if (IsFlagged) return false; // Can't reveal a flagged tile

        IsRevealed = true;
        if (IsMine)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = MineClickedSprite;
            return true;
        }

        // Reavel itself
        int bombCount = 0;
        int flagCount = 0;
        foreach (Tile neighbor in neighbors)
        {
            if (neighbor.IsMine)
            {
                bombCount++;
            }
            if (neighbor.IsFlagged)
            {
                flagCount++;
            }
        }
        gameObject.GetComponent<SpriteRenderer>().sprite = BlankSprite;
        // TODO: Write number of mines around

        // Reveal all neighbors
        if (bombCount != flagCount) return false; // Can't reveal if there are mines not flagged around
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
