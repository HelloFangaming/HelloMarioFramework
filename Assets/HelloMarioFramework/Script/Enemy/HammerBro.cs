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
    public class HammerBro : Stompable
    {

        //Components
        private Rigidbody myRigidBody;
        private Animator animator;
        private AudioSource audioPlayer;
        private Collider myCollider;

        //Audio clips
        [SerializeField]
        private AudioClip voiceSFX;

        //Game
        private Vector3 jumpDirection;
        private bool canAttack = true;
        private bool canTurn = true;
        private bool attackFinished = false;
        private bool onGround = true;
        private int collisionCount = 0;

        //Animator hash values
        private static int attackHash = Animator.StringToHash("Attack");
        private static int stompHash = Animator.StringToHash("Stomp");
        private static int jumpHash = Animator.StringToHash("Jump");

        //Hammer prefab
        [SerializeField]
        private GameObject hammer;
        [SerializeField]
        private Transform handThatThrows;
        
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
            myCollider = GetComponent<Collider>();
            jumpDirection = transform.forward;
            myRigidBody.freezeRotation = true;
        }

        //What to do when stomped. Override this.
        protected override void WhenStomped()
        {
            canAttack = false;
            attackFinished = false;
            animator.SetBool(stompHash, true);
            myCollider.enabled = false;
            myRigidBody.isKinematic = true;
            myRigidBody.detectCollisions = false;
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
                    animator.SetBool(jumpHash, true);
                }
            }

            //Manage drag
            if (!onGround) myRigidBody.drag = 0f;
            else myRigidBody.drag = 100f;

            //Change direction
            if (canTurn)
                Player.singleton.LookAtMe(transform);

            //If player enters search radius
            if (canAttack && Player.singleton.CanBeChased(transform.position, 12f))
            {
                canAttack = false;
                StartCoroutine(Attack());
            }
        }

        //Move on collision stay to here. Override this.
        protected override void OnCollisionStayStompable(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.6f)
                {
                    if (!onGround)
                    {
                        onGround = true;
                        animator.SetBool(jumpHash, false);
                    }
                    collisionCount = 8;
                    if (attackFinished)
                    {
                        attackFinished = false;
                        canAttack = true;
                        canTurn = true;
                    }
                    break;
                }
            }
        }

        //Attack pattern
        private IEnumerator Attack()
        {
            yield return new WaitForSeconds(1f);
            canTurn = false;

            //First attack
            animator.SetBool(attackHash, true);
            yield return new WaitForSeconds(0.1f);
            animator.SetBool(attackHash, false);
            Toss();
            yield return new WaitForSeconds(0.5f);
            audioPlayer.PlayOneShot(voiceSFX);

            yield return new WaitForSeconds(0.3f);
            canTurn = true;
            yield return new WaitForSeconds(0.2f);
            canTurn = false;

            //Second attack
            animator.SetBool(attackHash, true);
            yield return new WaitForSeconds(0.1f);
            animator.SetBool(attackHash, false);
            Toss();
            yield return new WaitForSeconds(0.5f);
            audioPlayer.PlayOneShot(voiceSFX);

            yield return new WaitForSeconds(1f);

            //Jump
            animator.SetBool(jumpHash, true);
            yield return new WaitForSeconds(0.1f);
            jumpDirection = -jumpDirection;
            myRigidBody.drag = 0f;
            onGround = false;
            myRigidBody.velocity = Vector3.up * 10f + jumpDirection * 5f;
            transform.position += Vector3.up * 0.1f;

            yield return new WaitForSeconds(1f);

            //Can attack after landing
            attackFinished = true;
        }

        private void Toss()
        {
            GameObject o = Instantiate(hammer);
            o.transform.position = Vector3.Lerp(handThatThrows.position, transform.position, 0.5f);
            Player.singleton.LookAtMe(o.transform);
            o.AddComponent<HammerSpawn>();
        }

    }
}
