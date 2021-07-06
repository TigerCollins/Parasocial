using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("In-Game")]
    [SerializeField] CanvasGroup GameCanvasGroup;

    [Header("Quality Settings")]
    [SerializeField]
    TextMeshProUGUI qualityText;
    [SerializeField]
    string[] qualitySettingNames;

    [Header("Menu References")]
    [SerializeField]
    TextMeshProUGUI startGameText;
    [SerializeField]
    internal Slider[] musicSlider;
    [SerializeField]
    internal Slider[] sfxSlider;

    [Header("Canvas Groups")]
    [SerializeField]
    CanvasGroup audioGroup;
      [SerializeField]
    CanvasGroup settingsGroup;
      [SerializeField]
    CanvasGroup menuGroup;



    public enum MenuState
    {
        Playing,
        PauseMenu,
        MainMenu,
        GraphicsSettings,
        AudioSettings,
        Tutorial,
        Controls
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
