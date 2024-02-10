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
    public class Stompable : MonoBehaviour
    {

        //Audio clips
        [SerializeField]
        protected AudioClip stompSFX;
        [SerializeField]
        protected AudioClip poofSFX;

        //Smoke particles
        [SerializeField]
        protected GameObject smoke;

        //Game
        protected bool stomped = false;
        protected float stompHeightCheck = 0.7f;

        //Get squished
        private void FixedUpdate()
        {

            //Move fixed update to here
            if (!stomped) FixedUpdateStompable();

            //Stomp
            else if (transform.localScale.y > 0.2f)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - 5f * Time.fixedDeltaTime, transform.localScale.z); //0.1f
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
                            StopAllCoroutines();
                            StartCoroutine(Stomp(p));
                            break;
                        }

                        //From below
                        else
                        {
                            p.Hurt(false, contact.normal);
                            WhenHurtPlayer(p);
                            break;
                        }

                    }
                }

                //Move on collision stay to here
                OnCollisionStayStompable(collision);
            }
        }

        //Dissapear after stomp
        protected IEnumerator Stomp(Player p)
        {
            stomped = true;
            WhenStomped();
            p.PlaySound(stompSFX);
            if (!p.IsPound())
                p.BounceUp(12f);
            if (stomped)
            {
                yield return new WaitForSeconds(1.45f);
                p.PlaySound(poofSFX);
                GameObject o = Instantiate(smoke);
                o.transform.position = transform.position;
                o.transform.rotation = transform.rotation;
                o.transform.localScale = o.transform.localScale * transform.localScale.x * 3f;
                Destroy(gameObject);
            }
        }

        //What to do when stomped. Override this.
        protected virtual void WhenStomped() { }

        //What to do when hurting player. Override this.
        protected virtual void WhenHurtPlayer(Player p) { }

        //Move fixed update to here. Override this.
        protected virtual void FixedUpdateStompable() { }

        //Move on collision stay to here. Override this.
        protected virtual void OnCollisionStayStompable(Collision collision) { }

    }
}
