using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private static DontDestroy dd;

    private void Awake()
    {
        dd = FindAnyObjectByType<DontDestroy>();
        dd.width = 20;
        dd.height = 20;
        dd.mineCount = 80;
        PlaceCamera();
    }

    private void PlaceCamera() => Camera.main.Reset();

    public void StartGame(int width, int height, int mineCount)
    {
        // Give data to DontDestroy
        dd.width = width;
        dd.height = height;
        dd.mineCount = mineCount;
        SceneManager.LoadScene(1);
    }

    public void StartGame() => SceneManager.LoadScene(1);

    public void QuitGame()
    {
        Application.Quit();
    }


    //Sous menu 

    public void StartBeginnerGame() => StartGame(9,10,10);

    public void StartIntermediateGame() => StartGame(16,16,40);

    public void StartExpertGame() => StartGame(29, 16, 99);

    // custom game

    public void GetWidthField(string value) => int.TryParse(value, out dd.width);
    public void GetHeightField(string value) => int.TryParse(value, out dd.height);
    public void GetMineCountField(string value) => int.TryParse(value, out dd.mineCount);

}
