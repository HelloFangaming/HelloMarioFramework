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
    public class IntroMenu : MonoBehaviour
    {

        //Components
        private AudioSource audioPlayer;
        [SerializeField]
        private Text levelName;
        [SerializeField]
        private Text starName;
        [SerializeField]
        private RectTransform starList;
        [SerializeField]
        private int offset = 160;

        //Audio clips
        [SerializeField]
        private AudioClip introMusic;
        [SerializeField]
        private AudioClip moveSFX;
        [SerializeField]
        private AudioClip enterPaintingSFX;
        private AudioClip letsGoSFX;

        //Input
        [SerializeField]
        private InputActionReference jumpAction;
        [SerializeField]
        private InputActionReference movementAction;

        //Game
        private Painting.StarInfo[] starArray;
        private bool displayed = false;
        private int index = 0;
        private bool buttonDown = false;
        private int choices = 0;
        
        void Update()
        {
            if (displayed)
            {
                //Enter stage
                if (jumpAction.action.WasPressedThisFrame())
                {
                    displayed = false;
                    LoadingScreen.scene = starArray[index].scene.ScenePath;
                    PauseMenu.levelSubText = starArray[index].starName;
                    MusicControl.singleton.Enter();
                    audioPlayer.Stop();
                    audioPlayer.PlayOneShot(enterPaintingSFX);
                    audioPlayer.PlayOneShot(letsGoSFX);
                }
                else
                {
                    float axis = movementAction.action.ReadValue<Vector2>().x;
                    //Left
                    if (!buttonDown && index > 0 && axis < -0.5f)
                    {
                        buttonDown = true;
                        audioPlayer.PlayOneShot(moveSFX);
                        starList.localPosition += Vector3.right * offset;
                        index--;
                        starName.text = starArray[index].starName;
                    }
                    //Right
                    else if (!buttonDown && index < choices && axis > 0.5f)
                    {
                        buttonDown = true;
                        audioPlayer.PlayOneShot(moveSFX);
                        starList.localPosition += Vector3.left * offset;
                        index++;
                        starName.text = starArray[index].starName;
                    }
                    //Reset
                    else if (buttonDown && axis > -0.5f && axis < 0.5f)
                    {
                        buttonDown = false;
                    }
                }
            }

        }

        //Called by the painting
        public void DisplayIntro(string nameOfLevel, Painting.StarInfo[] starData, AudioClip voiceClip)
        {
            audioPlayer = gameObject.AddComponent<AudioSource>();
            starArray = starData;
            audioPlayer.PlayOneShot(introMusic);
            displayed = true;
            levelName.text = nameOfLevel;
            starName.text = starArray[0].starName;
            GameObject starObj = starList.GetChild(0).gameObject;
            for (int i = 0; i < starData.Length; i++)
            {
                //Create necessary amount of stars
                GameObject newStar = Instantiate(starObj, starList);
                newStar.GetComponent<RectTransform>().localPosition = Vector3.right * offset * i;

                //Check if star is collected
                if (SaveData.save.CheckCollection(starData[i].starName))
                {
                    newStar.GetComponent<Image>().color = Color.yellow;
                    if (choices < starData.Length - 1) choices++;
                }
            }
            Destroy(starObj);
            letsGoSFX = voiceClip;
        }
    }
}
