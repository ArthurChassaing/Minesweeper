using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
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

    public void Reveal()
    {
        IsRevealed = true;
        if (IsBomb)
        {
            // TODO: Game over
            return;
        }

        // Reavel itself
        // TODO: change sprite
        int bombCount = 0;
        foreach (Tile neighbor in neighbors)
        {
            if (neighbor.IsBomb)
            {
                bombCount++;
            }
        }
        // TODO: Write number of bombs around

        // Reaveal all neighbors
        if (bombCount != 0) return; // Can't reveal if there are bombs around
        foreach (Tile neighbor in neighbors)
        {
            if (!neighbor.IsRevealed)
            {
                neighbor.Reveal();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
