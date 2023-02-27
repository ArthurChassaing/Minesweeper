using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private static DontDestroyGridData dd;
    private static DontDestroyAudioSource audioSource;
    private static GameObject ErrorMessage;
    private static TextMeshProUGUI ErrorText;

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
        if (ErrorMessage == null)
        {
            ErrorMessage = Instantiate(Resources.Load<GameObject>("Prefabs/MenuText"));
            ErrorMessage.name = "Error Message";
            ErrorMessage.transform.position = new Vector2(0, -125);
            ErrorMessage.transform.SetParent(CustomGameMenu.transform, false);
            ErrorText = ErrorMessage.GetComponent<TextMeshProUGUI>();
            ErrorText.color = Color.red;
            ErrorMessage.SetActive(false);
        }
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
    public void StartGame()
    {
        // Make sure data is correct
        ErrorMessage.SetActive(true);
        if (Grid.IsSizeTooSmall(dd.width, dd.height))
        {
            ErrorText.text = "The size is too small!\nBoth width and height must be more than 0\nand one of them must be more than 3.";
            return;
        }
        if (Grid.IsMineCountIncorrect(dd.width, dd.height, dd.mineCount))
        {
            ErrorText.text = "Mine count is incorrect!\nMine count should be more than 1 or less than the width times the height minus 9.";
            return;
        }
        ErrorMessage.SetActive(false);
        SceneManager.LoadScene(1);
    }

    public void StartGame(int width, int height, int mineCount)
    {
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
