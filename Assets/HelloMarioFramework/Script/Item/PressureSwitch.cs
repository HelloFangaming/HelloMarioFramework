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

namespace HelloMarioFramework
{
    [RequireComponent(typeof(ButtonHandler))]
    public class PressureSwitch : MonoBehaviour
    {

        //Components
        private AudioSource audioPlayer;
        private ButtonHandler myButton;
        private Transform innerButton;

        //Audio clips
        [SerializeField]
        private AudioClip pressSFX;
        [SerializeField]
        private AudioClip switchSFX;

        //Game
        private bool pressed = false;
        private int pressCount = 0;
        
        void Start()
        {
            audioPlayer = gameObject.AddComponent<AudioSource>();
            myButton = GetComponent<ButtonHandler>();
            innerButton = transform.GetChild(1);
        }
        
        void FixedUpdate()
        {
            if (pressed && pressCount > 0)
            {
                pressCount--;
                if (pressCount == 0)
                {
                    pressed = false;
                    StartCoroutine(PressSound());
                    innerButton.localPosition = Vector3.zero;
                }
            }
        }

        //All physics objects in trigger
        private void OnTriggerStay(Collider collision)
        {
            if (!collision.isTrigger)
            {
                pressCount = 8;
                if (!pressed)
                {
                    pressed = true;
                    StartCoroutine(PressSound());
                    innerButton.localPosition = new Vector3(0f, -0.1f, 0f);
                }
            }
        }

        //Delay press sound effects
        private IEnumerator PressSound()
        {
            audioPlayer.PlayOneShot(pressSFX);
            yield return new WaitForSeconds(0.24f);
            audioPlayer.PlayOneShot(switchSFX);
            myButton.SetActive(pressed);
        }


    }
}
