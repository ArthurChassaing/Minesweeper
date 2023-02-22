using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int MineCount { get; private set; }
    private Tile[,] grid;
    public Tile this[int x, int y] { get => grid[x, y]; }
    public Tile this[Vector2 coordinates] { get => grid[(int)coordinates.x, (int)coordinates.y]; }
    public Tile this[(int x, int y) coordinates] { get => grid[coordinates.x, coordinates.y]; }
    public bool isGridInitialized { get; private set; }
    public bool isMinesPlaced { get; private set; }

    public void Start()
    {
        isGridInitialized = false;
        isMinesPlaced = false;

        // /!\ REMOVE THE FOLLOWING LINES WHEN MENU IMPLEMENTED
        // TESTS PURPOSE ONLY /!\
        InitGrid(10, 10, 15);
        // REMOVE ABOUVE LINES /!\
    }
    public void Update()
    {
        if (!isGridInitialized) return;
        HandleInput();
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

        // Place the camera to show the entire grid
        transform.position = new Vector3(width * 0.5f - 0.5f, height * 0.5f - 0.5f, -10);
        Camera mainCam = GetComponent<Camera>();
        mainCam.orthographicSize = Mathf.Max(height * 0.5f, width * 0.5f / mainCam.aspect);

        isGridInitialized = true;
    }

    /// <summary>
    /// Place all mines in the grid.
    /// No mine will be placed on the clicked tile
    /// </summary>
    /// <param name="clickedTile">Position of the clicked tile in the grid</param>
    public void PlaceMines(Vector2 clickedTile)
    {
        if (!isGridInitialized) throw new Exception("The grid is not initialized");
        if (clickedTile.x < 0 || clickedTile.x >= Width || clickedTile.y < 0 || clickedTile.y >= Height)
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
        remaingTiles.Remove(((int)clickedTile.x, (int)clickedTile.y));

        // Place the bombs
        (int x, int y) rand;
        for (int i = 0; i < MineCount; i++)
        {
            rand = remaingTiles[UnityEngine.Random.Range(0, remaingTiles.Count)];
            this[rand].SetBomb();
            remaingTiles.Remove(rand);
        }
    }

    public Vector2 GetTileCoord()
    {
        Vector3 tileCoord = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(
            Mathf.Round(tileCoord.x),
            Mathf.Round(tileCoord.y));
    }

    public void HandleInput()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            if(!isMinesPlaced) 
            {
                PlaceMines(GetTileCoord());
                isMinesPlaced = true;
            }
            this[GetTileCoord()].Reveal(true);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            this[GetTileCoord()].ToggleFlagged();
        }
    }
}
