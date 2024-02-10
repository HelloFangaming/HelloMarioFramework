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
    public class Boo : MonoBehaviour
    {

        //Components
        private Rigidbody myRigidBody;
        private Animator animator;
        private AudioSource audioPlayer;

        //Audio clips
        [SerializeField]
        private AudioClip voiceSFX;

        //Game
        private bool shy = false;
        private bool chase = false;
        private bool cooldown = false;

        //Animator hash values
        private static int chaseHash = Animator.StringToHash("Chase");
        private static int shyHash = Animator.StringToHash("Shy");
        
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
        }
        
        void FixedUpdate()
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

                    //If player is looking at you
                    if (Vector3.Dot(Player.singleton.transform.forward, transform.position - Player.singleton.transform.position) > 0)
                    {
                        if (!shy) StartCoroutine(Cooldown());
                        shy = true;
                        chase = false;
                    }
                    else
                    {
                        if (shy) StartCoroutine(Cooldown());
                        if (!chase) audioPlayer.PlayOneShot(voiceSFX);

                        //Move towards player
                        myRigidBody.AddForce((Player.singleton.transform.position - transform.position).normalized * 3.5f);

                        shy = false;
                        chase = true;
                    }
                }
                else
                {
                    shy = false;
                    chase = false;
                }
            }

        }

        //Before draw calls
        private void LateUpdate()
        {
            animator.SetBool(chaseHash, chase);
            animator.SetBool(shyHash, shy);
        }

        //Cooldown
        private IEnumerator Cooldown()
        {
            cooldown = true;
            yield return new WaitForSeconds(0.4f);
            cooldown = false;
        }

    }
}
