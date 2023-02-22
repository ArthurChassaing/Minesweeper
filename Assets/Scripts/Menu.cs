using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void Awake()
    {
        PlaceCamera();
    }

    private void PlaceCamera() => Camera.main.Reset();
    public void StartGame() => StartGame(10, 10, 10);

    public void StartGame(int width, int height, int mineCount)
    {
        // Give data to DontDestroy
        DontDestroy dd = FindAnyObjectByType<DontDestroy>();
        dd.width = width;
        dd.height = height;
        dd.mineCount = mineCount;
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    //Sous menu 

    public void StartBeginnerGame() => StartGame(9,10,10);

    public void StartIntermediateGame() => StartGame(16,16,40);

    public void StartExpertGame() => StartGame(29, 16, 99);
}
