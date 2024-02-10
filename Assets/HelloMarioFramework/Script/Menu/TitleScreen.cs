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
using UnityEngine.SceneManagement;

namespace HelloMarioFramework
{
    public class TitleScreen : MonoBehaviour
    {

        //Components
        private AudioSource audioPlayer;
        private AudioSource musicPlayer;

        //Game
        private bool fileSelect = false;
        private int index = 0;
        private bool buttonDown = false;
        private bool selected = false;
        private bool newGame = false;

        //Audio clips
        [SerializeField]
        private AudioClip selectSFX;
        [SerializeField]
        private AudioClip moveSFX;
        [SerializeField]
        private AudioClip selectVoiceSFX;
        [SerializeField]
        private AudioClip fileSelectMusic;

        //Input
        [SerializeField]
        private InputActionReference jumpAction;
        [SerializeField]
        private InputActionReference movementAction;

        //UI
        [SerializeField]
        private GameObject titleScreenUI;
        [SerializeField]
        private GameObject fileSelectUI;
        [SerializeField]
        private Text fileText;
        [SerializeField]
        private Text dataText;
        [SerializeField]
        private SpriteBlinker aButton;
        private RectTransform titleScreenTransform;
        private RectTransform fileSelectTransform;

        //Hub world scene
        [SerializeField]
        private SceneReference hubScene;
        
        void Start()
        {
            musicPlayer = transform.GetComponent<AudioSource>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
            titleScreenTransform = titleScreenUI.GetComponent<RectTransform>();
            fileSelectTransform = fileSelectUI.GetComponent<RectTransform>();
            jumpAction.action.Enable();
            movementAction.action.Enable();

            //Remember scene information
            LoadingScreen.titleScene = SceneManager.GetActiveScene().path;
            LoadingScreen.hubScene = hubScene.ScenePath;

#if (UNITY_STANDALONE && !UNITY_EDITOR)
            //Disable mouse cursor on windows standalone
            Cursor.visible = false;
#endif
#if (UNITY_ANDROID || UNITY_IOS)
            //Set screen resolution on Android (Saved automatically on Windows, but Unity is inconsistent)
            if (OptionsSave.save == null) OptionsSave.Load();
            OptionsApplier.ChangeResolution(OptionsSave.save.resolution);
#endif
        }

        void Update()
        {
            if (!selected)
            {
                //Title screen
                if (!fileSelect)
                {
                    //Press A to go to file select
                    if (jumpAction.action.WasPressedThisFrame())
                    {
                        fileSelect = true;
                        musicPlayer.Stop();
                        audioPlayer.PlayOneShot(selectSFX);
                        audioPlayer.PlayOneShot(selectVoiceSFX);
                        StartCoroutine(Delay());
                    }
                }

                //File select
                else
                {
                    //Press A to select this file
                    if (jumpAction.action.WasPressedThisFrame())
                    {
                        musicPlayer.Stop();
                        audioPlayer.PlayOneShot(selectSFX);
                        audioPlayer.PlayOneShot(selectVoiceSFX);

                        //Create new game if needed
                        if (newGame) SaveData.NewGame();

                        StartCoroutine(ChangeScene());
                    }
                    //Movement keys
                    else
                    {
                        float axis = movementAction.action.ReadValue<Vector2>().x;
                        //Left
                        if (!buttonDown && axis < -0.5f)
                        {
                            buttonDown = true;
                            audioPlayer.PlayOneShot(moveSFX);
                            index--;
                            if (index < 0) index = 25;
                            UpdateFileSelectText();
                        }
                        //Right
                        else if (!buttonDown && axis > 0.5f)
                        {
                            buttonDown = true;
                            audioPlayer.PlayOneShot(moveSFX);
                            index++;
                            if (index > 25) index = 0;
                            UpdateFileSelectText();
                        }
                        //Reset
                        else if (buttonDown && axis > -0.5f && axis < 0.5f)
                        {
                            buttonDown = false;
                        }
                    }
                    //Move file select box into view
                    if (fileSelectTransform.anchoredPosition.y < 0f)
                    {
                        fileSelectTransform.anchoredPosition += Vector2.up * 480f * Time.deltaTime;
                        titleScreenTransform.anchoredPosition += Vector2.down * 480f * Time.deltaTime;
                        if (fileSelectTransform.anchoredPosition.y >= 0f)
                            fileSelectTransform.anchoredPosition = new Vector2(fileSelectTransform.anchoredPosition.x, 0f);
                    }
                }
            }
        }

        private void UpdateFileSelectText()
        {
            //Get file character
            char c = (char)('A' + index);

            fileText.text = "File " + c;

            SaveData.SetFileName("File" + c);

            //Attempt to load
            newGame = !SaveData.Load();

            //If the game was loaded
            if (!newGame)
            {
                dataText.text = "<color=yellow>Coins:</color>\t\t" + SaveData.save.GetCoins() + System.Environment.NewLine + "<color=yellow>Stars:</color>\t\t" + SaveData.save.GetStarCount();
            }

            //Otherwise, this is a new game
            else
            {
                dataText.text = "New Game";
            }
        }

        private IEnumerator Delay()
        {
            selected = true;
            aButton.delay = 0.1f;
            yield return new WaitForSeconds(1.5f);
            //titleScreenUI.SetActive(false);
            fileSelectUI.SetActive(true);
            UpdateFileSelectText();
            selected = false;
            musicPlayer.clip = fileSelectMusic;
            musicPlayer.Play();
        }

        private IEnumerator ChangeScene()
        {
            selected = true;
            yield return new WaitForSeconds(1.5f);
            LoadingScreen.scene = hubScene.ScenePath;
            FadeControl.singleton.FadeToLoadingScreen();
        }
    }
}
