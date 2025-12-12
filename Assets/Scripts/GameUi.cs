using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUi : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public void ResumeGame()
    {
        Debug.Log("ResumeGame được gọi!");
        if (gameManager != null)
        {
            gameManager.ResumeGame();
        }
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame được gọi!"); 
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
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
