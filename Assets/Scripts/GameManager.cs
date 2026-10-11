using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int currentEnergy;
    [SerializeField] private int energyThreshold = 5;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject enemySpaner;
    private bool bossCalled = false;
    [SerializeField] private Image energyBar;
    [SerializeField] GameObject gameUi;
    [SerializeField] GameObject red;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private bool showMobileControlsInEditor = true;
   
    void Start()
    {
        SetupMobileCamera();
        currentEnergy = 0;
        UpdateEnergybar();
        boss.SetActive(false);
        Time.timeScale = 1f; // Bắt đầu game bình thường
        audioManager.PlayDefaultAudio();
        red.SetActive(false);

        if (Application.isMobilePlatform || ShouldShowMobileControlsInEditor())
        {
            MobileHUDBuilder.Build(this);
        }
    }

    private bool ShouldShowMobileControlsInEditor()
    {
#if UNITY_EDITOR
        return showMobileControlsInEditor;
#else
        return false;
#endif
    }

    private void SetupMobileCamera()
    {
        if (!Application.isMobilePlatform && !ShouldShowMobileControlsInEditor())
        {
            return;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.GetComponent<MobileCameraFraming>() == null)
        {
            mainCamera.gameObject.AddComponent<MobileCameraFraming>();
        }
    }



    public void AddEnergy()
    {
        if (bossCalled)
        {
            return;
        }
        currentEnergy += 1;
        UpdateEnergybar();
        if (currentEnergy == energyThreshold)
        {
            CallBoss();
        }
    }
    private void CallBoss()
    {
        if (bossCalled || currentEnergy < energyThreshold)
        {
            return;
        }

        bossCalled = true;
        boss.SetActive(true);
        enemySpaner.SetActive(false); //Ngung sinh enemy
        gameUi.SetActive(false);
        audioManager.PlayBossAudio();
        red.SetActive(true);
    }
    private void UpdateEnergybar()
    {
        if (energyBar != null)
        {
            float fillAmount = Mathf.Clamp01((float)currentEnergy / (float)energyThreshold);
            energyBar.fillAmount = fillAmount;
        }
    }
    // Hiển thị pause menu
    public void PauseGameMenu()
    {
        MobileHUDBuilder.SetInteractable(false);
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        MobileHUDBuilder.SetInteractable(true);
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        PlayerPrefs.SetInt("IsWin", 0); // 0 = Game Over
        PlayerPrefs.Save();
        SceneManager.LoadScene("Endscene");
    }

    public void WinGame()
    {
        PlayerPrefs.SetInt("IsWin", 1); // 1 = Win
        PlayerPrefs.Save();
        SceneManager.LoadScene("Endscene");
    }
}
