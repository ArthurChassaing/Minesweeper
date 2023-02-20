using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public struct Coordinates
    {
        public Coordinates(int x, int y)
        {
            this.x = x;
            this.y = x;
        }
        public int x, y;
    }

    public (int w, int h) Size { get; private set; }
    public int BombCount { get; private set; }
    private Tile[,] grid;
    public Tile this[int x, int y] { get => grid[x, y]; }
    public Tile this[Coordinates coordinates] { get => grid[coordinates.x, coordinates.y];}
    public bool IsInitialized { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        IsInitialized = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsInitialized) return;
        // TODO: Handle mouse input (click on tile)
    }

    /// <summary>
    /// Initialize the grid with a size and a bomb count.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="bombCount"></param>
    public void Init((int w, int h) size, int bombCount)
    {
        if (Math.Max(size.w, size.h) <= 3) throw new ArgumentException("Size of the grid is too small");
        if (bombCount <= 1) throw new ArgumentException("Bomb count must me greater than 1");
        if (size.w * size.h >= bombCount - 1) throw new Exception("Too many bombs");

        Size = size;
        BombCount = bombCount;
        grid = new Tile[size.w, size.h];
        for (int x = 0; x < size.w; x++)
        {
            for (int y = 0; y < size.h; y++)
            {
                grid[x, y] = new Tile(false);

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

                    if (y < size.h - 1)
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

        IsInitialized = true;
    }

    /// <summary>
    /// Place all bombs in the grid.
    /// No bomb will be placed on the clicked tile
    /// </summary>
    /// <param name="clickedTile">Position of the clicked tile in the grid</param>
    public void PlaceBombs(Coordinates clickedTile)
    {
        if (!IsInitialized) throw new ArgumentOutOfRangeException("The grid is not initialized");
        if (clickedTile.x < 0 || clickedTile.x >= Size.w || clickedTile.y < 0 || clickedTile.y >= Size.h)
            throw new ArgumentOutOfRangeException("The given position is not in the grid");

        // List tiles where we can put a mine on
        List<Coordinates> remaingTiles = new List<Coordinates>();
        for (int x = 0; x < Size.w; x++)
        {
            for (int y = 0; y < Size.h; y++)
            {
                remaingTiles.Add(new Coordinates(x, y));
            }
        }
        if (!remaingTiles.Remove(clickedTile)) throw new Exception(); // if exception is triggerred replace the whole line by: remaingTiles.RemoveAt(remaingTiles.FindIndex(c => c.x == clickedTile.x && c.y == clickedTile.y));

        // Place the bombs
        Coordinates rand;
        for (int i = 0; i < BombCount; i++)
        {
            rand = remaingTiles[UnityEngine.Random.Range(0, remaingTiles.Count)];
            this[rand].SetBomb();
            remaingTiles.Remove(rand);
        }
    }
}
