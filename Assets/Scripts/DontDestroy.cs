using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public int width;
    public int height;
    public int mineCount;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
