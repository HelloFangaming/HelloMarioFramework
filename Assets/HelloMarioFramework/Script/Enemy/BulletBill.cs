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
    public class BulletBill : Stompable
    {

        //Components
        private Rigidbody myRigidBody;
        private AudioSource audioPlayer;
        private Collider myCollider;

        //Audio clips
        [SerializeField]
        private AudioClip launchSFX;
        [SerializeField]
        private AudioClip warningSFX;

        //Explosion particles
        [SerializeField]
        private GameObject explosion;
        
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody>();
            audioPlayer = GetComponent<AudioSource>();
            myCollider = GetComponent<Collider>();

            stompHeightCheck = 0.2f;
            Player.singleton.PlaySound(launchSFX);
            StartCoroutine(Delay());
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
            if (myRigidBody.detectCollisions && Player.singleton.CanBeChased(transform.position, 10f))
            {
                Quaternion r = Quaternion.LookRotation(new Vector3(Player.singleton.transform.position.x, transform.position.y, Player.singleton.transform.position.z) - transform.position, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, r, 100f * Time.fixedDeltaTime);
            }
            transform.position += transform.forward * 6f * Time.fixedDeltaTime;
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

            //Explode on impact
            if (!stomped) StartCoroutine(Explode());
        }

        private IEnumerator Delay()
        {
            myRigidBody.detectCollisions = false;
            yield return new WaitForSeconds(0.28f * transform.localScale.z);
            myRigidBody.detectCollisions = true;

            //Explode eventually
            yield return new WaitForSeconds(14f);
            audioPlayer.PlayOneShot(warningSFX);
            yield return new WaitForSeconds(2.7f);
            StartCoroutine(Explode());
        }

        private IEnumerator Explode()
        {
            //audioPlayer.PlayOneShot(explodeSFX);
            GameObject o = Instantiate(explosion);
            o.transform.position = transform.position;
            stomped = true;
            transform.localScale = Vector3.zero;
            myCollider.enabled = false;
            myRigidBody.isKinematic = true;
            myRigidBody.detectCollisions = false;
            yield return new WaitForSeconds(2.7f);
            Destroy(gameObject);
        }

    }
}
