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
    public class Hazard : MonoBehaviour
    {

        [Tooltip("Whether to use burn voice")]
        [SerializeField]
        public bool burn;

        //Bugfix
        void OnCollisionEnter(Collision collision)
        {
            OnCollisionStay(collision);
        }

        //Hurt player
        private void OnCollisionStay(Collision collision)
        {
            //All collisions
            foreach (ContactPoint contact in collision.contacts)
            {
                Player p = collision.transform.GetComponent<Player>();
                if (p != null)
                {
                    p.Hurt(burn, contact.normal);
                }
            }
        }
    }
}
