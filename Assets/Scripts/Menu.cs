using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private static DontDestroy dd;
    [Header("Component")]
    public AudioSource audio;



    private void Awake()
    {
        dd = new GameObject("Don't Destroy: Grid Data", typeof(DontDestroy)).GetComponent<DontDestroy>();
        dd.width = 20;
        dd.height = 20;
        dd.mineCount = 80;
        PlaceCamera();
    }

    private void PlaceCamera() => Camera.main.Reset();

    public void StartGame(int width, int height, int mineCount)
    {
        // Make sure data is correct
        if (Grid.IsSizeTooSmall(width, height)) return;
        if (Grid.IsMineCountIncorrect(width, height, mineCount)) return;

        // Give data to DontDestroy
        dd.width = width;
        dd.height = height;
        dd.mineCount = mineCount;
        SceneManager.LoadScene(1);
    }

    public void PlaySound()
    {
        audio.Play();
    }

    public void StartGame()
    {
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

    // custom game

    public void GetWidthField(string value) => int.TryParse(value, out dd.width);
    public void GetHeightField(string value) => int.TryParse(value, out dd.height);
    public void GetMineCountField(string value) => int.TryParse(value, out dd.mineCount);

}
