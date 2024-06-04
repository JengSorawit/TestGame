using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class Setting : MonoBehaviour
    {
        public static Setting instance;

        [SerializeField] private Button soundButton;
        [SerializeField] private Button resolutionButton;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Slider volumeFXSlider;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Button applyButton; // Add this line
        [SerializeField] private string musicName;
        private List<Resolution> resolutions;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (!PlayerPrefs.HasKey("initialized"))
            {
                InitializeSettings();
                PlayerPrefs.SetInt("initialized", 1);
            }
            else
            {
                LoadSettings();
            }

            volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChange(); });
            volumeFXSlider.onValueChanged.AddListener(delegate { OnVolumeFXChange(); });
            musicToggle.onValueChanged.AddListener(delegate { OnMusicToggle(); });
            fullscreenToggle.onValueChanged.AddListener(delegate { SetFullscreen(fullscreenToggle.isOn); });
            resolutionDropdown.onValueChanged.AddListener(delegate { SetResolution(resolutionDropdown.value); });
            settingsPanel.gameObject.SetActive(false);

            applyButton.onClick.AddListener(ApplySettings);
        }

        private void LoadSettings()
        {
            volumeSlider.value = PlayerPrefs.GetFloat("volume", 0.4f);
            volumeFXSlider.value = PlayerPrefs.GetFloat("sfxvolume", 0.4f);
            musicToggle.isOn = PlayerPrefs.GetInt("musicOn", 1) == 1;
            fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;
            InitializeResolutionDropdown();
        }


        private void InitializeSettings()
        {
            volumeSlider.value = PlayerPrefs.GetFloat("volume", 0.4f);
            volumeFXSlider.value = PlayerPrefs.GetFloat("sfxvolume", 0.4f);
            musicToggle.isOn = PlayerPrefs.GetInt("musicOn", 1) == 1;
            fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;
            OnMusicToggle();
            InitializeResolutionDropdown();
        }

        private void OnVolumeChange()
        {
            AudioManager.instance.SetMusicVolume(volumeSlider.value);
        }

        private void OnVolumeFXChange()
        {
            AudioManager.instance.SetSFXVolume(volumeFXSlider.value);
        }

        private void OnMusicToggle()
        {
            if (musicToggle.isOn)
            {
                AudioManager.instance.PlayMusic(musicName);
            }
            else
            {
                AudioManager.instance.PauseMusic();
            }
        }

      

        private void InitializeResolutionDropdown()
        {
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            resolutions = new List<Resolution>();

            int currentResolutionIndex = 0;
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                Resolution resolution = Screen.resolutions[i];
                string option = resolution.width + " x " + resolution.height + " @";
                options.Add(option);
                resolutions.Add(resolution);

                if (resolution.width == Screen.currentResolution.width &&
                    resolution.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex", currentResolutionIndex);
            resolutionDropdown.RefreshShownValue();
        }

        private void ApplySettings()
        {
            AudioManager.instance.SetMusicVolume(volumeSlider.value);
            AudioManager.instance.SetSFXVolume(volumeFXSlider.value);
            SetFullscreen(fullscreenToggle.isOn);
            SetResolution(resolutionDropdown.value);
            SaveSettings();
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int index)
        {
            Resolution resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("volume", volumeSlider.value);
            PlayerPrefs.SetFloat("sfxvolume", volumeFXSlider.value);
            PlayerPrefs.SetInt("musicOn", musicToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("fullscreen", fullscreenToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("resolutionIndex", resolutionDropdown.value);
            PlayerPrefs.SetFloat("sfxVolume", AudioManager.instance.sfxSource.volume);
            PlayerPrefs.SetFloat("musicVolume", AudioManager.instance.musicSource.volume);
            PlayerPrefs.Save();
        }
        private void PlayClickSound()
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayClickSound();
            }
        }
    }
}
