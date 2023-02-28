using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game : MonoBehaviour
{
    private float cameraSensibility;
    private bool dragging = false;

    private DontDestroyAudioSource audioSource;
    private Grid grid = null;

    [Header("Component")]
    public TextMeshProUGUI GameText;
    private float bestTime;
    private string stringBestTime;
    private float timer;

    void Start()
    {
        audioSource = FindAnyObjectByType<DontDestroyAudioSource>();
        if (audioSource == null)
            audioSource = new GameObject("Don't Destroy: Audio Source", typeof(DontDestroyAudioSource)).GetComponent<DontDestroyAudioSource>();

        cameraSensibility = PlayerPrefs.GetFloat("sensibility");

        InitGrid();
        PlaceCamera();
    }

    void Update()
    {
        HandleInputs();
        if (!grid.IsEnded) UiUpdate();
    }

    /// <summary>
    /// Init the grid. Destroy previous grid if it exists.
    /// Reset Ui of the game.
    /// </summary>
    public void InitGrid()
    {
        if (grid != null)
        {
            grid.Destroy();
        }
        int width = PlayerPrefs.GetInt("width");
        int height = PlayerPrefs.GetInt("height");
        int mineCount = PlayerPrefs.GetInt("mineCount");
        grid = new Grid(width, height, mineCount);

        string key = width + "x" + height + "/" + mineCount;
        bestTime = PlayerPrefs.GetFloat(key, -1);
        stringBestTime = bestTime == -1 ? "Not set" : stringFromTime(bestTime);
        timer = 0;
        GameText.color = Color.white;
        GameText.verticalAlignment = VerticalAlignmentOptions.Top;
        GameText.horizontalAlignment = HorizontalAlignmentOptions.Left;
        GameText.fontSize = 36;
        GameText.fontStyle = FontStyles.Normal;
        audioSource.PlayAudioStartGame();
    }

    /// <summary>
    /// Set UI in game with timer and minecount. 
    /// </summary>
    public void UiUpdate()
    {
        if (grid.IsMinesPlaced && !grid.IsEnded) { timer += Time.deltaTime; }
        GameText.text = "Mines left: " + (grid.MineCount - grid.FlagCount).ToString() + '\n';
        GameText.text += "Best time: " + stringBestTime + '\n';
        GameText.text += "Time: " + stringFromTime(timer, true);
    }


    /// <summary>
    /// Center the camera on the grid and zoom to show a maximum of 20 cells in height.
    /// </summary>
    public void PlaceCamera()
    {
        transform.position = new Vector3(grid.Width * 0.5f - 0.5f, grid.Height * 0.5f - 0.5f, -10);
        Camera.main.orthographicSize = Mathf.Max(grid.Width / Camera.main.aspect, grid.Height) / 2;
    }

    /// <summary>
    /// Transform a float corresponding to a duration in seconds into a string "[mintes]min [seconds]s"
    /// </summary>
    /// <param name="time"></param>
    static private string stringFromTime(float time, bool round = false) => (time > 60 ? (int)(time / 60) + "min " : "") + (round ? (time % 60).ToString("0") : (time % 60).ToString("0.00")) + "s";

    /// <summary>
    /// Save the score if it's the best time.
    /// </summary>
    private void SaveScore()
    {
        string key = grid.Width + "x" + grid.Height + "/" + grid.MineCount;
        float bestTime = PlayerPrefs.GetFloat(key, -1);
        if (bestTime == -1 || timer < bestTime)
        {
            PlayerPrefs.SetFloat(key, timer);
        }
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetBackToMenu();
            return;
        }

        // Scroll -> Zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - Input.mouseScrollDelta.y, 1 + Camera.main.orthographicSize % 1);
        }


        // Hold middle mouse button -> Move camera
        if (Input.GetKeyDown(KeyCode.Mouse2)) dragging = true;
        if (Input.GetKeyUp(KeyCode.Mouse2)) dragging = false;
        if (dragging)
        {
            Camera.main.transform.Translate(
                Time.deltaTime * Camera.main.orthographicSize * cameraSensibility *
                new Vector2(
                    -Input.GetAxis("Mouse X"),
                    -Input.GetAxis("Mouse Y")
            ));
        }

        // Left click -> Reveal tile
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Vector2Int clickPos = GetIntMouseCoordinates();
            if (grid.RevealTile(clickPos))
            {
                if (grid.IsEnded)
                {
                    EndGame();
                }
                else audioSource.PlayClick1();
            }
        }

        // Right click -> Toggle flag
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Vector2Int clickPos = GetIntMouseCoordinates();
            if (grid.ToggleFlagOnTile(clickPos))
            {
                audioSource.PlayClick2();
            }
        }
    }

    public void GetBackToMenu()
    {
        grid.Destroy();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Show the end game screen. Save the score if it's the best time.
    /// </summary>
    private void EndGame()
    {
        GameText.verticalAlignment = VerticalAlignmentOptions.Middle;
        GameText.horizontalAlignment = HorizontalAlignmentOptions.Center;
        GameText.fontSize = 50;
        GameText.fontStyle = FontStyles.Bold;
        if (grid.IsVictorious)
        {
            GameText.color = Color.green;
            GameText.text = "You win!\n";
            if (timer < bestTime || bestTime == -1)
            {
                GameText.text += "New best time !";
                SaveScore();
            }
            else GameText.text += "Best time: " + stringBestTime;
        }
        else
        {
            GameText.color = Color.red;
            GameText.text = "You lose !\nFlagged mines: " + grid.FlagCount + "/" + grid.MineCount;
            audioSource.PlayExplosion();
        }
        GameText.text += "\nTime: " + stringFromTime(timer, true);
    }
}
