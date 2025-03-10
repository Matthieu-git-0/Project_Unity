using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoSceneSwitcher : MonoBehaviour
{
    public string nextSceneName;
    private VideoPlayer videoPlayer;
    
    IEnumerator Start()
    {
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(nextSceneName);
    }

    /*void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    /*void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }*/
}
