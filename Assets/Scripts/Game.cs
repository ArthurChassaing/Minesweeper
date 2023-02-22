using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int MineCount { get; private set; }
    private Tile[,] grid;
    public Tile this[int x, int y] { get => grid[x, y]; }
    public Tile this[(int x, int y) coordinates] { get => grid[coordinates.x, coordinates.y]; }
    public bool IsGridInitialized { get; private set; }
    public bool IsMinesPlaced { get; private set; }
    public bool IsGameEnded { get; private set; }
    public bool IsVictorious { get; private set; }

    private void Start()
    {
        IsGridInitialized = false;
        IsMinesPlaced = false;
        IsGameEnded = false;
        IsVictorious = false;

        // /!\ REMOVE THE FOLLOWING LINES WHEN MENU IMPLEMENTED
        // TESTS PURPOSE ONLY /!\
        InitGrid(10, 10, 15);
        // REMOVE ABOUVE LINES /!\
    }
    private void Update()
    {
        if (IsGameEnded) return;
        HandleInputs();
    }

    /// <summary>
    /// Create a grid with a size and a mine count.
    /// </summary>
    public void InitGrid(int width, int height, int mineCount)
    {
        if (Mathf.Max(width, height) <= 3) throw new ArgumentException("Size of the grid is too small");
        if (mineCount <= 1) throw new ArgumentException("Bomb count must me greater than 1");
        if (width * height < mineCount - 1) throw new Exception("Too many bombs");

        Width = width;
        Height = height;
        MineCount = mineCount;

        // Create and fill the grid with tiles
        grid = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Tile(x, y);

                // Add neighbors
                if (x > 0)
                {
                    if (y > 0)
                    {
                        // Top left corner
                        grid[x - 1, y - 1].AddNeighbor(grid[x, y]);
                        grid[x, y].AddNeighbor(grid[x - 1, y - 1]);
                    }

                    // Left tile
                    grid[x - 1, y].AddNeighbor(grid[x, y]);
                    grid[x, y].AddNeighbor(grid[x - 1, y]);

                    if (y < height - 1)
                    {
                        // Bottom left corner
                        grid[x - 1, y + 1].AddNeighbor(grid[x, y]);
                        grid[x, y].AddNeighbor(grid[x - 1, y + 1]);
                    }
                }
                if (y > 0)
                {
                    // Top tile
                    grid[x, y - 1].AddNeighbor(grid[x, y]);
                    grid[x, y].AddNeighbor(grid[x, y - 1]);
                }
            }
        }

        IsGridInitialized = true;
    }

    /// <summary>
    /// Center the camera on the grid and zoom perfectly to fit it in the screen.
    /// </summary>
    private void PlaceCamera()
    {
        transform.position = new Vector3(Width * 0.5f - 0.5f, Height * 0.5f - 0.5f, -10);
        Camera.main.orthographicSize = Mathf.Max(Width * 0.5f / Camera.main.aspect, Height * 0.5f);
    }

    /// <summary>
    /// Place all mines in the grid.
    /// No mine will be placed on the clicked tile
    /// </summary>
    /// <param name="clickedTile">Position of the clicked tile in the grid</param>
    private void PlaceMines(Tile clickedTile)
    {
        if (!IsGridInitialized) throw new Exception("The grid is not initialized");
        if (clickedTile.X < 0 || clickedTile.X >= Width || clickedTile.Y < 0 || clickedTile.Y >= Height)
            throw new ArgumentOutOfRangeException("The given position is not in the grid");

        // List tiles where we can put a mine on
        List<(int x, int y)> remaingTiles = new List<(int x, int y)>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                remaingTiles.Add((x, y));
            }
        }
        remaingTiles.Remove((clickedTile.X, clickedTile.Y));

        // Place the bombs
        (int x, int y) rand;
        for (int i = 0; i < MineCount; i++)
        {
            rand = remaingTiles[UnityEngine.Random.Range(0, remaingTiles.Count)];
            this[rand].SetBomb();
            remaingTiles.Remove(rand);
        }
    }

    /// <summary>
    /// Get the tile under the mouse. 
    /// </summary>
    /// <returns>The tile under the mouse or null if there are not.</returns>
    private Tile GetTileAtMouse()
    {
        if (!IsGridInitialized) return null; // Can't get tile cord if tiles do not exist!
        Vector3 tileCoord = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        try
        {
            return this[(int)Mathf.Round(tileCoord.x), (int)Mathf.Round(tileCoord.y)];
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }

    /// <summary>
    /// Handle the inputs in the game
    /// </summary>
    private void HandleInputs()
    {
        if (!IsGridInitialized) return; // Can't handle input wihout a grid

        // Left click -> reveal
        if (Input.GetButtonDown("Fire1"))
        {
            Tile clickedTile = GetTileAtMouse();
            if (clickedTile == null) return; // Must click on a tile!
            if (!IsMinesPlaced) 
            {
                PlaceMines(clickedTile);
                IsMinesPlaced = true;
            }
            if (clickedTile.Reveal(true) || CheckForVictory())
            {
                // Game ended 
                IsGameEnded = true;
            }
        }

        // Right click -> flag
        if (Input.GetButtonDown("Fire2"))
        {
            Tile clickedTile = GetTileAtMouse();
            if (clickedTile == null) return; // Must click on a tile!
            MineCount += clickedTile.ToggleFlagged() ? 1 : -1;
        }
    }

    /// <summary>
    /// Check for the victory in the grid.
    /// </summary>
    /// <returns>True if all mines are flagged and all other tiles are discovered, false otherwise</returns>
    private bool CheckForVictory()
    {
        if (!IsGridInitialized || !IsMinesPlaced) throw new Exception("Checking victory but the grid is not initialized or there are no mines in it");
        foreach (Tile t in grid)
        {
            if (!t.IsMine && !t.IsRevealed)
                return false;
        }
        IsVictorious = true;
        foreach(Tile t in grid)
        {
            if (t.IsMine && !t.IsFlagged) t.ToggleFlagged();
        }
        return true;
    }
}
