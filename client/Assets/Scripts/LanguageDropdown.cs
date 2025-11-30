using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LanguageDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    // map codes → full names
    private Dictionary<string, string> languages = new Dictionary<string, string>
    {
        { "es", "Spanish" },
        { "ar", "Arabic" },
        { "fr", "French" },
        { "zh", "Chinese" },
        { "he", "Hebrew" },
        { "de", "German" },
        { "ja", "Japanese" },
        { "ko", "Korean" }
    };

    private List<string> codes;   // keeps codes in dropdown order

    void Start()
    {
        dropdown.ClearOptions();

        // track the order so we know which code matches which dropdown index
        codes = new List<string>(languages.Keys);

        // add the display names in the same order
        dropdown.AddOptions(new List<string>(languages.Values));

        // listen for selection
        dropdown.onValueChanged.AddListener(OnLanguageSelected);
    }

    private void OnLanguageSelected(int index)
    {
        string code = codes[index];
        GameSettings.CurrentLanguageCode = code;
        Debug.Log("Language set to: " + code);
    }
}
