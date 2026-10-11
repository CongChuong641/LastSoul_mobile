using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;

    void Start()
    {
        UiEventSystemBootstrap.Ensure();
        Time.timeScale = 1f; 
        if (audioManager != null)
        {
            audioManager.StopAudioGame();
        }
    }

    public void StartGame()
    {
        Debug.Log("StartGame được gọi!"); 
        SceneManager.LoadScene("Gameplay");
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame được gọi!");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
