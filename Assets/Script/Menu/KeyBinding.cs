using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class KeyBindingManager : MonoBehaviour
{

    [Header("PANEL")] public GameObject keyConfirmationPanel;

    [Header("KEY NAMES")] 
    public GameObject forwardtext;
    public GameObject backwardtext;
    public GameObject lefttext;
    public GameObject righttext;
    public GameObject jumptext;
    public GameObject sprinttext;
    public GameObject usetext;
    public GameObject interacttext;
    public GameObject pausetext;

    private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();
    private Dictionary<string, TMP_Text> keyBindingTexts = new Dictionary<string, TMP_Text>();

    private string currentKeyBinding;

    void Start()
    {
        keyBindingTexts["Forward"] = forwardtext.GetComponent<TMP_Text>();
        keyBindingTexts["Backward"] = backwardtext.GetComponent<TMP_Text>();
        keyBindingTexts["Left"] = lefttext.GetComponent<TMP_Text>();
        keyBindingTexts["Right"] = righttext.GetComponent<TMP_Text>();
        keyBindingTexts["Jump"] = jumptext.GetComponent<TMP_Text>();
        keyBindingTexts["Sprint"] = sprinttext.GetComponent<TMP_Text>();
        keyBindingTexts["Use"] = usetext.GetComponent<TMP_Text>();
        keyBindingTexts["Interact"] = interacttext.GetComponent<TMP_Text>();
        keyBindingTexts["Pause"] = pausetext.GetComponent<TMP_Text>();

        LoadKeyBindings();
    }
    
    /*void Update()
    {
        if (keyConfirmationPanel.activeSelf && currentKeyBinding != null)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    KeyCode correctedKey = ConvertQWERTYtoAZERTY(keyCode);

                    keyBindings[currentKeyBinding] = correctedKey;
                    SaveKeyBindings();
                    UpdateKeyBindingText(currentKeyBinding, correctedKey);
                    keyConfirmationPanel.SetActive(false);
                    currentKeyBinding = null;
                    break;
                }
            }
        }
    }*/


    void Update()
    {
        if (keyConfirmationPanel.activeSelf && currentKeyBinding != null)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    keyBindings[currentKeyBinding] = keyCode;
                    SaveKeyBindings();
                    UpdateKeyBindingText(currentKeyBinding, keyCode);
                    keyConfirmationPanel.SetActive(false);
                    currentKeyBinding = null;
                    break;
                }
            }
        }
    }

    public void OnKeyBindingButtonClicked(string keyBindingName)
    {
        currentKeyBinding = keyBindingName;
        keyConfirmationPanel.SetActive(true);
    }

    public void CancelButton()
    {
        keyConfirmationPanel.SetActive(false);
        currentKeyBinding = null;
    }

    private void SaveKeyBindings()
    {
        foreach (var binding in keyBindings)
        {
            PlayerPrefs.SetString(binding.Key, binding.Value.ToString());
        }

        PlayerPrefs.Save();
    }
    
    /*private KeyCode ConvertQWERTYtoAZERTY(KeyCode key)
    {
        Dictionary<KeyCode, KeyCode> qwertyToAzerty = new Dictionary<KeyCode, KeyCode>
        {
            { KeyCode.A, KeyCode.Q },
            { KeyCode.Z, KeyCode.W },
            { KeyCode.W, KeyCode.Z },
            { KeyCode.Q, KeyCode.A },
        };
    
        return qwertyToAzerty.ContainsKey(key) ? qwertyToAzerty[key] : key;
    }*/


    private void LoadKeyBindings()
    {
        Dictionary<string, KeyCode> defaultBindings = new Dictionary<string, KeyCode>
        {
            { "Forward", KeyCode.Z },
            { "Backward", KeyCode.S },
            { "Left", KeyCode.Q },
            { "Right", KeyCode.D },
            { "Jump", KeyCode.Space },
            { "Sprint", KeyCode.LeftShift },
            { "Use", KeyCode.A },
            { "Interact", KeyCode.E },
            { "Pause", KeyCode.Escape }
        };

        foreach (var action in defaultBindings.Keys)
        {
            string savedKey = PlayerPrefs.GetString(action, "None");
            if (System.Enum.TryParse(savedKey, out KeyCode keyCode) && keyCode != KeyCode.None)
            {
                keyBindings[action] = keyCode;
            }
            else
            {
                keyBindings[action] = defaultBindings[action];
                PlayerPrefs.SetString(action, defaultBindings[action].ToString());
            }

            UpdateKeyBindingText(action, keyBindings[action]);
        }

        PlayerPrefs.Save();
    }


    private void UpdateKeyBindingText(string action, KeyCode keyCode)
    {
        if (keyBindingTexts.ContainsKey(action))
        {
            keyBindingTexts[action].text = keyCode.ToString();
        }
    }
}
