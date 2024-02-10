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
    public class BeepBlockBeeper : MonoBehaviour
    {

        //Components
        private AudioSource audioPlayer;

        //Audio clips
        [SerializeField]
        private AudioClip beepSFX;
        [SerializeField]
        private AudioClip swapSFX;

        //Time values (Default: Synced to Beep Block Skyway music)
        [SerializeField]
        private float startTime = 2.3f;
        [SerializeField]
        private float beepTime = 0.5f;
        [SerializeField]
        private float pauseTime = 2.441f;

        //Game
        private BeepBlock[] blockList;
        
        void Start()
        {
            audioPlayer = gameObject.AddComponent<AudioSource>();
            blockList = FindObjectsByType<BeepBlock>(FindObjectsSortMode.None);
            StartCoroutine(Beep());
        }

        private IEnumerator Beep()
        {
            yield return new WaitForSeconds(startTime);

            //Infinite loop
            while (true)
            {
                //Beeps
                audioPlayer.PlayOneShot(beepSFX);
                yield return new WaitForSeconds(beepTime);
                audioPlayer.PlayOneShot(beepSFX);
                yield return new WaitForSeconds(beepTime);
                audioPlayer.PlayOneShot(beepSFX);
                yield return new WaitForSeconds(beepTime);

                //Swap
                audioPlayer.PlayOneShot(swapSFX);
                foreach (BeepBlock b in blockList) b.Swap();

                yield return new WaitForSeconds(pauseTime);
            }
        }

    }
}
