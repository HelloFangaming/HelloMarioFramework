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
    public class JumpPanel : MonoBehaviour
    {

        //Components
        private Animator animator;
        private AudioSource audioPlayer;

        //Audio clips
        [SerializeField]
        private AudioClip jumpSFX;

        //Animator hash values
        private static int jumpHash = Animator.StringToHash("Jump");

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
        }

        void OnTriggerExit(Collider collision)
        {
            Player p = collision.transform.GetComponent<Player>();
            if (p != null)
            {
                if (p.jumpAction.action.IsPressed())
                {
                    p.BounceUp(24f);
                    StopAllCoroutines();
                    StartCoroutine(Triggered());
                }
            }
        }

        private IEnumerator Triggered()
        {
            animator.SetBool(jumpHash, true);
            audioPlayer.PlayOneShot(jumpSFX);

            yield return new WaitForSeconds(0.5f);

            animator.SetBool(jumpHash, false);
        }

    }
}
