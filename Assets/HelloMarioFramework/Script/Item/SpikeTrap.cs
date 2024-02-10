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
    public class SpikeTrap : MonoBehaviour
    {

        //Components
        private Animator animator;
        private AudioSource audioPlayer;

        //Audio clips
        [SerializeField]
        private AudioClip signSFX;
        [SerializeField]
        private AudioClip appearSFX;
        [SerializeField]
        private AudioClip endSFX;

        //Start delay
        [SerializeField]
        private float delay = 0f;

        //Animator hash values
        private static int upHash = Animator.StringToHash("Up");
        
        void Start()
        {
            animator = GetComponent<Animator>();
            audioPlayer = GetComponent<AudioSource>();

            if (delay > 0) StartCoroutine(DelayStart());
            else StartCoroutine(SpikeAnimation());
        }

        private IEnumerator SpikeAnimation()
        {
            yield return new WaitForSeconds(3f);

            //Sign
            animator.SetBool(upHash, true);
            yield return new WaitForSeconds(1.1f);
            audioPlayer.PlayOneShot(signSFX);

            //Appear
            yield return new WaitForSeconds(0.7f);
            audioPlayer.PlayOneShot(appearSFX);

            //Go down
            yield return new WaitForSeconds(2f);
            animator.SetBool(upHash, false);
            audioPlayer.PlayOneShot(endSFX);

            StartCoroutine(SpikeAnimation());
        }

        private IEnumerator DelayStart()
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(SpikeAnimation());
        }

    }
}
