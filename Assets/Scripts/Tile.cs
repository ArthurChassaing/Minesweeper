using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile
{
    public static Dictionary<int, Color> NumColors { get; } = new()
    {
        { 0, new Color(0, 0, 0, 0) }, // 0 is transparent
        { 1, new Color(0, 0, 250, 255) },
        { 2, new Color(0, 125, 0, 255) },
        { 3, new Color(240, 20, 20, 255) },
        { 4, new Color(0, 0, 125, 255) },
        { 5, new Color(120, 0, 0, 255) },
        { 6, new Color(0, 130, 130, 255) },
        { 7, new Color(132, 0, 132, 255) },
        { 8, new Color(123, 123, 123, 255) },
    };

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
        gameObject = Object.Instantiate(SquarePrefab, new Vector2(X, Y), Quaternion.identity);
        gameObject.name = "Tile at (" + X + ", " + Y + ")";
    }

    public void SetBomb()
    {
        IsMine = true;
    }

    public void ToggleFlagged()
    {
        IsFlagged = !IsFlagged;
        if (IsFlagged)
            gameObject.GetComponent<SpriteRenderer>().sprite = FlagSprite;
        else
            gameObject.GetComponent<SpriteRenderer>().sprite = UnknownSprite;
    }

    public void AddNeighbor(Tile neighbor)
    {
        neighbors.Add(neighbor);
    }

    /// <summary>
    /// Reveal a tile and it's neighbours if it's empty.
    /// </summary>
    /// <param name="fromClick">true if called by the user, else if it's an automatic reveal (called by another tile)</param>
    /// <returns>True if a mine is revealed, false otherwise</returns>
    public bool Reveal(bool fromClick)
    {
        if (!fromClick && IsRevealed) return false; // Can't reveal an already revealed tile
        if (fromClick && IsFlagged) return false; // Can't click on a flagged tile
        if (IsFlagged && IsMine) return false; // Atomatic reveal can't reveal a flagged mine, but remove a flag on non-mine tiles

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
        gameObject.GetComponentInChildren<TextMeshPro>().text = bombCount.ToString();
        gameObject.GetComponentInChildren<TextMeshPro>().color = NumColors.TryGetValue(bombCount, out Color c) ? c : new Color(0, 0, 0, 255);

        // Reveal all neighbors
        if (fromClick ? bombCount != flagCount : bombCount > 0)
        {
            // Automatic reveal can't reveal around a number,
            // but user can if the number of flag equals the number
            return false;
        }
        foreach (Tile neighbor in neighbors)
        {
            neighbor.Reveal(false);
        }
        return false;
    }
}
