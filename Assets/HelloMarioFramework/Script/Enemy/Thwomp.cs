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
    public class Thwomp : MonoBehaviour
    {

        //Components
        private Rigidbody myRigidBody;
        private AudioSource audioPlayer;

        //Face transforms
        private Transform[] readyFace;
        private Transform[] fallFace;
        private Transform[] upFace;

        //Audio clips
        [SerializeField]
        private AudioClip thwompSFX;
        [SerializeField]
        private AudioClip thwompVoiceSFX;

        //Ready to fall
        private bool ready = true;
        private bool falling = false;
        private bool up = false;
        private float yStart;
        
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
            readyFace = new Transform[] { transform.GetChild(5), transform.GetChild(6), transform.GetChild(7) };
            fallFace = new Transform[] { transform.GetChild(8), transform.GetChild(9), transform.GetChild(10), transform.GetChild(11) };
            upFace = new Transform[] { transform.GetChild(1), transform.GetChild(2), transform.GetChild(3), transform.GetChild(4) };
            yStart = transform.position.y;
        }

        void FixedUpdate()
        {
            if (!ready && !falling && transform.position.y >= yStart)
            {
                ready = true;
                up = false;
                transform.position = new Vector3(transform.position.x, yStart, transform.position.z);
                SetReadyFace(true);
            }
            if (up) transform.position += Vector3.up * 4f * Time.fixedDeltaTime;
        }

        //Collision with player
        private void OnTriggerStay(Collider collision)
        {
            if (ready)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    SetFallFace(true);
                    ready = false;
                    falling = true;
                    myRigidBody.useGravity = true;
                    myRigidBody.isKinematic = false;
                }
            }

        }

        //Land on ground
        private void OnCollisionEnter(Collision collision)
        {
            if (falling)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    //If collision was from below
                    if (Vector3.Dot(contact.normal, Vector3.up) > 0.6f)
                    {
                        falling = false;
                        Player p = collision.transform.GetComponent<Player>();
                        if (p != null)
                        {
                            p.Hurt(false, contact.normal);
                        }
                    }
                }
                if (!falling)
                {
                    audioPlayer.PlayOneShot(thwompSFX);
                    audioPlayer.PlayOneShot(thwompVoiceSFX);
                    myRigidBody.velocity = Vector3.zero;
                    myRigidBody.useGravity = false;
                    myRigidBody.isKinematic = true;
                    StartCoroutine(WaitAndReturn());
                }
            }
        }

        //Wait to return
        private IEnumerator WaitAndReturn()
        {
            yield return new WaitForSeconds(1f);
            up = true;
            SetUpFace(true);
        }

        //Up face
        private void SetReadyFace(bool b)
        {
            for (int i = 0; i < readyFace.Length; i++)
            {
                readyFace[i].gameObject.SetActive(b);
            }
            if (b)
            {
                SetFallFace(false);
                SetUpFace(false);
            }
        }

        //Up face
        private void SetFallFace(bool b)
        {
            for (int i = 0; i < fallFace.Length; i++)
            {
                fallFace[i].gameObject.SetActive(b);
            }
            if (b)
            {
                SetReadyFace(false);
                SetUpFace(false);
            }
        }

        //Up face
        private void SetUpFace(bool b)
        {
            for (int i = 0; i < upFace.Length; i++)
            {
                upFace[i].gameObject.SetActive(b);
            }
            if (b)
            {
                SetReadyFace(false);
                SetFallFace(false);
            }
        }

    }
}
