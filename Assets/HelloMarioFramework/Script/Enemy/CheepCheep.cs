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
    public class CheepCheep : Stompable
    {

        //Components
        private Rigidbody myRigidBody;
        private AudioSource audioPlayer;
        private Collider myCollider;

        //Audio clips
        [SerializeField]
        private AudioClip inSFX;
        [SerializeField]
        private AudioClip outSFX;

        //Game
        private bool swimming;
        [Tooltip("The y value that this Cheep Cheep will swim at")]
        [SerializeField]
        private float waterLevel = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody>();
            audioPlayer = GetComponent<AudioSource>();
            myCollider = GetComponentInChildren<Collider>();

            myRigidBody.freezeRotation = true;
            stompHeightCheck = 0.2f;
            swimming = (transform.position.y <= waterLevel);
            myRigidBody.isKinematic = swimming;
        }

        //What to do when stomped. Override this.
        protected override void WhenStomped()
        {
            myCollider.enabled = false;
            myRigidBody.isKinematic = true;
            myRigidBody.detectCollisions = false;
        }

        //Move fixed update to here. Override this.
        protected override void FixedUpdateStompable()
        {
            myRigidBody.rotation = Quaternion.Euler(0f, myRigidBody.rotation.eulerAngles.y, 0f);

            //Not swimming
            if (!swimming)
            {
                //Move in direction
                myRigidBody.velocity += transform.forward * 12.5f * Time.fixedDeltaTime; //0.25f

                //Speed cap
                Vector2 mvmntSpeed = new Vector2(myRigidBody.velocity.x, myRigidBody.velocity.z);
                if (mvmntSpeed.sqrMagnitude > 16f)
                {
                    mvmntSpeed.Normalize();
                    mvmntSpeed = mvmntSpeed * 3f;
                    myRigidBody.velocity = new Vector3(mvmntSpeed.x, myRigidBody.velocity.y, mvmntSpeed.y);
                }

                //Land in water
                if (transform.position.y <= waterLevel)
                {
                    swimming = true;
                    myRigidBody.isKinematic = true;
                    audioPlayer.PlayOneShot(inSFX);
                }
            }

            //If player is nearby
            if (Player.singleton.CanBeChased(transform.position, 10f))
            {
                //Change direction
                Player.singleton.LookAtMe(transform);

                //If swimming, jump out of water
                if (swimming)
                {
                    swimming = false;
                    myRigidBody.isKinematic = false;
                    audioPlayer.PlayOneShot(outSFX);
                    myRigidBody.velocity = Vector3.up * 15f;
                }
            }

        }

        //Move on collision stay to here. Override this.
        protected override void OnCollisionStayStompable(Collision collision)
        {
            if (!swimming)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    if (Vector3.Dot(contact.normal, Vector3.up) > 0.6f)
                    {
                        //Bounce on floor
                        myRigidBody.velocity = new Vector3(myRigidBody.velocity.x, 6f, myRigidBody.velocity.z);
                        break;
                    }
                }
            }
        }

    }
}
