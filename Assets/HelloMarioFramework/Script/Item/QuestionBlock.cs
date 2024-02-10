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
    public class QuestionBlock : MonoBehaviour
    {

        //Sprout
        public enum Sprout { Coin, MultiCoin, Mushroom };
        [SerializeField]
        private Sprout sprout;

        //Components
        private Animator animator;
        private AudioSource audioPlayer;

        //Audio clips
        [SerializeField]
        private AudioClip bumpSFX;

        //Animator hash values
        private static int bumpHash = Animator.StringToHash("Bump");

        //Game
        [SerializeField]
        protected GameObject coin;
        [SerializeField]
        protected GameObject mushroom;
        protected bool bumpable = true;
        protected int timer = 0;
        
        void Start()
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
            audioPlayer = gameObject.AddComponent<AudioSource>();
        }

        //Bump on contact
        private void OnCollisionEnter(Collision collision)
        {
            if (bumpable)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    foreach (ContactPoint contact in collision.contacts)
                    {
                        if (Vector3.Dot(contact.normal, Vector3.up) > 0.9f)
                        {
                            StartCoroutine(Bump(1));
                            break;
                        }
                        else if (p.IsPound() && Vector3.Dot(contact.normal, Vector3.down) > 0.9f)
                        {
                            StartCoroutine(Bump(-1));
                            break;
                        }
                    }
                }
            }
        }

        //Before block moves up, make coins here
        protected virtual void BlockHitCoin()
        {
            if (sprout == Sprout.Coin || sprout == Sprout.MultiCoin)
            {
                GameObject o = Instantiate(coin, transform);
                o.transform.localPosition = Vector3.up * 0.5f;
                SaveData.save.CollectCoin();
            }
        }

        //Block hit
        protected virtual void BlockHit()
        {
            switch (sprout)
            {
                case Sprout.Coin:
                    Empty();
                    break;
                case Sprout.MultiCoin:
                    if (timer == 2) Empty();
                    else
                    {
                        bumpable = true;
                        if (timer == 0)
                        {
                            timer = 1;
                            StartCoroutine(Timer());
                        }
                    }
                    break;
                case Sprout.Mushroom:
                    GameObject o = Instantiate(mushroom, transform);
                    Empty();
                    break;
            }
        }

        //Bump
        public IEnumerator Bump(int i)
        {
            bumpable = false;
            audioPlayer.PlayOneShot(bumpSFX);
            BlockHitCoin();

            animator.SetInteger(bumpHash, i);
            yield return new WaitForSeconds(0.24f);
            animator.SetInteger(bumpHash, 0);

            BlockHit();
        }

        //Multicoin timer
        protected IEnumerator Timer()
        {
            yield return new WaitForSeconds(3.2f);
            timer = 2;
        }

        //Become empty
        protected void Empty()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }

    }
}
