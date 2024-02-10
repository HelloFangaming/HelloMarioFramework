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
using UnityEngine.InputSystem;

namespace HelloMarioFramework
{
    public class PauseMenu : MonoBehaviour
    {

        //Components
        public static PauseMenu singleton;
        private AudioSource audioPlayer;
        [SerializeField]
        private GameObject pauseUI;
        [SerializeField]
        private Transform extraUI;
        [SerializeField]
        private MenuCursor cursor;

        //Audio clips
        [SerializeField]
        private AudioClip pauseSFX;
        [SerializeField]
        private AudioClip returnSFX;
        [SerializeField]
        private AudioClip moveSFX;
        [SerializeField]
        private AudioClip optionsSFX;

        //Input
        [SerializeField]
        private InputActionReference pauseAction;
        [SerializeField]
        private InputActionReference jumpAction;

        //UI
        [SerializeField]
        private GameObject progressUI;
        [SerializeField]
        private Text progressText;
        [SerializeField]
        private Text returnText;
        [SerializeField]
        private AutoHide titleCardsUI;
        [SerializeField]
        private Text levelNameText;
        private Text[] titleCardsText = new Text[2];

        //Options menu prefab
        [SerializeField]
        private GameObject optionsPrefab;

        //Level name
        [SerializeField]
        private string levelName;
        public static string levelSubText = "";

        //Stars in the current scene
        private Star[] starList;

        //Game
        private bool selected = false;
        private bool paused = false;
        private bool optionsMenuOpen = false;
        private GameObject optionsMenu;

        private void Awake()
        {
            singleton = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            audioPlayer = GetComponent<AudioSource>();
            pauseAction.action.Enable();
            jumpAction.action.Enable();
            if (LoadingScreen.IsHubScene())
                returnText.text = "Save & Quit";
            titleCardsText[0] = titleCardsUI.transform.GetChild(0).GetComponent<Text>();
            titleCardsText[1] = titleCardsUI.transform.GetChild(1).GetComponent<Text>();
            DisplayTitleCards(levelName, levelSubText);
            levelNameText.text = levelName;
            starList = FindObjectsByType<Star>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            //Hide progress UI if there are no stars in the current scene
            if (starList.Length == 0) progressUI.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (optionsMenuOpen)
            {
                if (jumpAction.action.WasPressedThisFrame())
                {
                    audioPlayer.PlayOneShot(returnSFX);
                    Destroy(optionsMenu);
                    optionsMenuOpen = false;
                    pauseUI.SetActive(true);
                    cursor.DeSelect();
                }
            }
            else if (!selected)
            {
                if (pauseAction.action.WasPressedThisFrame())
                {
                    audioPlayer.PlayOneShot(pauseSFX);
                    paused = !paused;
                    pauseUI.SetActive(paused);
                    //Pause
                    if (paused)
                    {
                        Time.timeScale = 0.0f;
                        MusicControl.singleton.PauseMusic();

                        //Update star counter
                        if (starList.Length > 0)
                        {
                            int p = 0;

                            foreach (Star s in starList)
                            {
                                if (s.IsCollected()) p++;
                            }

                            progressText.text = p + "/" + starList.Length;
                        }

                    }
                    //Unpause
                    else
                    {
                        Time.timeScale = 1.0f;
                        MusicControl.singleton.ResumeMusic();
                    }
                }
                else if (paused && jumpAction.action.WasPressedThisFrame())
                {
                    int i = cursor.Select();
                    jumpAction.action.Reset(); //Prevents player jumping after menu item is selected
                    pauseUI.SetActive(false);
                    switch (i)
                    {
                        //Continue
                        case 0:
                            cursor.DeSelect();
                            audioPlayer.PlayOneShot(pauseSFX);
                            MusicControl.singleton.ResumeMusic();
                            Time.timeScale = 1.0f;
                            paused = false;
                            break;
                        //Options
                        case 1:
                            optionsMenuOpen = true;
                            optionsMenu = CreateUI(optionsPrefab);
                            audioPlayer.PlayOneShot(optionsSFX);
                            break;
                        //Quit
                        case 2:
                            selected = true;
                            MusicControl.singleton.Return();
                            audioPlayer.PlayOneShot(returnSFX);
                            Time.timeScale = 1.0f;
                            paused = false;
                            break;
                    }
                }
            }
        }

        //Disable pause menu
        public void Disable()
        {
            selected = true;
        }

        //Enable pause menu
        public void Enable()
        {
            selected = false;
        }

        //Create UI
        public GameObject CreateUI(GameObject newUI)
        {
            return Instantiate(newUI, extraUI);
        }

        //Display the title cards
        public void DisplayTitleCards(string title, string desc)
        {
            titleCardsText[0].text = title;
            titleCardsText[1].text = desc;
            titleCardsUI.ShowMe();
        }

        //Lengthen title cards
        public void LengthenTitleCard()
        {
            titleCardsUI.autoHideAfter = 100f;
        }

    }
}
