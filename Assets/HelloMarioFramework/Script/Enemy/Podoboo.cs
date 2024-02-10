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
    public class Podoboo : MonoBehaviour
    {

        //Components
        private Animator animator;
        private AudioSource audioPlayer;

        //Audio clips
        [SerializeField]
        private AudioClip inSFX;
        [SerializeField]
        private AudioClip outSFX;

        //Game
        private bool falling = true;
        private bool pause = false;
        private float fallSpeed = 0.0f;

        //Animator hash values
        private static int fallHash = Animator.StringToHash("Fall");

        //Lowest and highest y value
        [SerializeField]
        private float lowestY = -2f;
        private float highestY;
        private bool highestYSet = false;
        
        void Start()
        {
            animator = GetComponent<Animator>();
            audioPlayer = GetComponent<AudioSource>();
        }
        
        void FixedUpdate()
        {
            if (!pause)
            {
                if (falling)
                {
                    if (transform.position.y < lowestY)
                    {
                        StartCoroutine(Wait());
                    }
                    else
                    {
                        fallSpeed += 0.2f;
                        if (fallSpeed > 8f)
                        {
                            fallSpeed = 8f;
                            if (!highestYSet)
                            {
                                highestYSet = true;
                                highestY = transform.position.y;
                            }
                        }
                        transform.position += Vector3.down * fallSpeed * Time.fixedDeltaTime;
                    }
                }
                else
                {
                    if (transform.position.y > highestY)
                    {
                        falling = true;
                        StartCoroutine(DelayAnim());
                    }
                    else
                    {
                        transform.position += Vector3.down * fallSpeed * Time.fixedDeltaTime;
                    }
                }
            }
        }

        //Wait
        private IEnumerator Wait()
        {
            pause = true;
            audioPlayer.PlayOneShot(inSFX);
            yield return new WaitForSeconds(1f);
            audioPlayer.PlayOneShot(outSFX);
            pause = false;
            falling = false;
            animator.SetBool(fallHash, false);
            fallSpeed = -fallSpeed;
        }

        //Delay animation
        private IEnumerator DelayAnim()
        {
            yield return new WaitForSeconds(0.2f);
            animator.SetBool(fallHash, true);
        }

    }
}
