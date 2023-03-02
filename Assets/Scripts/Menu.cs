using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private static DontDestroyAudioSource audioSource;
    private static GameObject ErrorMessage;
    private static TextMeshProUGUI ErrorText;

    [Header("Menus")]
    public GameObject MainMenu;
    public GameObject SettingsMenu;
    public GameObject StartGameMenu;
    public GameObject GameModeDropdown;
    public GameObject CustomGameMenu;

    [Header("Input fields")]
    public GameObject WidthField;
    public GameObject HeightField;
    public GameObject MineCountField;

    [Header("Settings Menu")]
    public GameObject NumberVolume;
    public GameObject SliderVolume;
    public GameObject NumberSensibility;
    public GameObject SliderSensitivity;
    public GameObject FullscreenToggle;

    private int gridWidth;
    private int gridHeight;
    private int gridMineCount;

    private void Awake()
    {
        // Audio
        audioSource = FindAnyObjectByType<DontDestroyAudioSource>();
        if (audioSource == null)
            audioSource = new GameObject("Don't Destroy: Audio Source", typeof(DontDestroyAudioSource)).GetComponent<DontDestroyAudioSource>();

        // Game mode dropdown
        GameModeDropdown.GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt("gameMode", 0);

        // Error message (custom game)
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

        // Settings
        float volume = PlayerPrefs.GetFloat("volume", 0.5f);
        SliderVolume.GetComponent<Slider>().value = volume;
        float sensibility = PlayerPrefs.GetFloat("sensibility", 16f);
        SliderSensitivity.GetComponent<Slider>().value = sensibility;
        int fullscreen = PlayerPrefs.GetInt("fullscreen", 1);
        FullscreenToggle.GetComponent<Toggle>().isOn = fullscreen == 1;

        // Set default values
        gridWidth = PlayerPrefs.GetInt("width", 20);
        gridHeight = PlayerPrefs.GetInt("height", 20);
        gridMineCount = PlayerPrefs.GetInt("mineCount", 50);

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

    /// <summary>
    /// Open a menu.
    /// </summary>
    /// <param name="menu"></param>
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
    public void OpenCustomGameMenu()
    {
        WidthField.GetComponent<TMP_InputField>().text = gridWidth.ToString();
        HeightField.GetComponent<TMP_InputField>().text = gridHeight.ToString();
        MineCountField.GetComponent<TMP_InputField>().text = gridMineCount.ToString();
        OpenMenu(CustomGameMenu);
    }
    public void CloseCustomGameMenu() => CustomGameMenu.SetActive(false);

    // Start games

    /// <summary>
    /// Change scene to GameScene. Data is send via the DontDestroy object.
    /// </summary>
    public void StartGame()
    {
        // Make sure data is correct
        ErrorMessage.SetActive(true);
        if (Grid.IsSizeTooSmall(gridWidth, gridHeight))
        {
            ErrorText.text = "The size is too small!\nBoth width and height must be more than or equal to 3.";
            return;
        }
        if (Grid.IsMineCountIncorrect(gridWidth, gridHeight, gridMineCount))
        {
            ErrorText.text = "Mine count is incorrect!\nMine count should be more than 1 or less than the width times the height minus 9.";
            return;
        }
        ErrorMessage.SetActive(false);
        
        // Save data
        PlayerPrefs.SetInt("width", gridWidth);
        PlayerPrefs.SetInt("height", gridHeight);
        PlayerPrefs.SetInt("mineCount", gridMineCount);
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Start a game with custom data.
    /// </summary>
    /// <param name="width">Width of the grid</param>
    /// <param name="height">Height of the grid</param>
    /// <param name="mineCount">Number of mines</param>
    public void StartGame(int width, int height, int mineCount)
    {
        // Save data
        gridWidth = width;
        gridHeight = height;
        gridMineCount = mineCount;
        
        StartGame();
    }

    public void StartBeginnerGame() => StartGame(9, 10, 10);
    public void StartIntermediateGame() => StartGame(16, 16, 40);
    public void StartExpertGame() => StartGame(29, 16, 99);

    public void ChangeGameMode(int value) => PlayerPrefs.SetInt("gameMode", value);

    // Parse fields

    public void ParseWidthField(string value) => int.TryParse(value, out gridWidth);
    public void ParseHeightField(string value) => int.TryParse(value, out gridHeight);
    public void ParseMineCountField(string value) => int.TryParse(value, out gridMineCount);

    // Settings

    /// <summary>
    /// Reset all best times.
    /// </summary>
    public void ResetBestTimes()
    {
        float volume = PlayerPrefs.GetFloat("volume", 1f);
        float sensibility = PlayerPrefs.GetFloat("sensibility", 16f);
        int fullscreen = PlayerPrefs.GetInt("fullscreen", 1);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("sensibility", sensibility);
        PlayerPrefs.SetInt("fullscreen", fullscreen);
        audioSource.PlayClick1();
    }

    /// <summary>
    /// Change the volume of the audio source.
    /// Update the volume text in the settings menu.
    /// </summary>
    /// <param name="value"></param>
    public void ChangeVolume(float value)
    {
        NumberVolume.GetComponent<TextMeshProUGUI>().text = (value * 200).ToString("0");
        audioSource.ChangeVolume(value);
    }

    /// <summary>
    /// Change the sensibility of the camera.
    /// Update the sensibility text in the settings menu.
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSensibility(float value) 
    {
        NumberSensibility.GetComponent<TextMeshProUGUI>().text = value.ToString("0.00");
        PlayerPrefs.SetFloat("sensibility", value);
    }

    public void SetFullscreen(bool value)
    {
        if (value)
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        else
            Screen.SetResolution(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, false);
        PlayerPrefs.SetInt("fullscreen", value ? 1 : 0);
    }
}
