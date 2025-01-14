using UnityEngine;
using UnityEngine.Video; // Ajoute ceci pour VideoPlayer
using UnityEngine.SceneManagement;

public class VideoSceneSwitcher : MonoBehaviour
{
    public string nextSceneName; // Nom de la scène suivante
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd; // Événement pour détecter la fin de la vidéo
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName); // Charge la scène suivante
    }
}
