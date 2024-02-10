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
    public class Bully : Stompable
    {

        //Components
        private Rigidbody myRigidBody;
        private Animator animator;
        private AudioSource audioPlayer;

        //Game
        private bool chase = false;
        private bool cooldown = false;
        private bool onGround = true;
        private int collisionCount = 0;

        //Animator hash values
        private static int chaseHash = Animator.StringToHash("Chase");
        private static int speedHash = Animator.StringToHash("Speed");
        private static int groundHash = Animator.StringToHash("onGround");

        // Start is called before the first frame update
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();

            myRigidBody.freezeRotation = true;
            stompHeightCheck = 1.7f;
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

        //Bugfix
        void OnCollisionEnter(Collision collision)
        {
            //Do nothing
        }

        //Contact with Mario
        private void OnCollisionStay(Collision collision)
        {
            if (!stomped)
            {
                //All collisions
                if (!cooldown)
                {
                    foreach (ContactPoint contact in collision.contacts)
                    {
                        Player p = contact.otherCollider.transform.GetComponent<Player>();
                        if (p != null)
                        {
                            Vector3 push = new Vector3(p.transform.position.x - transform.position.x, 0f, p.transform.position.z - transform.position.z).normalized;
                            p.Knockback(push * -10f + Vector3.down);
                            p.transform.position += push * 0.2f;
                            transform.position += push * -0.2f;

                            //From above
                            if (p.transform.position.y > transform.position.y + transform.localScale.y * stompHeightCheck)
                            {
                                transform.position += Vector3.up * 0.25f;
                                myRigidBody.velocity += push * -10f + Vector3.up * 3f;
                                myRigidBody.drag = 0f;
                                StopAllCoroutines();
                                StartCoroutine(Stomp(p));
                                break;
                            }

                            //From below
                            else
                            {
                                p.BreakSpeedCap();
                                p.PlaySound(stompSFX);
                                WhenHurtPlayer(p);
                                break;
                            }

                        }

                        //Smack around physics objects
                        else
                        {
                            Rigidbody r = contact.otherCollider.transform.GetComponent<Rigidbody>();
                            if (r != null && !r.isKinematic)
                            {
                                Stompable s = contact.otherCollider.transform.GetComponent<Stompable>();
                                if (s == null)
                                {
                                    Vector3 push = new Vector3(r.transform.position.x - transform.position.x, 0f, r.transform.position.z - transform.position.z).normalized;
                                    r.velocity = push * 18f + Vector3.up * 3f;
                                    audioPlayer.PlayOneShot(stompSFX);
                                    WhenHurtPlayer(null);
                                    break;
                                }
                            }
                        }
                    }
                }

                //Move on collision stay to here
                OnCollisionStayStompable(collision);
            }
        }

        //What to do when stomped. Override this.
        protected override void WhenStomped()
        {
            chase = false;
            onGround = false;
            animator.SetBool(groundHash, false);
            stomped = false;
            StartCoroutine(Cooldown(0.2f));
        }

        //What to do when hurting player. Override this.
        protected override void WhenHurtPlayer(Player p)
        {
            chase = false;
            myRigidBody.velocity = Vector3.zero;
            StopAllCoroutines();
            StartCoroutine(Cooldown(0.2f));
        }

        //Move fixed update to here. Override this.
        protected override void FixedUpdateStompable()
        {
            myRigidBody.rotation = Quaternion.Euler(0f, myRigidBody.rotation.eulerAngles.y, 0f);

            //Collision delay
            if (collisionCount > 0)
            {
                collisionCount--;
                if (collisionCount == 0)
                {
                    onGround = false;
                    animator.SetBool(groundHash, false);
                }
            }

            //Manage drag
            if (!onGround) myRigidBody.drag = 0f;
            else if (chase) myRigidBody.drag = 1.7f;
            else myRigidBody.drag = 100f;

            //Cooldown
            if (!cooldown)
            {
                //Start chase
                if (onGround && !chase && Player.singleton.CanBeChased(transform.position, 10f))
                {
                    chase = true;
                    StartCoroutine(Cooldown(0.9f));
                }

                //End chase
                else if (!onGround || (chase && !Player.singleton.CanBeChased(transform.position, 10f)))
                {
                    chase = false;
                    StartCoroutine(Cooldown(1.1f));
                    if (onGround) myRigidBody.velocity = Vector3.zero;
                }

                //Chase
                if (chase)
                {
                    //Change direction
                    Player.singleton.LookAtMe(transform);

                    //Move in direction
                    myRigidBody.velocity += transform.forward * 12.5f * Time.fixedDeltaTime; //0.25f

                    //Speed cap
                    Vector2 mvmntSpeed = new Vector2(myRigidBody.velocity.x, myRigidBody.velocity.z);
                    if (mvmntSpeed.sqrMagnitude > 16f)
                    {
                        mvmntSpeed.Normalize();
                        mvmntSpeed = mvmntSpeed * 4f;
                        myRigidBody.velocity = new Vector3(mvmntSpeed.x, myRigidBody.velocity.y, mvmntSpeed.y);
                    }

                }
            }
        }

        //Move on collision stay to here. Override this.
        protected override void OnCollisionStayStompable(Collision collision)
        {
            //Break bricks
            Brick b = collision.transform.GetComponent<Brick>();
            if (b != null)
            {
                b.BreakBrick();
            }
            else
            {
                BrickHard bb = collision.transform.GetComponent<BrickHard>();
                if (bb != null)
                {
                    bb.BreakBrick();
                }
            }
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.6f)
                {
                    if (!onGround)
                    {
                        onGround = true;
                        animator.SetBool(groundHash, true);
                    }
                    collisionCount = 8;
                    break;
                }
            }
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
