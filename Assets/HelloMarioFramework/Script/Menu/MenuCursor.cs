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
using UnityEngine.InputSystem;

namespace HelloMarioFramework
{
    public class MenuCursor : MonoBehaviour
    {

        //Components
        private AudioSource audioPlayer;
        private RectTransform rect;

        //Number of choices
        [SerializeField]
        private int choices = 0;
        [SerializeField]
        private int offset = 80;

        //Audio clips
        [SerializeField]
        private AudioClip selectSFX;
        [SerializeField]
        private AudioClip moveSFX;

        //Input
        [SerializeField]
        private InputActionReference movementAction;

        //Game
        private int index = 0;
        private bool buttonDown = false;
        private bool selected = false;
        private bool axisLow = false;
        private bool coroutineRunning = false;
        
        void Start()
        {
            audioPlayer = gameObject.AddComponent<AudioSource>();
            rect = GetComponent<RectTransform>();
            movementAction.action.Enable();
        }
        
        void Update()
        {
            if (!selected)
            {
                float axis = movementAction.action.ReadValue<Vector2>().y;
                //Up
                if (!buttonDown && axis > 0.5f)
                {
                    ButtonPress();
                    if (index > 0)
                    {
                        rect.localPosition += Vector3.up * offset;
                        index--;
                    }
                    else
                    {
                        rect.localPosition += Vector3.down * offset * (choices - 1);
                        index = choices - 1;
                    }

                }
                //Down
                else if (!buttonDown && axis < -0.5f)
                {
                    ButtonPress();
                    if (index < choices - 1)
                    {
                        rect.localPosition += Vector3.down * offset;
                        index++;
                    }
                    else
                    {
                        rect.localPosition += Vector3.up * offset * (choices - 1);
                        index = 0;
                    }

                }
                //Reset
                else
                {
                    axisLow = axis > -0.5f && axis < 0.5f;
                    if (buttonDown && axisLow)
                    {
                        StopAllCoroutines();
                        coroutineRunning = false;
                        buttonDown = false;
                    }
                }
            }
        }

        private void OnEnable()
        {
            ResetIndex();
            coroutineRunning = false;
        }

        public int Select()
        {
            selected = true;
            audioPlayer.Stop();
            audioPlayer.PlayOneShot(selectSFX);
            return index;
        }

        public void DeSelect()
        {
            selected = false;
        }

        public int GetIndex()
        {
            return index;
        }

        public void ResetIndex()
        {
            if (index > 0)
            {
                rect.localPosition += Vector3.up * offset * index;
                index = 0;
            }
        }

        private void ButtonPress()
        {
            buttonDown = true;
            audioPlayer.PlayOneShot(moveSFX);
            if (!coroutineRunning) StartCoroutine(MultiMove());
        }

        //Allow the player to hold up/down to move faster
        private IEnumerator MultiMove()
        {
            coroutineRunning = true;
            yield return new WaitForSecondsRealtime(0.5f);
            while (!axisLow)
            {
                buttonDown = false;
                yield return new WaitForSecondsRealtime(0.1f);
            }
            coroutineRunning = false;
        }

    }
}
