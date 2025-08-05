using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private int currentSequenceIndex = 0;
    private KeyCode[] konamiCode = new KeyCode[] {
    KeyCode.UpArrow, KeyCode.UpArrow,
    KeyCode.DownArrow, KeyCode.DownArrow,
    KeyCode.LeftArrow, KeyCode.RightArrow,
    KeyCode.LeftArrow, KeyCode.RightArrow,
    KeyCode.B, KeyCode.A
};
    private TextMeshProUGUI highScoreText;
    public void Start()
    {
        highScoreText = GameObject.Find("High Score Text").GetComponent<TextMeshProUGUI>();
        showHighScore();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(konamiCode[currentSequenceIndex]))
            {
                currentSequenceIndex++;
                if (currentSequenceIndex == konamiCode.Length)
                {
                    ActivateEasterEgg();
                    currentSequenceIndex = 0;
                }
            }
            else
            {
                currentSequenceIndex = 0;
            }
        }
    }

    void ActivateEasterEgg()
    {
        PlayerPrefs.SetInt("KonamiBonus", 1);
        SceneManager.LoadScene("SecretLevel");
        Debug.Log("Konami Code Activated!");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Quit();
    }

    public void showHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (highScore == 0)
        {
            highScoreText.text = "HIGH SCORE: 0";
        }
        else
        {
            highScoreText.text = "HIGH SCORE: " + highScore;
        }

    }

    public void Quit()
    {
        #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            Debug.Log(this.name+" : "+this.GetType()+" : "+System.Reflection.MethodBase.GetCurrentMethod().Name); 
        #endif
        #if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
        #elif (UNITY_STANDALONE)
            Application.Quit();
        #elif (UNITY_WEBGL)
            SceneManager.LoadScene("ExitScreen");
        #endif
    }
}
