using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button starGame;
    [SerializeField] private Button setting;
    [SerializeField] private Button exitGame;
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private string sceneName;

    private void Start()
    {
        starGame.onClick.AddListener(StartGame);
        setting.onClick.AddListener(Setting);
        exitGame.onClick.AddListener(Exit);
        settingCanvas.gameObject.SetActive(false);
    }

    private void StartGame()
    {
        SceneManager.LoadScene(sceneName);
    }
    private void Setting()
    {
       
        settingCanvas.gameObject.SetActive(true);
       
    }

    private void Exit()
    {
        Application.Quit();
    }

   
}
