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
    public class Donut : MonoBehaviour
    {

        //Components
        private AudioSource audioPlayer;

        //Audio clips
        [SerializeField]
        private AudioClip shakeSFX;
        [SerializeField]
        private AudioClip fallSFX;

        //Game
        private bool falling = false;
        private bool waiting = false;
        private bool shaking = false;
        private bool growing = false;
        private Vector3 start;
        private Transform shakeChild;
        private int childCount;
        
        void Start()
        {
            audioPlayer = gameObject.AddComponent<AudioSource>();
            start = transform.position;
            shakeChild = transform.GetChild(0);
            childCount = transform.childCount;
        }
        
        void FixedUpdate()
        {
            if (falling)
            {
                transform.position += Vector3.down * 10f * Time.fixedDeltaTime;

            }
            else if (waiting)
            {
                shaking = !shaking;
                if (shaking) shakeChild.localPosition = new Vector3(-0.1f, 0f, 0f);
                else shakeChild.localPosition = new Vector3(0.1f, 0f, 0f);
            }
            else if (growing)
            {
                transform.localScale += Vector3.one * 0.5f * Time.fixedDeltaTime;
                if (transform.localScale.x >= 1f)
                {
                    transform.localScale = Vector3.one;
                    growing = false;
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!waiting && !growing)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    if (Vector3.Dot(contact.normal, Vector3.down) > 0.9f)
                    {
                        waiting = true;
                        StartCoroutine(WaitToFall());
                        break;
                    }
                }
            }
        }

        public IEnumerator WaitToFall()
        {
            audioPlayer.PlayOneShot(shakeSFX);
            yield return new WaitForSeconds(1f);
            audioPlayer.PlayOneShot(fallSFX);
            falling = true;
            yield return new WaitForSeconds(10f);
            if (transform.childCount != childCount) Destroy(transform.GetChild(childCount).gameObject); //Prevent the player from being teleported back up if still on
            falling = false;
            waiting = false;
            shakeChild.localPosition = Vector3.zero;
            transform.position = start;
            transform.localScale = Vector3.zero;
            growing = true;
        }

    }
}
