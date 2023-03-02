using System;
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
    public Tile RunningBomb { get; set; } // Game mode "Running Bomb"

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
    /// No mine will be placed on the clicked tile and its neighbours.
    /// </summary>
    /// <param name="clickedPos">Position of the clicked tile in the grid</param>
    public void PlaceMines(Vector2Int? clickedPos)
    {
        if (!clickedPos.HasValue || !IsTileInGrid(clickedPos.Value))
            throw new ArgumentOutOfRangeException("The given position is not in the grid");

        List<Tile> clickedAndNeighbours = GetNeighbours(this[clickedPos.Value]).ToList();
        clickedAndNeighbours.Add(this[clickedPos.Value]);

        // Place the mines
        Vector2Int rand;
        for (int i = 0; i < MineCount; i++)
        {
            // Get random coordinates (not a mine)
            do { rand = new Vector2Int(UnityEngine.Random.Range(0, Width), UnityEngine.Random.Range(0, Height)); }
            while (this[rand].IsMine || clickedAndNeighbours.Contains(this[rand]));

            this[rand].SetMine();
            if (RunningBomb == null) RunningBomb = this[rand];
        }
        IsMinesPlaced = true;
    }

    /// <summary>
    /// Check if the given position correspond to a tile in the grid.
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <returns>True if the tile is in the grid, false otherwise</returns>
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
            currentTile = tilesToReveal.First();

            // Check for mines and flags in it's neighbourhood
            neighbours = GetNeighbours(currentTile);
            mineCount = 0;
            flagCount = 0;
            foreach (Tile n in neighbours)
            {
                if (n.IsMine) mineCount++;
                if (n.IsFlagged) flagCount++;
            }

            if (clickOnRevealed)
            {
                if (mineCount > 0 && mineCount == flagCount)
                {
                    foreach (Tile n in neighbours)
                    {
                        if (!n.IsRevealed && !n.IsFlagged) tilesToReveal.Add(n);
                    }
                    clickOnRevealed = false;
                    continue;
                }
                else return false;
            }

            // Reveal the tile
            else if (currentTile.Reveal())
            {
                SetEnded();
                return true;
            }

            // No mines around: Reveal neighbours
            if (mineCount == 0)
            {
                tilesToReveal = tilesToReveal.Union(neighbours.Where(n => !n.IsRevealed)).ToList();
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
                list.Add(this[neighbourPos]);
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
                else if (!t.IsRevealed && !t.IsFlagged) t.SetSprite(Tile.MineSprite);
            }
            else if (t.IsFlagged)
            {
                t.SetSprite(Tile.MineCrossedSprite);
                FlagCount--;
            }
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

    // Game mode specific methods:

    /// <summary>
    /// Move the RunningBomb to a random unrevealed tile next to it.
    /// </summary>
    public void MoveRunningBomb()
    {
        // Get the next possible positions
        List<Tile> oldNeighbours = GetNeighbours(RunningBomb);
        List<Tile> nextPositions = new();
        foreach (Tile n in oldNeighbours)
        {
            if (!n.IsRevealed && !n.IsMine) nextPositions.Add(n);
        }

        // If there is no next position, stop the bomb
        if (nextPositions.Count == 0)
        {
            return;
        }

        // Change the bomb position
        RunningBomb.SetMine(false);
        RunningBomb = nextPositions[UnityEngine.Random.Range(0, nextPositions.Count)];
        RunningBomb.SetMine(true);

        // Update the neighbours numbers
        foreach (Tile n in oldNeighbours)
        {
            if (n.IsRevealed) n.SetNumber(n.Number - 1);
        }
        foreach (Tile n in GetNeighbours(RunningBomb))
        {
            if (n.IsRevealed) n.SetNumber(n.Number + 1);

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
    /// A mine count is incorrect if it's less than 1 or more than the width times the height minus 9.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="mineCount"></param>
    /// <returns></returns>
    public static bool IsMineCountIncorrect(int width, int height, int mineCount) => mineCount < 1 || mineCount > width * height - 9;
}
