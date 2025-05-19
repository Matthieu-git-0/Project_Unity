using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light flickerLight; // Drag & drop your light here
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 0.1f; // Temps entre chaque "sursaut"
    public bool useRandomFlicker = true;

    private float timer;

    void Start()
    {
        if (flickerLight == null)
        {
            flickerLight = GetComponent<Light>();
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = flickerSpeed;

            if (useRandomFlicker)
            {
                flickerLight.intensity = Random.Range(minIntensity, maxIntensity);
                flickerLight.enabled = (Random.value > 0.1f); // parfois elle s’éteint
            }
            else
            {
                flickerLight.intensity = (flickerLight.intensity == minIntensity) ? maxIntensity : minIntensity;
            }
        }
    }
}
