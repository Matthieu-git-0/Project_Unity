using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnTrigger : MonoBehaviour
{
    public string sceneName = "NomDeTaScene";
    public string requiredTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(requiredTag))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
