using TMPro;
using Unity.VisualScripting;
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
    public GameObject CustomGameMenu;

    [Header("Input fields")]
    public GameObject WidthField;
    public GameObject HeightField;
    public GameObject MineCountField;

    [Header("Settings Menu")]
    public GameObject NumberVolume;
    public GameObject SliderVolume;

    private int gridWidth;
    private int gridHeight;
    private int gridMineCount;

    private void Awake()
    {
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

        // Settings
        float volume = PlayerPrefs.GetFloat("volume", 1f);
        ChangeVolume(volume);
        SliderVolume.GetComponent<Slider>().value = volume; 

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

    // Settings

    public void ResetBestTimes()
    {
        float volume = PlayerPrefs.GetFloat("volume", 1f);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("volume", volume);
        audioSource.PlayClick1();
    }

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
            ErrorText.text = "The size is too small!\nBoth width and height must be more than 0\nand one of them must be more than 3.";
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

    // Parse fields

    public void ParseWidthField(string value) => int.TryParse(value, out gridWidth);
    public void ParseHeightField(string value) => int.TryParse(value, out gridHeight);
    public void ParseMineCountField(string value) => int.TryParse(value, out gridMineCount);

    public void ChangeVolume(float value)
    {
        NumberVolume.GetComponent<TextMeshProUGUI>().text = (value * 100).ToString("0");
        audioSource.ChangeVolume(value);
    }

}
