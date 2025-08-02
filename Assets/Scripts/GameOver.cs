using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI bestScore;

    public void OnRestart()
    {
        SceneManager.LoadScene("BallGame");
    }

    public void OnContinue()
    {
        // There we will save some max balls
        SceneManager.LoadScene("BallGame");
    }

    private void Awake()
    {
        if (currentScore != null)
        {
            currentScore.text = PlayerPrefs.GetInt("CurrentScore", 0).ToString();
        }

        if (bestScore != null)
        {
            bestScore.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        }
    }
}