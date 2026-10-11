using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject winMenu;
    private bool isWin = false;

    void Start()
    {
        UiEventSystemBootstrap.Ensure();
        Time.timeScale = 1f; 

        isWin = PlayerPrefs.GetInt("IsWin", 0) == 1;
        
        Debug.Log("EndScene - IsWin: " + isWin); // Debug 

        if (isWin)
        {
            ShowWinMenu();
        }
        else
        {
            ShowGameOverMenu();
        }
    }

    private void ShowGameOverMenu()
    {
        Debug.Log("Hiển thị Game Over Menu"); // Debug
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
            Debug.Log("Game Over Menu đã được bật");
        }
        else
        {
            Debug.LogError("gameOverMenu NULL - Chưa kéo vào Inspector!");
        }
        
        if (winMenu != null)
        {
            winMenu.SetActive(false);
        }
    }

    private void ShowWinMenu()
    {
        Debug.Log("Hiển thị Win Menu"); // Debug
        if (winMenu != null)
        {
            winMenu.SetActive(true);
            Debug.Log("Win Menu đã được bật");
        }
        else
        {
            Debug.LogError("winMenu NULL - Chưa kéo vào Inspector!");
        }
        
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false);
        }
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame được gọi!"); // Debug 
        SceneManager.LoadScene("Gameplay");
    }


    public void BackToMainMenu()
    {
        Debug.Log("BackToMainMenu được gọi!"); // Debug 
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
