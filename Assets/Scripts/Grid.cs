using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int MineCount { get; private set; }
    public int FlagCount { get; private set; }
    private Tile[,] tiles;
    public Tile this[int x, int y] { get => tiles[x, y]; }
    public Tile this[Vector2Int coordinates] { get => tiles[coordinates.x, coordinates.y]; }

    public bool IsMinesPlaced { get; private set; }
    public bool IsEnded { get; private set; }
    public bool IsVictorious { get; private set; }

    /// <summary>
    /// Create a grid with a size and a mine count.
    /// </summary>
    public Grid(int width, int height, int mineCount)
    {
        if (IsSizeTooSmall(width, height)) throw new ArgumentException("Size of the grid is too small");
        if (IsMineCountIncorrect(width, height, mineCount)) throw new ArgumentException("The mine count is incorrect");

        Width = width;
        Height = height;
        MineCount = mineCount;
        FlagCount = 0;

        IsMinesPlaced = false;
        IsEnded = false;
        IsVictorious = false;

        // Create and fill the grid with tiles
        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(x, y);
            }
        }
    }

    /// <summary>
    /// Place all mines randomly in the grid.
    /// </summary>
    public void PlaceMines() => PlaceMines(null);

    /// <summary>
    /// Place all mines randomly in the grid.
    /// No mine will be placed on the clicked tile
    /// </summary>
    /// <param name="clickedPos">Position of the clicked tile in the grid</param>
    public void PlaceMines(Vector2Int? clickedPos)
    {
        if (!clickedPos.HasValue || !IsTileInGrid(clickedPos.Value))
            throw new ArgumentOutOfRangeException("The given position is not in the grid");

        // Place the mines
        Vector2Int rand;
        for (int i = 0; i < MineCount; i++)
        {
            // Get random coordinates (not a mine)
            do { rand = new Vector2Int(UnityEngine.Random.Range(0, Width), UnityEngine.Random.Range(0, Height)); }
            while (this[rand].IsMine || rand == clickedPos);

            this[rand].SetBomb();
        }
        IsMinesPlaced = true;
    }

    public bool IsTileInGrid(Vector2Int tilePosition) => 0 <= tilePosition.x && tilePosition.x < Width && 0 <= tilePosition.y && tilePosition.y < Height;

    /// <summary>
    /// Reveal the tile at the given position.
    /// </summary>
    /// <param name="clickPos"></param>
    /// <returns>True if the tile has been revealed, false otherwise</returns>
    public bool RevealTile(Vector2Int clickPos)
    {
        if (!IsTileInGrid(clickPos) || IsEnded) return false;
        if (!IsMinesPlaced) PlaceMines(clickPos);
        if (this[clickPos].IsFlagged) return false;
        bool clickOnRevealed = this[clickPos].IsRevealed;

        // Reveal all empty tiles connected to the tile revealed
        List<Tile> tilesToReveal = new() { this[clickPos] };
        Tile currentTile;
        List<Tile> neighbours;
        int mineCount, flagCount;
        while (tilesToReveal.Count > 0)
        {
            // Reveal the first tile in the list
            currentTile = tilesToReveal.First();
            if (currentTile.Reveal())
            {
                SetEnded();
                return true;
            }

            // Check for mines and flags in it's neighbourhood
            neighbours = GetNeighbours(currentTile);
            mineCount = 0;
            flagCount = 0;
            foreach (Tile n in neighbours)
            {
                if (n.IsMine) mineCount++;
                if (n.IsFlagged) flagCount++;
            }

            if (clickOnRevealed && mineCount > 0 && mineCount == flagCount)
            {
                foreach (Tile n in neighbours)
                {
                    if (!n.IsRevealed && !n.IsFlagged) tilesToReveal.Add(n);
                }
                clickOnRevealed = false;
                continue;
            }

            // No mines around: Reveal neighbours
            if (mineCount == 0)
            {
                tilesToReveal = tilesToReveal.Union(neighbours).ToList();
            }
            // Else: There is mines around: Write the number of mines on the tile
            else
            {
                currentTile.SetNumber(mineCount);
            }
            tilesToReveal.Remove(currentTile);
        }
        if (CheckForVictory())
        {
            SetEnded();
        }
        return true;
    }

    /// <summary>
    /// Get the neighbours of a tile.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private List<Tile> GetNeighbours(Tile tile)
    {
        List<Tile> list = new();
        Vector2Int neighbourPos = new();
        for (int x = tile.Position.x - 1; x <= tile.Position.x + 1; x++)
        {
            neighbourPos.x = x;
            for (int y = tile.Position.y - 1; y <= tile.Position.y + 1; y++)
            {
                neighbourPos.y = y;
                if (!IsTileInGrid(neighbourPos)) continue;
                if (!this[neighbourPos].IsRevealed) list.Add(this[neighbourPos]);
            }
        }
        return list;
    }

    /// <summary>
    /// Check for the victory in the grid.
    /// </summary>
    /// <returns>True if all mines are flagged and all other tiles are discovered, false otherwise</returns>
    private bool CheckForVictory()
    {
        foreach (Tile t in tiles)
        {
            if (!t.IsMine && !t.IsRevealed)
                return false;
        }
        IsVictorious = true;
        return true;
    }

    /// <summary>
    /// Flag or un-flag the tile at the given position.
    /// </summary>
    /// <param name="clickPos"></param>
    /// <returns>True if a flag as been placed or removed, else otherwise</returns>
    public bool ToggleFlagOnTile(Vector2Int clickPos)
    {
        if (!IsTileInGrid(clickPos) || IsEnded) return false;
        if (!IsMinesPlaced) PlaceMines();
        if (this[clickPos].IsRevealed) return false;
        FlagCount += this[clickPos].ToggleFlag() ? 1 : -1;
        return true;
    }

    /// <summary>
    /// Set grid to ended. Show mines locations.
    /// </summary>
    public void SetEnded()
    {
        foreach (Tile t in tiles)
        {
            if (t.IsMine)
            {
                if (IsVictorious) t.SetSprite(Tile.FlagSprite);
                else if (!t.IsRevealed) t.SetSprite(Tile.MineSprite);
            }
            else if (t.IsFlagged) t.SetSprite(Tile.MineCrossedSprite);
        }
        IsEnded = true;
    }

    /// <summary>
    /// Destroy each tile of the grid.
    /// </summary>
    public void Destroy()
    {
        foreach (Tile t in tiles)
        {
            t.Destroy();
        }
    }


    // Statics:

    /// <summary>
    /// Check if the size of the grid si too small.
    /// Is considered too small a grid with a width or a height less than 3.
    /// </summary>
    public static bool IsSizeTooSmall(int width, int height) => width < 3 || height < 3;

    /// <summary>
    /// Check if the mine count is incorrect.
    /// A mine count is incorrect if it's less than 1 or more than or equal to the width times the height.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="mineCount"></param>
    /// <returns></returns>
    public static bool IsMineCountIncorrect(int width, int height, int mineCount) => mineCount < 1 || width * height <= mineCount;
}
