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
    public class BlockSwitch : MonoBehaviour
    {

        //Components
        private Animator animator;
        private AudioSource audioPlayer;
        private ButtonHandler myButton;

        //Audio clips
        [SerializeField]
        private AudioClip bumpSFX;
        [SerializeField]
        private AudioClip switchSFX;

        //Animator hash values
        private static int bumpHash = Animator.StringToHash("Bump");

        //Game
        private bool bumpable = true;
        private bool pressed = false;
        
        void Start()
        {
            animator = GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
            myButton = GetComponent<ButtonHandler>();
        }

        //Bump on contact
        private void OnCollisionEnter(Collision collision)
        {
            if (bumpable)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    foreach (ContactPoint contact in collision.contacts)
                    {
                        if (Vector3.Dot(contact.normal, Vector3.up) > 0.9f)
                        {
                            StartCoroutine(Bump(1));
                            break;
                        }
                        else if (p.IsPound() && Vector3.Dot(contact.normal, Vector3.down) > 0.9f)
                        {
                            StartCoroutine(Bump(-1));
                            break;
                        }
                    }
                }
            }
        }

        //Block hit
        protected virtual void BlockHit()
        {
            pressed = !pressed;
            audioPlayer.PlayOneShot(switchSFX);
            myButton.SetActive(pressed);
            bumpable = true;
        }

        //Bump
        public IEnumerator Bump(int i)
        {
            bumpable = false;
            audioPlayer.PlayOneShot(bumpSFX);

            animator.SetInteger(bumpHash, i);
            yield return new WaitForSeconds(0.24f);
            animator.SetInteger(bumpHash, 0);

            BlockHit();
        }

    }
}
