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
    public class Powerup : MonoBehaviour
    {

        //Sprout
        public enum Behavior { Heal, FullHeal, Hurt };
        [SerializeField]
        private Behavior behavior;

        //Audio clips
        [SerializeField]
        private AudioClip collectSFX;

        //Game
        private bool ready = false;
        private bool collected = false;
        private float sproutY;
        
        void Start()
        {
            transform.localScale = Vector3.one * 0.9f;
            sproutY = transform.position.y + 1f;
        }
        
        void FixedUpdate()
        {
            if (!ready)
            {
                if (transform.position.y < sproutY)
                    transform.position += Vector3.up * 1f * Time.fixedDeltaTime;
                else
                {
                    ready = true;
                    transform.position = new Vector3(transform.position.x, sproutY, transform.position.z);
                }
            }
        }

        //Collision with player
        private void OnTriggerStay(Collider collision)
        {
            if (ready && !collected)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    collected = true;

                    switch (behavior)
                    {
                        case Behavior.Heal:
                            p.Heal();
                            p.PlaySound(collectSFX);
                            p.CollectItemVoice();
                            break;
                        case Behavior.FullHeal:
                            p.Heal();
                            p.Heal();
                            p.PlaySound(collectSFX);
                            p.CollectItemVoice();
                            break;
                        case Behavior.Hurt:
                            p.Hurt(false, Vector3.up);
                            break;
                    }

                    Destroy(gameObject);
                }
            }
        }

    }
}
