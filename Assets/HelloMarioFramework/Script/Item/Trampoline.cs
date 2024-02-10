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
    public class Trampoline : MonoBehaviour
    {

        //Components
        private Animator animator;
        private AudioSource audioPlayer;

        [SerializeField]
        private float bounceMax = 24f;

        //Audio clips
        [SerializeField]
        private AudioClip springSFX;
        [SerializeField]
        private AudioClip springBounceSFX;

        //Animator hash values
        private static int bounceHash = Animator.StringToHash("Bounce");
        
        void Start()
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
        }

        //Bounce on contact
        private void OnCollisionEnter(Collision collision)
        {
            Player p = collision.transform.GetComponent<Player>();
            if (p != null)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    if (Vector3.Dot(contact.normal, Vector3.down) > 0.9)
                    {
                        if (p.jumpAction.action.IsPressed()) audioPlayer.PlayOneShot(springBounceSFX);
                        else audioPlayer.PlayOneShot(springSFX);
                        p.UndoPound();
                        p.BounceUp(bounceMax);
                        StartCoroutine(Shake());
                        break;
                    }
                }
            }
        }

        //Shake
        public IEnumerator Shake()
        {
            animator.SetBool(bounceHash, true);
            yield return new WaitForSeconds(0.5f);
            animator.SetBool(bounceHash, false);
        }

    }
}
