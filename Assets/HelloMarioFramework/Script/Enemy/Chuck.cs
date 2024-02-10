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
    public class Chuck : Stompable
    {

        //Components
        private Rigidbody myRigidBody;
        private Animator animator;
        private AudioSource audioPlayer;
        private Collider myCollider;

        //Audio clips
        [SerializeField]
        private AudioClip damageSFX;

        //Game
        private bool chase = false;
        private bool cooldown = false;
        private bool onGround = true;
        private int collisionCount = 0;
        private int health = 3;
        private bool hurting = false;

        //Animator hash values
        private static int chaseHash = Animator.StringToHash("Chase");
        private static int stompHash = Animator.StringToHash("Stomp");
        private static int speedHash = Animator.StringToHash("Speed");
        private static int hurtHash = Animator.StringToHash("Hurt");
        
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
            myCollider = GetComponentInChildren<Collider>();

            myRigidBody.freezeRotation = true;
            stompHeightCheck = 1f;
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
            OnCollisionStay(collision);
        }

        //Contact with Mario
        private void OnCollisionStay(Collision collision)
        {
            if (!stomped)
            {
                //All collisions
                foreach (ContactPoint contact in collision.contacts)
                {
                    Player p = contact.otherCollider.transform.GetComponent<Player>();
                    if (p != null)
                    {

                        //From above
                        if (p.transform.position.y > transform.position.y + transform.localScale.y * stompHeightCheck)
                        {
                            if (!hurting) StopAllCoroutines();
                            StartCoroutine(Stomp(p));
                            break;
                        }

                        //From below
                        else
                        {
                            p.Hurt(false, contact.normal);
                            if (!hurting) WhenHurtPlayer(p);
                            break;
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
            if (!hurting)
            {
                chase = false;
                animator.SetBool(chaseHash, false);
                cooldown = true;
                health--;
                if (health > 0)
                {
                    stomped = false;
                    animator.SetBool(hurtHash, true);
                    StartCoroutine(Hurt());
                }
                else
                {
                    animator.SetBool(stompHash, true);
                    myCollider.enabled = false;
                    myRigidBody.isKinematic = true;
                    myRigidBody.detectCollisions = false;
                }
            }
            else
            {
                stomped = false;
            }
        }

        //What to do when hurting player. Override this.
        protected override void WhenHurtPlayer(Player p)
        {
            chase = false;
            myRigidBody.velocity = Vector3.zero;
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
            myRigidBody.rotation = Quaternion.Euler(0f, myRigidBody.rotation.eulerAngles.y, 0f);

            //Collision delay
            if (collisionCount > 0)
            {
                collisionCount--;
                if (collisionCount == 0)
                {
                    onGround = false;
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
                    StartCoroutine(Cooldown(0.8f));
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
                    if (mvmntSpeed.sqrMagnitude > 36f)
                    {
                        mvmntSpeed.Normalize();
                        mvmntSpeed = mvmntSpeed * 6f;
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

        //Hurt
        private IEnumerator Hurt()
        {
            hurting = true;
            audioPlayer.PlayOneShot(damageSFX);
            yield return new WaitForSeconds(1f);
            animator.SetBool(hurtHash, false);
            yield return new WaitForSeconds(2f);
            hurting = false;
            cooldown = false;
        }

    }
}
