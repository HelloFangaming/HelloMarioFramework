/*
 *  Copyright (c) 2024 Hello Fangaming
 *
 *  Use of this source code is governed by an MIT-style
 *  license that can be found in the LICENSE file or at
 *  https://opensource.org/licenses/MIT.
 *  
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HelloMarioFramework
{
    public class MusicControl : MonoBehaviour
    {

        //Components
        public static MusicControl singleton;
        private AudioSource audioPlayer;
        private AudioSource musicPlayer;

        //Audio clips
        [SerializeField]
        private AudioClip victoryMusic;
        [SerializeField]
        private AudioClip victoryShortMusic;
        [SerializeField]
        private AudioClip deathMusic;
        [SerializeField]
        private AudioClip starAppearsSFX;

        private bool bugfix = false;
        private bool bugfix2 = false;
        private bool bugfix3 = false;
        
        void Start()
        {
            singleton = this;
            audioPlayer = gameObject.AddComponent<AudioSource>();
            audioPlayer.volume = 0.4f;
            musicPlayer = GetComponent<AudioSource>();
        }

        //Level ending star
        public void Victory(string starName)
        {
            StopMusic();
            audioPlayer.PlayOneShot(victoryMusic);
            PauseMenu.singleton.LengthenTitleCard();
            PauseMenu.singleton.DisplayTitleCards("You got a star!", starName);
            if (SaveData.save.CollectStar(starName))
                SaveData.save.Save();
            SaveData.checkpoint = false;
            PauseMenu.levelSubText = "";
            LoadingScreen.scene = LoadingScreen.hubScene;
            StartCoroutine(ChangeScene(6f));
        }

        //Non-level ending star
        public void VictoryShort(string starName)
        {
            StartCoroutine(RestartMusic(4f));
            audioPlayer.Stop();
            audioPlayer.PlayOneShot(victoryShortMusic);
            PauseMenu.singleton.DisplayTitleCards("You got a star!", starName);
            if (SaveData.save.CollectStar(starName))
                SaveData.save.Save();
        }

        public void Death()
        {
            StopMusic();
            audioPlayer.PlayOneShot(deathMusic);
            LoadingScreen.scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
            StartCoroutine(ChangeScene(3f));
        }

        public void Return()
        {
            StopMusic();
            SaveData.checkpoint = false;
            PauseMenu.levelSubText = "";
            if (LoadingScreen.IsHubScene())
            {
                LoadingScreen.scene = LoadingScreen.titleScene;
                SaveData.save.Save();
            }
            else
                LoadingScreen.scene = LoadingScreen.hubScene;
            StartCoroutine(ChangeScene(0.1f));
        }

        public void StarAppears()
        {
            StartCoroutine(StarAppearSound());
        }

        public void ChangeMusic(AudioClip newMusic)
        {
            if (musicPlayer.clip != newMusic)
            {
                musicPlayer.Stop();
                musicPlayer.clip = newMusic;
                musicPlayer.Play();
                if (bugfix2) musicPlayer.Pause();
            }
        }

        public void StopMusic()
        {
            musicPlayer.Stop();
            audioPlayer.Stop();
        }

        public void PauseMusic()
        {
            musicPlayer.Pause();
        }

        public void ResumeMusic()
        {
            musicPlayer.UnPause();
        }

        private IEnumerator StarAppearSound()
        {
            if (bugfix3)
            {
                bugfix3 = false;
                //Wait for victory short to play first if it is playing.
                yield return new WaitForSeconds(4f);
            }
            bugfix2 = true;
            musicPlayer.Pause();
            audioPlayer.PlayOneShot(starAppearsSFX);
            yield return new WaitForSeconds(4.9f);
            if (!bugfix && !bugfix3)
            {
                musicPlayer.UnPause();
                PauseMenu.singleton.Enable();
            }
            bugfix2 = false;
        }

        private IEnumerator RestartMusic(float t)
        {
            bugfix3 = true;
            PauseMenu.singleton.Disable();
            musicPlayer.Pause();
            yield return new WaitForSeconds(t);
            if (bugfix3)
            {
                musicPlayer.UnPause();
                bugfix3 = false;
                PauseMenu.singleton.Enable();
            }
        }

        public void Enter()
        {
            musicPlayer.Stop();
            SaveData.checkpoint = false;
            StartCoroutine(ChangeScene(1.5f));
        }

        private IEnumerator ChangeScene(float f)
        {
            bugfix = true;
            PauseMenu.singleton.Disable();
            yield return new WaitForSeconds(f);
            FadeControl.singleton.FadeToLoadingScreen();
        }

    }
}
