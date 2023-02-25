using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public int mineCount = 80;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
