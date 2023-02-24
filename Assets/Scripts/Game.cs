using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Data;

public class Game : MonoBehaviour
{
    private Grid grid = null;

    [Header("Component")]
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI MineCount;
    private DontDestroy dd;
    private float timer; 

    void Start()
    {
        // Get data from DontDestroy
        dd = FindAnyObjectByType<DontDestroy>();
        InitGrid();
        PlaceCamera();
        
    }
    void Update()
    {
        HandleInputs();
        UiUpdate();
    }
    

    /// <summary>
    /// Initialize grid of the game
    /// </summary>
    public void InitGrid()
    {
        if(grid != null) { grid.Destroy(); }
        grid = new Grid(dd.width, dd.height, dd.mineCount);
        timer = 0;
    }

    /// <summary>
    /// Set UI in game with timer and minecount. 
    /// </summary>
    public void UiUpdate()
    {
        if (grid.IsMinesPlaced && !grid.IsEnded) { timer += Time.deltaTime; }
        TimerText.text = timer.ToString("0");
        MineCount.text = (grid.MineCount - grid.FlagCount).ToString();
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
