using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject playerInputs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Opens the settings panel
    public void OnOpen()
    {
        playerInputs.GetComponent<PressReleaseDetector>().Disable();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    // Closes the settings panel
    public void OnClose()
    {
        playerInputs.GetComponent<PressReleaseDetector>().Enable();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // Continues the game by closing the modal
    public void OnContinue()
    {
        OnClose();
    }

    // Reloads the BallGame scene
    public void OnReplay()
    {
        SceneManager.LoadScene("BallGame");
    }

    // Redirects to Google Play Store for rating the app
    public void OnRateApp()
    {
        Application.OpenURL("market://details?id=" + Application.identifier);
    }

    // Toggles the sound status in PlayerPrefs
    public void OnToggleSound()
    {
        bool currentSoundStatus = PlayerPrefs.GetInt("SoundStatus", 1) == 1;
        PlayerPrefs.SetInt("SoundStatus", currentSoundStatus ? 0 : 1);
        PlayerPrefs.Save();
    }

    // Toggles the background music status in PlayerPrefs
    public void OnToggleBgm()
    {
        bool currentBgmStatus = PlayerPrefs.GetInt("BGM", 1) == 1;
        PlayerPrefs.SetInt("BGM", currentBgmStatus ? 0 : 1);
        PlayerPrefs.Save();
    }
}
