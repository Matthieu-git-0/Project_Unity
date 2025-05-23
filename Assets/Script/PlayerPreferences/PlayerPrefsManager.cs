using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{


    public void PlayerpreferencesAddFloat(string key, float input)
    {
        PlayerPrefs.SetFloat(key, input);
        PlayerPrefs.Save();
    }
    public void PlayerpreferencesAddInt(string key, int input)
    {
        PlayerPrefs.SetInt(key, input);
        PlayerPrefs.Save();
    }
    public void PlayerpreferencesAddString(string key, string input)
    {
        PlayerPrefs.SetString(key, input);
        PlayerPrefs.Save();
    }

    public void PlayerpreferencesSuppKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }

    public void PlayerpreferencesModifyStringKey(string key, string input)
    {
        string tmp = PlayerPrefs.GetString(key);
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.SetString(key, input);
        PlayerPrefs.Save();
    }
    
    public void PlayerpreferencesModifyFloatKey(string key, float input)
    {
        float tmp = PlayerPrefs.GetFloat(key);
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.SetFloat(key, input);
        PlayerPrefs.Save();
    }
    
    public void PlayerpreferencesModifyIntKey(string key, int input)
    {
        int tmp = PlayerPrefs.GetInt(key);
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.SetInt(key, input);
        PlayerPrefs.Save();
    }

    public bool PlayerpreferencesHasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    
    
    
}
