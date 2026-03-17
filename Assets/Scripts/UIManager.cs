using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject hitUI;
    public GameObject deathUI;
    public TextMeshProUGUI ammoText;
    public Image healthBar;
    public Gradient healthGradient;
    public GameObject winUI;
    public GameObject mainMenuPanel;
    public GameObject player;

    private void Awake()
    {
        Time.timeScale = 1f;
        Instance = this;
    }

    void Start()
    {
        deathUI.SetActive(false);
        winUI.SetActive(false);
        mainMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.GetComponent<PlayerShooting>().enabled = false;
        player.GetComponent<PlayerLook>().enabled = false; 
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.GetComponent<PlayerShooting>().enabled = true;
        player.GetComponent<PlayerLook>().enabled = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void InstantiateHitUI()
    {
        Instantiate(hitUI, transform);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void EnableDeathUI()
    {
        deathUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        player.GetComponent<PlayerShooting>().enabled = false; 
        player.GetComponent<PlayerLook>().enabled = false;    
    }

    public void SetHealthValue(int health)
    {
        float floatHealth = (float)health / 100f;
        healthBar.color = healthGradient.Evaluate(floatHealth);
        healthBar.fillAmount = floatHealth;
    }

    public void EnableWinUI()
    {
        winUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.GetComponent<PlayerShooting>().enabled = false; 
        player.GetComponent<PlayerLook>().enabled = false;     
    }
}