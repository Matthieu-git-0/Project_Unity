using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public Slider loadingSlider;
    public TMP_Text loadingText;
    public float simulatedLoadDuration = 5f; // Temps en secondes pour simuler un chargement plus long

    void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        // Commence le chargement de la scène en arrière-plan
        AsyncOperation operation = SceneManager.LoadSceneAsync("Map");
        operation.allowSceneActivation = false;

        float simulatedProgress = 0f; // Progression simulée pour la barre de chargement
        float elapsedTime = 0f;

        while(!operation.isDone)
        {
            // Calculer le pourcentage réel de progression de la scène et le combiner avec le chargement simulé
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);
            elapsedTime += Time.deltaTime;
            simulatedProgress = Mathf.Clamp01(elapsedTime / simulatedLoadDuration);

            // Utiliser la progression simulée tant qu'elle est inférieure à la progression réelle
            float displayProgress = Mathf.Min(realProgress, simulatedProgress);
            loadingSlider.value = displayProgress;
            loadingText.text = "Chargement : " + (displayProgress * 100f).ToString("F0") + "%";

            // Lorsque le chargement est complet et que la progression simulée est à 100%
            if (realProgress >= 1f && simulatedProgress >= 1f)
            {
                loadingText.text = "Appuyez pour commencer !";
                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
