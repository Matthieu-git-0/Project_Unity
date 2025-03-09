using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public Slider loadingSlider;
    public TMP_Text loadingText;
    public float simulatedLoadDuration = 5f;

    void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Map");
        operation.allowSceneActivation = false;

        float simulatedProgress = 0f;
        float elapsedTime = 0f;

        while(!operation.isDone)
        {
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);
            elapsedTime += Time.deltaTime;
            simulatedProgress = Mathf.Clamp01(elapsedTime / simulatedLoadDuration);
            
            float displayProgress = Mathf.Min(realProgress, simulatedProgress);
            loadingSlider.value = displayProgress;
            loadingText.text = "Loading : " + (displayProgress * 100f).ToString("F0") + "%";
            
            if (realProgress >= 1f && simulatedProgress >= 1f)
            {
                loadingText.text = "Tap to start!";
                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
        
        if (Input.anyKeyDown)
        {
            operation.allowSceneActivation = true;
        }
    }
}
