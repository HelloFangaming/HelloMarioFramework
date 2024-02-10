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
    public class StompSwitch : MonoBehaviour
    {

        //Components
        private Animator animator;
        private AudioSource audioPlayer;
        private ButtonHandler myButton;

        //Audio clips
        [SerializeField]
        private AudioClip pressSFX;
        [SerializeField]
        private AudioClip switchSFX;

        //Game
        private bool pressed = false;

        //Animator hash values
        private static int pressHash = Animator.StringToHash("Press");
        
        void Start()
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
            myButton = GetComponent<ButtonHandler>();
        }

        //Stomp to press
        private void OnCollisionEnter(Collision collision)
        {
            if (!pressed)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    foreach (ContactPoint contact in collision.contacts)
                    {
                        if (!pressed && p.IsPound() && Vector3.Dot(contact.normal, Vector3.down) > 0.9f)
                        {
                            StartCoroutine(Press());
                        }
                    }
                }
            }
        }

        //Finish press
        private IEnumerator Press()
        {
            pressed = true;
            audioPlayer.PlayOneShot(pressSFX);
            animator.SetBool(pressHash, true);
            Destroy(GetComponent<MeshCollider>());
            yield return new WaitForSeconds(0.24f);
            audioPlayer.PlayOneShot(switchSFX);
            myButton.SetActive(true);
        }
    }
}
