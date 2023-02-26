using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private static DontDestroy dd;
   
    private void Awake()
    {
        dd = FindAnyObjectByType<DontDestroy>();
        if (dd == null)
            dd = new GameObject("Don't Destroy: Grid Data", typeof(DontDestroy)).GetComponent<DontDestroy>();
        PlaceCamera();
    }

    /// <summary>
    /// Set Camera position and size to it's default values.
    /// </summary>
    private void PlaceCamera() => Camera.main.Reset();

    /// <summary>
    /// Close the application.
    /// </summary>
    public void QuitGame() => Application.Quit();

    // Start games

    /// <summary>
    /// Change scene to GameScene. Data is send via the DontDestroy object.
    /// </summary>
    public void StartGame() => SceneManager.LoadScene(1);

    public void StartGame(int width, int height, int mineCount)
    {
        // Make sure data is correct
        if (Grid.IsSizeTooSmall(width, height)) return;
        if (Grid.IsMineCountIncorrect(width, height, mineCount)) return;

        // Give data to DontDestroy
        dd.width = width;
        dd.height = height;
        dd.mineCount = mineCount;
        StartGame();
    }

    public void StartBeginnerGame() => StartGame(9, 10, 10);

    public void StartIntermediateGame() => StartGame(16, 16, 40);

    public void StartExpertGame() => StartGame(29, 16, 99);

    // Parse fields

    public void ParseWidthField(string value) => int.TryParse(value, out dd.width);
    public void ParseHeightField(string value) => int.TryParse(value, out dd.height);
    public void ParseMineCountField(string value) => int.TryParse(value, out dd.mineCount);

}
