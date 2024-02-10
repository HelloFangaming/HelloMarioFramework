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
    public class OptionsMenu : MonoBehaviour
    {

        //Components
        private AudioSource audioPlayer;
        private MenuCursor cursor;
        private OptionsApplier optionsApplier;

        //Audio clips
        [SerializeField]
        private AudioClip changeOptionSFX;

        //Input
        [SerializeField]
        private InputActionReference movementAction;

        //UI
        [SerializeField]
        private Text[] optionsText;

        //Game
        private int[] optionsIndex = new int[13];
        private bool buttonDown = false;

        //Option names
        private string[] defaultOptions = { "Off", "On" };
        private int[] resolutionOptions = { 360, 432, 576, 720, 864, 1080, -1 };

        //Options constants
        private const int MUSIC = 0;
        private const int RESOLUTION = 1;
        private const int LIGHTING = 2;
        private const int FRAME_RATE = 3;
        private const int CAMERA_X = 4;
        private const int CAMERA = 5;
        private const int COLOR = 6;
        private const int BLOOM = 7;
        private const int VIGNETTE = 8;
        private const int OCCLUSION = 9;
        private const int REFLECTIONS = 10;
        private const int DEPTH_OF_FIELD = 11;
        private const int FULL_SCREEN = 12;
        
        void Start()
        {
            audioPlayer = gameObject.AddComponent<AudioSource>();
            cursor = GetComponent<MenuCursor>();
            optionsApplier = FindAnyObjectByType<OptionsApplier>();
            movementAction.action.Enable();

            //Read the current settings

            //Music
            optionsIndex[MUSIC] = OptionsSave.save.musicVolume;

            //Resolution
            switch (Screen.height)
            {
                case 360:
                    optionsIndex[RESOLUTION] = 0;
                    break;
                case 432:
                    optionsIndex[RESOLUTION] = 1;
                    break;
                case 576:
                    optionsIndex[RESOLUTION] = 2;
                    break;
                case 720:
                    optionsIndex[RESOLUTION] = 3;
                    break;
                case 864:
                    optionsIndex[RESOLUTION] = 4;
                    break;
                case 1080:
                    optionsIndex[RESOLUTION] = 5;
                    break;
                default:
                    optionsIndex[RESOLUTION] = 6;
                    break;
            }

            //Lighting
            optionsIndex[LIGHTING] = QualitySettings.GetQualityLevel();

            //Frame Rate
            optionsIndex[FRAME_RATE] = OptionsSave.save.frameRate;

            //Camera X
            optionsIndex[CAMERA_X] = BoolToInt(OptionsSave.save.cameraXRecentering);

            //Camera
            optionsIndex[CAMERA] = BoolToInt(OptionsSave.save.cameraRecentering);

            //Post process
            optionsIndex[COLOR] = BoolToInt(OptionsSave.save.colorGrading);
            optionsIndex[BLOOM] = BoolToInt(OptionsSave.save.bloom);
            optionsIndex[VIGNETTE] = BoolToInt(OptionsSave.save.vignette);
            optionsIndex[OCCLUSION] = BoolToInt(OptionsSave.save.ambientOcclusion);
            optionsIndex[REFLECTIONS] = BoolToInt(OptionsSave.save.screenSpaceReflections);
            optionsIndex[DEPTH_OF_FIELD] = BoolToInt(OptionsSave.save.depthOfField);

            //Full Screen
            optionsIndex[FULL_SCREEN] = BoolToInt(Screen.fullScreen);

            //Update all text
            for (int i = 0; i < 13; i++) UpdateText(i);
        }
        
        void Update()
        {
            float axis = movementAction.action.ReadValue<Vector2>().x;

            //Left
            if (!buttonDown && axis < -0.5f)
            {
                ChangeOption(false);
                buttonDown = true;
            }
            //Right
            else if (!buttonDown && axis > 0.5f)
            {
                ChangeOption(true);
                buttonDown = true;
            }

            //Reset button
            if (buttonDown && axis > -0.5f && axis < 0.5f)
            {
                buttonDown = false;
            }
        }

        //Apply settings when menu is disabled
        private void OnDisable()
        {

            //Music
            OptionsSave.save.musicVolume = (byte)optionsIndex[MUSIC];

            //Resolution
            OptionsApplier.ChangeResolution(resolutionOptions[optionsIndex[RESOLUTION]]);

            //Save resolution for Android
            OptionsSave.save.resolution = resolutionOptions[optionsIndex[RESOLUTION]];

            //Lighting
            QualitySettings.SetQualityLevel(optionsIndex[LIGHTING], true);

            //Frame Rate
            OptionsSave.save.frameRate = (byte)optionsIndex[FRAME_RATE];

            //Camera X
            OptionsSave.save.cameraXRecentering = (optionsIndex[CAMERA_X] == 1);

            //Camera
            OptionsSave.save.cameraRecentering = (optionsIndex[CAMERA] == 1);

            //Post Process
            OptionsSave.save.colorGrading = (optionsIndex[COLOR] == 1);
            OptionsSave.save.bloom = (optionsIndex[BLOOM] == 1);
            OptionsSave.save.vignette = (optionsIndex[VIGNETTE] == 1);
            OptionsSave.save.ambientOcclusion = (optionsIndex[OCCLUSION] == 1);
            OptionsSave.save.screenSpaceReflections = (optionsIndex[REFLECTIONS] == 1);
            OptionsSave.save.depthOfField = (optionsIndex[DEPTH_OF_FIELD] == 1);

            //Full Screen
            Screen.fullScreen = (optionsIndex[FULL_SCREEN] == 1);

            //Apply Settings
            optionsApplier.ChangeSettings();

            //Save settings file
            OptionsSave.save.Save();

        }

        //Change the option
        private void ChangeOption(bool right)
        {
            switch (cursor.GetIndex())
            {
                case MUSIC:
                    //Left
                    if (!right)
                    {
                        if (optionsIndex[MUSIC] > 0)
                        {
                            optionsIndex[MUSIC]--;
                            audioPlayer.PlayOneShot(changeOptionSFX);
                            UpdateText(MUSIC);
                            optionsApplier.musicPlayer.volume = ((float)optionsIndex[MUSIC]) / 20;
                        }
                    }
                    //Right
                    else
                    {
                        if (optionsIndex[MUSIC] < 10)
                        {
                            optionsIndex[MUSIC]++;
                            audioPlayer.PlayOneShot(changeOptionSFX);
                            UpdateText(MUSIC);
                            optionsApplier.musicPlayer.volume = ((float)optionsIndex[MUSIC]) / 20;
                        }
                    }
                    break;
                case RESOLUTION:
                    //Left
                    if (!right)
                    {
                        if (optionsIndex[RESOLUTION] > 0)
                        {
                            optionsIndex[RESOLUTION]--;
                            audioPlayer.PlayOneShot(changeOptionSFX);
                            UpdateText(RESOLUTION);
                        }
                    }
                    //Right
                    else
                    {
                        if (optionsIndex[RESOLUTION] + 1 < resolutionOptions.Length)
                        {
                            optionsIndex[RESOLUTION]++;
                            audioPlayer.PlayOneShot(changeOptionSFX);
                            UpdateText(RESOLUTION);
                        }
                    }
                    break;
                case LIGHTING:
                    //Left
                    if (!right)
                    {
                        if (optionsIndex[LIGHTING] > 0)
                        {
                            optionsIndex[LIGHTING]--;
                            audioPlayer.PlayOneShot(changeOptionSFX);
                            UpdateText(LIGHTING);
                        }
                    }
                    //Right
                    else
                    {
                        if (optionsIndex[LIGHTING] + 1 < QualitySettings.names.Length)
                        {
                            optionsIndex[LIGHTING]++;
                            audioPlayer.PlayOneShot(changeOptionSFX);
                            UpdateText(LIGHTING);
                        }
                    }
                    break;
                case FRAME_RATE:
                    //Left
                    if (!right)
                    {
                        if (optionsIndex[FRAME_RATE] != 30)
                        {
                            if (optionsIndex[FRAME_RATE] == 60) optionsIndex[FRAME_RATE] = 30;
                            else optionsIndex[FRAME_RATE] = 60;
                            audioPlayer.PlayOneShot(changeOptionSFX);
                            UpdateText(FRAME_RATE);
                        }
                    }
                    //Right
                    else
                    {
                        if (optionsIndex[FRAME_RATE] != 0)
                        {
                            if (optionsIndex[FRAME_RATE] == 30) optionsIndex[FRAME_RATE] = 60;
                            else optionsIndex[FRAME_RATE] = 0;
                            audioPlayer.PlayOneShot(changeOptionSFX);
                            UpdateText(FRAME_RATE);
                        }
                    }
                    break;
                default:
                    //Left
                    if (!right)
                    {
                        if (optionsIndex[cursor.GetIndex()] > 0)
                        {
                            optionsIndex[cursor.GetIndex()]--;
                            audioPlayer.PlayOneShot(changeOptionSFX);
                            UpdateText(cursor.GetIndex());
                        }
                    }
                    //Right
                    else
                    {
                        if (optionsIndex[cursor.GetIndex()] + 1 < defaultOptions.Length)
                        {
                            optionsIndex[cursor.GetIndex()]++;
                            audioPlayer.PlayOneShot(changeOptionSFX);
                            UpdateText(cursor.GetIndex());
                        }
                    }
                    break;
            }
        }

        //Update UI Text
        private void UpdateText(int i)
        {
            switch (i)
            {
                case MUSIC:
                    optionsText[MUSIC].text = optionsIndex[MUSIC] * 10 + "%";
                    break;
                case RESOLUTION:
                    if (resolutionOptions[optionsIndex[RESOLUTION]] == -1)
                        optionsText[RESOLUTION].text = "Native";
                    else
                        optionsText[RESOLUTION].text = resolutionOptions[optionsIndex[RESOLUTION]] + "p";
                    break;
                case LIGHTING:
                    optionsText[LIGHTING].text = QualitySettings.names[optionsIndex[LIGHTING]];
                    break;
                case FRAME_RATE:
                    if (optionsIndex[FRAME_RATE] == 0) optionsText[FRAME_RATE].text = "VSync";
                    else optionsText[FRAME_RATE].text = optionsIndex[FRAME_RATE].ToString();
                    break;
                default:
                    optionsText[i].text = defaultOptions[optionsIndex[i]];
                    break;
            }
        }

        private int BoolToInt(bool b)
        {
            if (b) return 1;
            else return 0;
        }

    }
}
