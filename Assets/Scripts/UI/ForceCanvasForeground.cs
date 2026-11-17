using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
public class ForceCanvasForeground : MonoBehaviour
{
    private Canvas canvas;
    public int sortingOrder = 999;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        Foreground();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        Foreground();
    }

    public void Foreground()
    {
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortingOrder;
        }
    }
}
