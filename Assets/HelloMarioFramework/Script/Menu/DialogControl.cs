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
    public class DialogControl : MonoBehaviour
    {

        //Components
        public static DialogControl singleton;
        private AudioSource audioPlayer;
        [SerializeField]
        private GameObject DialogUI;
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text dialogText;
        [SerializeField]
        private GameObject continueCursor;

        //Voice clips
        [SerializeField]
        private AudioClip dialogTypeSFX;

        //Input
        [SerializeField]
        private InputActionReference jumpAction;

        //Lines of dialog
        private string[] dialog;
        private int currentDialog = 0;
        private int currentLetter = 0;
        private bool displaying = false;
        private float typewriter = 0f;
        private bool finishedTyping = false;
        private NPC npc;
        
        void Start()
        {
            singleton = this;
            audioPlayer = GetComponent<AudioSource>();
            jumpAction.action.Enable();
        }
        
        void Update()
        {
            if (displaying)
            {
                if (!finishedTyping)
                {
                    //Finish early
                    if (jumpAction.action.WasPressedThisFrame())
                    {
                        dialogText.text = dialog[currentDialog];
                        finishedTyping = true;
                        continueCursor.SetActive(true);
                    }
                    //Keep typing
                    else
                    {
                        typewriter -= Time.deltaTime;
                        if (typewriter <= 0f)
                        {
                            if (dialog[currentDialog][currentLetter] != ' ') audioPlayer.PlayOneShot(dialogTypeSFX);
                            dialogText.text += dialog[currentDialog][currentLetter];
                            currentLetter++;
                            if (currentLetter < dialog[currentDialog].Length)
                            {
                                if (dialog[currentDialog][currentLetter - 1] == '.' || dialog[currentDialog][currentLetter - 1] == '?' || dialog[currentDialog][currentLetter - 1] == '!' || dialog[currentDialog][currentLetter - 1] == ',')
                                    typewriter += 0.32f;
                                else
                                    typewriter += 0.03f;

                            }
                            else
                            {
                                finishedTyping = true;
                                continueCursor.SetActive(true);
                            }
                        }
                    }
                }
                //Next dialog
                else if (jumpAction.action.WasPressedThisFrame())
                {
                    currentDialog++;
                    NextDialog();
                }
            }
        }

        //Start the dialog engine
        public void DisplayDialog(NPC npcSpeaking)
        {
            npc = npcSpeaking;
            nameText.text = npcSpeaking.npcName;
            dialog = npcSpeaking.dialogue;
            currentDialog = 0;
            displaying = true;
            PauseMenu.singleton.Disable();
            NextDialog();
            if (displaying) DialogUI.SetActive(true);
        }

        private void NextDialog()
        {
            currentLetter = 0;
            typewriter = 0f;
            finishedTyping = false;
            dialogText.text = "";
            continueCursor.SetActive(false);
            jumpAction.action.Reset();

            //If there is more dialog, play voice sound
            if (currentDialog < dialog.Length) audioPlayer.PlayOneShot(npc.voiceSFX);

            //Otherwise, quit
            else
            {
                displaying = false;
                DialogUI.SetActive(false);
                PauseMenu.singleton.Enable();

                //Enable NPC
                npc.FinishedSpeaking();
            }

        }

    }
}
