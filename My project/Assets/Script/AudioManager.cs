using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    [System.Serializable]
    public class Sound
    {
        public string name; 
        public AudioClip clip; 
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        public AudioSource musicSource; 
        public AudioSource sfxSource; 

        public List<Sound> musicList = new List<Sound>(); 
        public List<Sound> sfxList = new List<Sound>(); 

        private float currentVolume = 1f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlayMusic(string musicName)
        {
            Sound music = musicList.Find(x => x.name == musicName);
            if (music != null && music.clip != null)
            {
                musicSource.clip = music.clip;
                musicSource.Play();
                musicSource.loop = true;
            }
            else
            {
                Debug.LogError("Music clip not found: " + musicName);
            }
        }

        public void PauseMusic()
        {
            musicSource.Pause();
        }

        public void ResumeMusic()
        {
            musicSource.Play();
        }

        public void PauseSFXMusic()
        {
            sfxSource.Pause();
        }

        public void ResumeSFXMusic()
        {
            sfxSource.Play();
        }

      
        public void PlaySFX(string sfxName)
        {
            Sound sfx = sfxList.Find(x => x.name == sfxName);
            if (sfx != null && sfx.clip != null)
            {
                sfxSource.PlayOneShot(sfx.clip);
            }
            else
            {
                Debug.LogError("SFX clip not found: " + sfxName);
            }
        }

        public void PlayClickSound()
        {
            Sound clickSound = sfxList.Find(x => x.name == "Click");
            if (clickSound != null && clickSound.clip != null)
            {
                sfxSource.PlayOneShot(clickSound.clip);
            }
            else
            {
                Debug.LogError("Click sound not found");
            }
        }

   
        public void SetMusicVolume(float volume)
        {
            musicSource.volume = volume;
        }

        
        public void SetSFXVolume(float volume)
        {
            sfxSource.volume = volume;
        }

       
        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void StopSFX()
        {
            sfxSource.Stop();
        }
    }
}
