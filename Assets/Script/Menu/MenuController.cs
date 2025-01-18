using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class MenuController: MonoBehaviour
{
    [Header("Volume Settings")] 
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 100f;
    [SerializeField] private AudioSource audioSource = null;
    
    [Header("GamePlay Settings")]
    [SerializeField] private TMP_Text controllerSensTextValue = null;
    [SerializeField] private Slider controllerSensSlider = null;
    [SerializeField] private float defaultSens = 4f;
    public float mainControllerSens = 4f;
    
    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertYToggle = null;
    
    [Header("Graphics Settings")]
    
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TMP_Text brightnessValueText;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    private Resolution[] resolutions;
    
    private int _qualityLevel;
    private bool _isFullScreen;
    public Button mybuttton;
    
    [Header("Confirmation")]
    [SerializeField] public GameObject confirmationPrompt = null;
    
    [Header("Levels To Load")] 
    public string _newGameLevel;
    private string levelToLoad;
    //private bool isclick;

    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }
    
    //
    private void Start()
    {
        // Initialiser les résolutions disponibles
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        //StartCoroutine(LoadAsyncScene());


        // Ajouter les options au Dropdown de résolution
        foreach (Resolution res in resolutions)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{res.width}x{res.height}"));
        }

        // Ajouter des callbacks aux UI Elements
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);

        // Initialiser les paramètres par défaut
        InitializeSettings();
    }
    
    void InitializeSettings()
    {
        // Luminosité par défaut
        brightnessSlider.value = 4; // Exemple
        brightnessValueText.text = brightnessSlider.value.ToString();

        // Mode plein écran
        fullscreenToggle.isOn = Screen.fullScreen;
        Screen.SetResolution(1920, 1080, true);
        int currentResolutionIndex = System.Array.FindIndex(resolutions, r => r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height);
        resolutionDropdown.value = currentResolutionIndex != -1 ? currentResolutionIndex : 0;
        
        qualityDropdown.value = QualitySettings.GetQualityLevel();
    }
    
    void OnBrightnessChanged(float value)
    {
        brightnessValueText.text = value.ToString("0.0");
        RenderSettings.ambientLight = Color.white * (value / 10);
    }

    void OnResolutionChanged(int index)
    {
        Resolution selectedResolution = resolutions[index];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
    }

    void OnFullscreenToggle(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    void OnQualityChanged(int index)
    {
        // Appliquer le niveau de qualité sélectionné
        QualitySettings.SetQualityLevel(index);

        // Vous pouvez aussi vérifier si le changement a bien pris effet en affichant un message
        Debug.Log("Niveau de qualité changé en : " + QualitySettings.names[index]);
    }

    // Boutons : Appliquer et Réinitialiser
    public void ApplySettings()
    {
        StartCoroutine(ConfirmationBox());
        // Sauvegarde des paramètres dans PlayerPrefs si nécessaire
    }

    public void ResetToDefault()
    {
        //Debug.Log("Réinitialisation des paramètres !");
        InitializeSettings();
    }
    //

    public void ExitButton()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        AudioListener.volume = volume;
        float new_volume = volume * 100f;
        volumeTextValue.text = new_volume.ToString("0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }
    
    public void SetControllerSen()
    {
        mainControllerSens = controllerSensSlider.value;
        controllerSensTextValue.text = mainControllerSens.ToString("0");
    }

    public void GamePlayApply()
    {
        if (invertYToggle.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
        }
        
        PlayerPrefs.SetFloat("masterSens", mainControllerSens);
        StartCoroutine(ConfirmationBox());

    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            controllerSensSlider.value = defaultSens;
            mainControllerSens = defaultSens;
            controllerSensTextValue.text = defaultSens.ToString("0");
            invertYToggle.isOn = false;
            GamePlayApply();
        }
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}