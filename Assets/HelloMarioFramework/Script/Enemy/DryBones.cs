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
    public class DryBones : Stompable
    {

        //Components
        private Rigidbody myRigidBody;
        private Animator animator;
        private AudioSource audioPlayer;
        private Collider myCollider;

        //Audio clips
        [SerializeField]
        private AudioClip breakSFX;
        [SerializeField]
        private AudioClip unbreakSFX;

        //Game
        private bool chase = false;
        private bool cooldown = false;

        //Animator hash values
        private static int chaseHash = Animator.StringToHash("Chase");
        private static int stompHash = Animator.StringToHash("Stomp");
        private static int speedHash = Animator.StringToHash("Speed");

        //Physics faller
        [SerializeField]
        private GameObject fallObject;
        private Transform fallInstance;
        private bool fall = false;
        
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            audioPlayer = GetComponent<AudioSource>();
            myCollider = GetComponent<Collider>();

            myRigidBody.freezeRotation = true;
        }

        //Before draw calls
        private void LateUpdate()
        {
            if (chase)
            {
                Vector3 i = new Vector3(myRigidBody.velocity.x, 0f, myRigidBody.velocity.z);
                animator.SetFloat(speedHash, i.magnitude);
            }
        }

        //What to do when stomped. Override this.
        protected override void WhenStomped()
        {
            chase = false;
            stomped = false;

            StartCoroutine(Crumble());
        }

        //What to do when hurting player. Override this.
        protected override void WhenHurtPlayer(Player p)
        {
            chase = false;
            StopAllCoroutines();
            if (p.GetHealth() > 0)
                StartCoroutine(Cooldown(1.1f));
            else
            {
                animator.SetBool(chaseHash, false);
                cooldown = true;
            }
        }

        //Move fixed update to here. Override this.
        protected override void FixedUpdateStompable()
        {
            //Force upright
            myRigidBody.rotation = Quaternion.Euler(0f, myRigidBody.rotation.eulerAngles.y, 0f);

            if (!cooldown)
            {

                //If player is nearby
                if (Player.singleton.CanBeChased(transform.position, 10f))
                {
                    //Look at player
                    Player.singleton.LookAtMe(transform);

                    //Start chase
                    if (!chase)
                    {
                        chase = true;
                        StartCoroutine(Cooldown(0.9f));
                    }

                    //Chase
                    else
                    {
                        //Move towards player
                        myRigidBody.AddForce((Player.singleton.transform.position - transform.position).normalized * 3.5f);
                    }
                }
                else if (chase)
                {
                    chase = false;
                    StartCoroutine(Cooldown(1.1f));
                }
            }

            if (fall) transform.position = fallInstance.position;
        }

        //Crumble and uncrumble
        private IEnumerator Crumble()
        {
            cooldown = true;

            audioPlayer.PlayOneShot(breakSFX);
            animator.SetBool(stompHash, true);
            animator.SetBool(chaseHash, false);

            myCollider.enabled = false;
            myRigidBody.isKinematic = true;
            myRigidBody.detectCollisions = false;
            GameObject o = Instantiate(fallObject);
            o.transform.position = transform.position;
            fallInstance = o.transform;
            fall = true;

            yield return new WaitForSeconds(4.5f);

            audioPlayer.PlayOneShot(unbreakSFX);
            animator.SetBool(stompHash, false);

            yield return new WaitForSeconds(1f);

            myCollider.enabled = true;
            myRigidBody.isKinematic = false;
            myRigidBody.detectCollisions = true;
            Destroy(o);
            fall = false;
            myRigidBody.velocity = Vector3.up;

            yield return new WaitForSeconds(2f);

            cooldown = false;
        }

        //Cooldown
        private IEnumerator Cooldown(float f)
        {
            animator.SetBool(chaseHash, chase);
            cooldown = true;
            yield return new WaitForSeconds(f); //0.9f
            cooldown = false;
        }

    }
}
