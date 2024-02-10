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
    public class Magikoopa : Stompable
    {

        //Components
        private Rigidbody myRigidBody;
        private Animator animator;
        private AudioSource audioPlayer;
        private Collider myCollider;

        //Audio clips
        [SerializeField]
        private AudioClip magicSFX;
        [SerializeField]
        private AudioClip appearSFX;
        [SerializeField]
        private AudioClip disappearSFX;

        //Game
        private bool appear = false;

        //Animator hash values
        private static int appearHash = Animator.StringToHash("Appear");
        private static int stompHash = Animator.StringToHash("Stomp");

        //Magic to cast
        [SerializeField]
        private GameObject magicSpell;

        //Start delay
        [SerializeField]
        private float delay = 0f;
        
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
            myCollider = GetComponentInChildren<Collider>();

            stompHeightCheck = 0.1f;
            transform.localScale = Vector3.zero;
            myRigidBody.detectCollisions = false;
            if (delay > 0) StartCoroutine(DelayCast());
            else StartCoroutine(Cast());
        }

        //What to do when stomped. Override this.
        protected override void WhenStomped()
        {
            animator.SetBool(stompHash, true);
            myCollider.enabled = false;
        }

        //Move fixed update to here. Override this.
        protected override void FixedUpdateStompable()
        {
            if (appear)
            {
                if (transform.localScale.y < 1f)
                {
                    transform.localScale += Vector3.one * Time.fixedDeltaTime;
                    if (transform.localScale.x >= 1f) transform.localScale = Vector3.one;
                }

                //Change direction
                Player.singleton.LookAtMe(transform);
            }
            else if (transform.localScale.y > 0f)
            {
                transform.localScale -= Vector3.one * Time.fixedDeltaTime;
                if (transform.localScale.x <= 0f) transform.localScale = Vector3.zero;
            }

        }

        //Throw balls
        private IEnumerator Cast()
        {
            yield return new WaitForSeconds(7f);

            //If player is nearby, appear
            if (Player.singleton.CanBeChased(transform.position, 15f))
            {
                appear = true;
                audioPlayer.PlayOneShot(appearSFX);
                animator.SetBool(appearHash, true);
                myRigidBody.detectCollisions = true;
                yield return new WaitForSeconds(1.9f);
                audioPlayer.PlayOneShot(magicSFX);
                GameObject o = Instantiate(magicSpell);
                o.transform.position = transform.position + transform.forward;
                yield return new WaitForSeconds(1.5f);
                appear = false;
                audioPlayer.PlayOneShot(disappearSFX);
                animator.SetBool(appearHash, false);
                myRigidBody.detectCollisions = false;
            }

            StartCoroutine(Cast());
        }

        private IEnumerator DelayCast()
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(Cast());
        }

    }
}
