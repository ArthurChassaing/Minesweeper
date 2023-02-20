using System.Collections.Generic;

public class Tile
{
    public bool IsBomb { get; private set; }
    public bool IsFlagged { get; private set; }
    public bool IsRevealed { get; private set; }
    private List<Tile> neighbors;

    public Tile(bool isBomb)
    {
        IsBomb = isBomb;
        IsFlagged = false;
        IsRevealed = false;
        neighbors = new List<Tile>();
    }
    
    public void SetBomb()
    {
        IsBomb = true;
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
    /// <returns>True if a bomb is revealed, false otherwise</returns>
    public bool Reveal()
    {
        IsRevealed = true;
        if (IsBomb)
        {
            return true;
        }

        // Reavel itself
        int bombCount = 0;
        foreach (Tile neighbor in neighbors)
        {
            if (neighbor.IsBomb)
            {
                bombCount++;
            }
        }
        // TODO: change sprite
        // TODO: Write number of bombs around

        // Reveal all neighbors
        if (bombCount != 0) return false; // Can't reveal if there are bombs around
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
