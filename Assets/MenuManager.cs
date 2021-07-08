using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    BasicMovementScript playerScript;
    [SerializeField]
    MenuState menuState;
    [SerializeField]
    MenuState previousMenu;
    [SerializeField]
    EventSystem eventSystem;
    [SerializeField]
    bool hasBeenPaused;

    [Header("In-Game")]
    [SerializeField] CanvasGroup gameCanvasGroup;

    [Header("Quality Settings")]
    [SerializeField]
    TextMeshProUGUI qualityText;
    [SerializeField]
    string[] qualitySettingNames;

    [Header("Menu References")]
    [SerializeField]
    TextMeshProUGUI startGameText;
    [SerializeField]
    TextMeshProUGUI exitGameText;
    [SerializeField]
    internal Slider[] sfxSlider;

    [Header("Canvas Groups")]
    [SerializeField]
    CanvasGroup loadingGroup;
    [SerializeField]
    CanvasGroup audioGroup;
    [SerializeField]
    CanvasGroup settingsGroup;
    [SerializeField]
    CanvasGroup menuGroup;
    [SerializeField]
    CanvasGroup tutorialGroup;

    [SerializeField]
    CanvasGroup controlsGroup;



    public enum MenuState
    {
        Playing,
        PauseMenu,
        MainMenu,
        GraphicsSettings,
        AudioSettings,
        Tutorial,
        Controls,
        GameOver
    }

    // Start is called before the first frame update
    void Awake()
    {
        qualitySettingNames = QualitySettings.names;
        if (PlayerPrefs.GetInt("TimesPlayedInt") == 0)
        {
            SetGraphicsOption(2);
            PlayerPrefs.GetInt("TimesPlayedInt", 1);
        }


        SetGraphicsOption(PlayerPrefs.GetInt("QualityInt"));


        SetGamestateEnum("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPreviousMenuType()
    {
        if (hasBeenPaused)
        {
            previousMenu = MenuState.PauseMenu;
        }

        else
        {
            previousMenu = MenuState.MainMenu;
        }
    }

    public void ChangeMenuCanvas(MenuState wantedGameState)
    {
        switch (wantedGameState)
        {
            case MenuState.Playing:
                menuGroup.alpha = 0;
                menuGroup.interactable = false;
                menuGroup.blocksRaycasts = false;
                gameCanvasGroup.alpha = 1;
                gameCanvasGroup.interactable = false;
                gameCanvasGroup.blocksRaycasts = false;
                settingsGroup.interactable = false;
                settingsGroup.alpha = 0;               
                settingsGroup.blocksRaycasts = false;
                audioGroup.alpha = 0;
                audioGroup.interactable = false;
                audioGroup.blocksRaycasts = false;
                tutorialGroup.alpha = 0;
                tutorialGroup.interactable = false;
                tutorialGroup.blocksRaycasts = false;
                controlsGroup.alpha = 0;
                controlsGroup.interactable = false;
                controlsGroup.blocksRaycasts = false;
                break;
            case MenuState.PauseMenu:
                menuGroup.alpha = 1;
                menuGroup.interactable = true;
                menuGroup.blocksRaycasts = true;
                gameCanvasGroup.alpha = 0;
                gameCanvasGroup.interactable = false;
                gameCanvasGroup.blocksRaycasts = false;
                settingsGroup.interactable = false;
                settingsGroup.alpha = 1;
                settingsGroup.blocksRaycasts = false;
                startGameText.text = "Resume Stream";
                audioGroup.alpha = 1;
                audioGroup.interactable = false;
                audioGroup.blocksRaycasts = false;
                tutorialGroup.alpha = 0;
                tutorialGroup.interactable = false;
                tutorialGroup.blocksRaycasts = false;
                controlsGroup.alpha = 0;
                controlsGroup.interactable = false;
                controlsGroup.blocksRaycasts = false;
                exitGameText.text = "Reset Stream";
                break;
            case MenuState.MainMenu:
                menuGroup.alpha = 1;
                menuGroup.interactable = true;
                menuGroup.blocksRaycasts = true;
                gameCanvasGroup.alpha = 0;
                gameCanvasGroup.interactable = false;
                gameCanvasGroup.blocksRaycasts = false;
                settingsGroup.interactable = false;
                settingsGroup.alpha = 1;
                settingsGroup.blocksRaycasts = false;
                startGameText.text = "Start Stream";
                audioGroup.alpha = 1;
                audioGroup.interactable = false;
                audioGroup.blocksRaycasts = false;
                tutorialGroup.alpha = 0;
                tutorialGroup.interactable = false;
                tutorialGroup.blocksRaycasts = false;
                controlsGroup.alpha = 0;
                controlsGroup.interactable = false;
                controlsGroup.blocksRaycasts = false;
                exitGameText.text = "Exit";
                break;
            case MenuState.GraphicsSettings:
                menuGroup.alpha = 1;
                menuGroup.interactable = false;
                menuGroup.blocksRaycasts = false;
                gameCanvasGroup.alpha = 0;
                gameCanvasGroup.interactable = false;
                gameCanvasGroup.blocksRaycasts = false;
                settingsGroup.interactable = true;
                settingsGroup.alpha = 1;
                settingsGroup.blocksRaycasts = true;
                audioGroup.alpha = 1;
                audioGroup.interactable = false;
                audioGroup.blocksRaycasts = false;
                tutorialGroup.alpha = 0;
                tutorialGroup.interactable = false;
                tutorialGroup.blocksRaycasts = false;
                controlsGroup.alpha = 0;
                controlsGroup.interactable = false;
                controlsGroup.blocksRaycasts = false;
                break;
            case MenuState.AudioSettings:
                menuGroup.alpha = 1;
                menuGroup.interactable = false;
                menuGroup.blocksRaycasts = false;
                gameCanvasGroup.alpha = 0;
                gameCanvasGroup.interactable = false;
                gameCanvasGroup.blocksRaycasts = false;
                settingsGroup.interactable = false;
                settingsGroup.alpha = 1;
                settingsGroup.blocksRaycasts = false;
                audioGroup.alpha = 1;
                audioGroup.interactable = true;
                audioGroup.blocksRaycasts = true;
                tutorialGroup.alpha = 0;
                tutorialGroup.interactable = false;
                tutorialGroup.blocksRaycasts = false;
                controlsGroup.alpha = 0;
                controlsGroup.interactable = false;
                controlsGroup.blocksRaycasts = false;
                break;
            case MenuState.Tutorial:
                menuGroup.alpha = 0;
                menuGroup.interactable = false;
                menuGroup.blocksRaycasts = false;
                gameCanvasGroup.alpha = 0;
                gameCanvasGroup.interactable = false;
                gameCanvasGroup.blocksRaycasts = false;
                settingsGroup.interactable = false;
                settingsGroup.alpha = 0;
                settingsGroup.blocksRaycasts = false;
                audioGroup.alpha = 0;
                audioGroup.interactable = false;
                audioGroup.blocksRaycasts = false;
                tutorialGroup.alpha = 1;
                tutorialGroup.interactable = true;
                tutorialGroup.blocksRaycasts = true;
                controlsGroup.alpha = 0;
                controlsGroup.interactable = false;
                controlsGroup.blocksRaycasts = false;
                break;
            case MenuState.Controls:
                menuGroup.alpha = 0;
                menuGroup.interactable = false;
                menuGroup.blocksRaycasts = false;
                gameCanvasGroup.alpha = 0;
                gameCanvasGroup.interactable = false;
                gameCanvasGroup.blocksRaycasts = false;
                settingsGroup.interactable = false;
                settingsGroup.alpha = 0;
                settingsGroup.blocksRaycasts = false;
                audioGroup.alpha = 0;
                audioGroup.interactable = false;
                audioGroup.blocksRaycasts = false;
                tutorialGroup.alpha = 0;
                tutorialGroup.interactable = false;
                tutorialGroup.blocksRaycasts = false;
                controlsGroup.alpha = 1;
                controlsGroup.interactable = true;
                controlsGroup.blocksRaycasts = true;
                break;
            case MenuState.GameOver:
                menuGroup.alpha = 0;
                menuGroup.interactable = false;
                menuGroup.blocksRaycasts = false;
                gameCanvasGroup.alpha = 0;
                gameCanvasGroup.interactable = false;
                gameCanvasGroup.blocksRaycasts = false;
                settingsGroup.interactable = false;
                settingsGroup.alpha = 0;
                settingsGroup.blocksRaycasts = false;
                audioGroup.alpha = 0;
                audioGroup.interactable = false;
                audioGroup.blocksRaycasts = false;
                tutorialGroup.alpha = 0;
                tutorialGroup.interactable = false;
                tutorialGroup.blocksRaycasts = false;
                controlsGroup.alpha = 0;
                controlsGroup.interactable = false;
                controlsGroup.blocksRaycasts = false;
                break;
            default:
                break;
        }
    }

    public void GameStateSwitch()
    {
        switch (menuState)
        {
            case MenuState.Playing:
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case MenuState.PauseMenu:
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                hasBeenPaused = true;
                break;
            case MenuState.MainMenu:
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case MenuState.GraphicsSettings:
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                break;
            case MenuState.AudioSettings:
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                break;
            case MenuState.Tutorial:
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case MenuState.Controls:
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case MenuState.GameOver:
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                hasBeenPaused = true;
                break;
            default:
                break;
        }


        ChangeMenuCanvas(menuState);
    }

    void SetFirstSelected(GameObject desiredGameObject)
    {
        if (desiredGameObject != null)
        {
            eventSystem.SetSelectedGameObject(desiredGameObject);
        }

        else
        {
            eventSystem.SetSelectedGameObject(null);
        }
    }

    public void SetGraphicsOption(int selectedQualityInt)
    {
        QualitySettings.SetQualityLevel(selectedQualityInt, true);
        qualityText.text = qualitySettingNames[selectedQualityInt];
        if (PlayerPrefs.GetInt("QualityInt") != selectedQualityInt)
        {
            PlayerPrefs.SetInt("QualityInt", selectedQualityInt);
        }

    }

    public void SetGamestateEnum(string enumString)
    {
        PlayerPrefs.SetFloat("sfx", sfxSlider[0].value);
        PlayerPrefs.SetFloat("music", sfxSlider[1].value);

        menuState = (MenuState)System.Enum.Parse(typeof(MenuState), enumString);//THIS IS WHERE THE MENU CHANGE INITIALLY STARTS
        GameStateSwitch();
    }

    public void Unpause()
    {
        PlayerPrefs.SetFloat("sfx", sfxSlider[0].value);
        PlayerPrefs.SetFloat("music", sfxSlider[1].value);

        menuState = MenuState.Playing;
        GameStateSwitch();
    }

    public void ReturnMenu()
    {
        menuState = previousMenu;
       GameStateSwitch();
    }

    public void InputPause(InputAction.CallbackContext context)
    {
        if (context.performed && Time.timeScale != 0)
        {
            SetGamestateEnum("PauseMenu");
        }

        else if (context.performed && menuState == MenuState.PauseMenu)
        {
            Unpause();
        }

        else if (context.performed && menuState == MenuState.GameOver)
        {
            Debug.Log("Nothing happened, the games over...");
        }

        else if (context.performed && menuState != MenuState.MainMenu || context.performed && menuState != MenuState.PauseMenu)
        {
            SetGamestateEnum(previousMenu.ToString());
        }


    }

    public void InvertY(Toggle toggle)
    {
        playerScript._firstPersonControl.yAxis.invertY = toggle.isOn;
    }

    public void ShowCanvasGroup(CanvasGroup desiredGroup)
    {
        desiredGroup.alpha = 1;
    }



    public void SetPreviousMenu(string enumString)
    {
        previousMenu = (MenuState)System.Enum.Parse(typeof(MenuState), enumString);
    }

    public void QuitGame()
    {
        if (menuState == MenuState.MainMenu)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        else if(menuState == MenuState.PauseMenu)
        {
            loadingGroup.alpha = 1;
            loadingGroup.blocksRaycasts = true;
            loadingGroup.interactable = true;
            SceneManager.LoadScene(0);
        }


    }
}
