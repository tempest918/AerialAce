using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video; 
using TMPro;

public class Secret : MonoBehaviour
{
    public VideoPlayer videoPlayer;  
    public RawImage rawImage; 

    void Start()
    {
        // Prepare the video 
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, "EasterEgg.mp4"); 
        videoPlayer.url = videoPath;
        videoPlayer.Prepare();

        // Assign the video output to the Raw Image
        videoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 0);
        rawImage.texture = videoPlayer.targetTexture; 
    }

    public void returnToMainMenu()
    {
        SceneManager.LoadScene("TitleScreen");

        // Optional: Stop the video before switching scenes
        if (videoPlayer.isPlaying) 
        {
            videoPlayer.Stop();
        }
    }

    public void PlayVideo() 
    {
        videoPlayer.Play();
    }
}
