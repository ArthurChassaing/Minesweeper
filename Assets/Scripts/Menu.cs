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
   
}
