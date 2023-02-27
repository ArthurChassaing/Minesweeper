using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private static DontDestroyGridData dd;
    private static DontDestroyAudioSource audioSource;

    [Header("Menus")]
    public GameObject MainMenu;
    public GameObject SettingsMenu;
    public GameObject StartGameMenu;
    public GameObject CustomGameMenu;

    private void Awake()
    {
        dd = FindAnyObjectByType<DontDestroyGridData>();
        if (dd == null)
            dd = new GameObject("Don't Destroy: Grid Data", typeof(DontDestroyGridData)).GetComponent<DontDestroyGridData>();
        audioSource = FindAnyObjectByType<DontDestroyAudioSource>();
        if (audioSource == null)
            audioSource = new GameObject("Don't Destroy: Audio Source", typeof(DontDestroyAudioSource)).GetComponent<DontDestroyAudioSource>();
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

    private void OpenMenu(GameObject menu)
    {
        menu.SetActive(true);
        audioSource.PlayClick1();
    }

    public void OpenMainMenu() => OpenMenu(MainMenu);
    public void CloseMainMenu() => MainMenu.SetActive(false);
    public void OpenSettingsMenu() => OpenMenu(SettingsMenu);
    public void CloseSettingsMenu() => SettingsMenu.SetActive(false);
    public void OpenStartGameMenu() => OpenMenu(StartGameMenu);
    public void CloseStartGameMenu() => StartGameMenu.SetActive(false);
    public void OpenCustomGameMenu() => OpenMenu(CustomGameMenu);
    public void CloseCustomGameMenu() => CustomGameMenu.SetActive(false);
    
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
