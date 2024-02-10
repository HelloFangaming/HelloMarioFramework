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
    public class Piranha : Stompable
    {

        //Components
        private Rigidbody myRigidBody;
        private Animator animator;
        private AudioSource audioPlayer;
        private Collider myCollider;

        //Audio clips
        [SerializeField]
        private AudioClip chompSFX;
        [SerializeField]
        private AudioClip attackSFX;

        //Game
        private bool canAttack = true;

        //Animator hash values
        private static int stompHash = Animator.StringToHash("Stomp");
        private static int attackHash = Animator.StringToHash("Attack");
        
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
            myCollider = GetComponentInChildren<Collider>();
        }

        //If player enters search radius
        private void OnTriggerStay(Collider collision)
        {
            if (canAttack)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    canAttack = false;
                    StartCoroutine(Attack());
                }
            }

        }

        //What to do when stomped. Override this.
        protected override void WhenStomped()
        {
            animator.SetBool(stompHash, true);
            myCollider.enabled = false;
            myRigidBody.detectCollisions = false;
        }

        //Move fixed update to here. Override this.
        protected override void FixedUpdateStompable()
        {
            if (canAttack) Player.singleton.LookAtMe(transform);
        }

        //Attack pattern
        private IEnumerator Attack()
        {
            animator.SetBool(attackHash, true);
            audioPlayer.PlayOneShot(chompSFX);
            yield return new WaitForSeconds(0.3f);
            audioPlayer.PlayOneShot(chompSFX);
            yield return new WaitForSeconds(0.8f);
            audioPlayer.PlayOneShot(attackSFX);
            animator.SetBool(attackHash, false);
            yield return new WaitForSeconds(2.5f);
            canAttack = true;
        }

    }
}
