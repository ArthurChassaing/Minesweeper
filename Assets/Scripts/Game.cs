using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game : MonoBehaviour
{
    const int tileSpriteSize = 16;
    static readonly float moveDeadZone = 0.1f;

    private bool mouse0Held = false;
    private bool mouse1Held = false;
    private bool dragging = false;
    Vector2 mouseMovement = Vector2.zero;

    private DontDestroy dd;
    private Grid grid = null;

    [Header("Component")]
    public TextMeshProUGUI TopLeftText;
    private float timer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip click;
    public AudioClip explosion;
    public AudioClip flag;
    public AudioClip victory;

    void Start()
    {
        // Get data from DontDestroy
        dd = FindAnyObjectByType<DontDestroy>();
        if (dd == null)
            dd = new GameObject("Don't Destroy: Grid Data", typeof(DontDestroy)).GetComponent<DontDestroy>();
        grid = new Grid(dd.width, dd.height, dd.mineCount);
        PlaceCamera();
    }

    void Update()
    {
        HandleInputs();
        UiUpdate();
    }

    /// <summary>
    /// Init the grid. Destroy previous grid if it exists.
    /// </summary>
    public void InitGrid()
    {
        if (grid != null) grid.Destroy();
        grid = new Grid(dd.width, dd.height, dd.mineCount);
        timer = 0;
    }

    /// <summary>
    /// Set UI in game with timer and minecount. 
    /// </summary>
    public void UiUpdate()
    {
        if (grid.IsMinesPlaced && !grid.IsEnded) { timer += Time.deltaTime; }
        TopLeftText.text = "Mines left: " + (grid.MineCount - grid.FlagCount).ToString() + '\n';
        TopLeftText.text += "Time: " + timer.ToString("0");
    }


    /// <summary>
    /// Center the camera on the grid and zoom perfectly to fit it in the screen.
    /// </summary>
    public void PlaceCamera()
    {
        transform.position = new Vector3(grid.Width * 0.5f - 0.5f, grid.Height * 0.5f - 0.5f, -10);
        Camera.main.orthographicSize = Mathf.Max(grid.Width / Camera.main.aspect, grid.Height) / 2;
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
            SceneManager.LoadScene(0);
            return;
        }

        // Scroll -> Zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - Input.mouseScrollDelta.y, 1 + Camera.main.orthographicSize % 1);
        }

        // Mouse inputs
        if (Input.GetKeyDown(KeyCode.Mouse0)) mouse0Held = true;
        if (Input.GetKeyDown(KeyCode.Mouse1)) mouse1Held = true;

        // Mouse hold any click
        if (mouse0Held || mouse1Held)
        {
            mouseMovement += new Vector2(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
            if (Mathf.Abs(mouseMovement.x) > moveDeadZone || Mathf.Abs(mouseMovement.y) > moveDeadZone)
            {
                dragging = true;
                Camera.main.transform.Translate(mouseMovement * Time.deltaTime * tileSpriteSize * Camera.main.orthographicSize);
                mouseMovement = Vector2.zero;
            }
        }

        // Left click
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (!dragging)
            {
                Vector2Int clickPos = GetIntMouseCoordinates();
                if (grid.RevealTile(clickPos))
                {
                    if (grid.IsEnded)
                    {
                        if (grid.IsVictorious)
                            audioSource.clip = victory;
                        else
                            audioSource.clip = explosion;
                    }
                    else audioSource.clip = click;
                    audioSource.Play();
                }
            }
            mouse0Held = false;
            mouseMovement = Vector2.zero;
            dragging = false;
        }

        // Right click
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (!dragging)
            {
                Vector2Int clickPos = GetIntMouseCoordinates();
                if (grid.ToggleFlagOnTile(clickPos))
                {
                    audioSource.clip = flag;
                    audioSource.Play();
                }
            }
            mouse1Held = false;
            mouseMovement = Vector2.zero;
            dragging = false;
        }
    }
}
