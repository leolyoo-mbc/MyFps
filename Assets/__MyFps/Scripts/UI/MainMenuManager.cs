using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void NewGame()
    {
        Debug.Log("NEW GAME");
        // SceneManager.LoadScene("PlayScene01");
    }

    public void LoadGame()
    {
        Debug.Log("LOAD GAME");
    }

    public void Options()
    {
        Debug.Log("OPTIONS");
    }

    public void Credits()
    {
        Debug.Log("CREDITS");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
