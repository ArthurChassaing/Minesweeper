using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    private Grid grid = null;

    void Start()
    {
        // Get data from DontDestroy
        DontDestroy dd = FindAnyObjectByType<DontDestroy>();
        grid = new Grid(dd.width, dd.height, dd.mineCount);
        PlaceCamera();
    }
    void Update()
    {
        HandleInputs();
    }


    /// <summary>
    /// Center the camera on the grid and zoom perfectly to fit it in the screen.
    /// </summary>
    public void PlaceCamera()
    {
        transform.position = new Vector3(grid.Width * 0.5f - 0.5f, grid.Height * 0.5f - 0.5f, -10);
        Camera.main.orthographicSize = Mathf.Max(grid.Width * 0.5f / Camera.main.aspect, grid.Height * 0.5f);
    }

    /// <summary>
    /// Get the coordinates of the mouse, rounded to integer. 
    /// </summary>
    /// <returns>Pair of int corresponding to the coordinate of the mouse</returns>
    private Vector2Int GetIntMouseCoordinates()
    {
        Vector2 coords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2Int((int)Mathf.Round(coords.x), (int)Mathf.Round(coords.y));
    }

    /// <summary>
    /// Handle the inputs in the game
    /// </summary>
    private void HandleInputs()
    {
        // Escape -> Return to menu
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        // Left click -> reveal
        if (Input.GetButtonDown("Fire1"))
        {
            Vector2Int clickPos = GetIntMouseCoordinates();
            grid.RevealTile(clickPos);
        }

        // Right click -> flag
        if (Input.GetButtonDown("Fire2"))
        {
            Vector2Int clickPos = GetIntMouseCoordinates();
            grid.ToggleFlagOnTile(clickPos);
        }
    }
}
